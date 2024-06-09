using magisterDiplom.Model;
using magisterDiplom.Utils;
using newAlgorithm;
using newAlgorithm.Model;
using newAlgorithm.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magisterDiplom.Fabric
{
    public class BufferSchedule : Schedule
    {
        /// <summary>
        /// Конструктор выполняющий создание экземпляра данного класса 
        /// </summary>
        public BufferSchedule(Config config)
        {
            this.config = config;

            startProcessing = new Dictionary<int, List<List<int>>>();
        }

        public override bool Build(List<List<int>> matrixA)
        {
            // Объявляем общее количество пакетов заданий
            int batchCount = 0;

            // Для каждого типа в пакетах заданий
            for (int dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                // Вычисляем количество ПЗ
                batchCount += matrixA.Count();

            // Отчищяем существующие расписание
            this.schedule?.Clear();

            // Инициализируем список пакетов заданий
            this.schedule = new List<Model.Batch>(batchCount);

            // Для каждого типа в пакетах заданий
            for (int dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                // Для каждого ПЗ
                for (int batch = 0; batch < matrixA[dataType].Count(); batch++) {

                    // Добавляем ПЗ в конец расписания 
                    this.schedule.Add(new Model.Batch(dataType, matrixA[dataType][batch]));

                    // Выполняем поиск наилучшего расписания ПЗ
                    SearchByPosition(5);
                }
            
            //// Если визуализация включена отображаем Excel
            //if (Form1.vizualizationOn)
            //{
            //    viz.CreateExcelAppList(deviceCount, dataTypesCount);
            //    viz.Visualize(tnMatrix, proccessingTimeMatrix, rMatrix);
            //}

            return true;
        }

        
        /// <summary>
        /// Данная функция выполняет локальную оптимизацию составов ПЗ
        /// </summary>
        /// <param name="swapCount">Количество перестановок</param>
        private void SearchByPosition(int swapCount = 999999)
        {
            
            // Объявляем лучшее расписание
            List<Model.Batch> bestSchedule = new List<Model.Batch>(this.schedule);
            
            // Вычисляем матрицу моментов времени начала выполнения заданий
            this.CalcStartProcessing();

            // Объявляем значение наилучшего критерия f2
            int bestTime = this.GetPreMUtility();

            // Устанавливаем лучшее расписание
            bestSchedule = new List<Model.Batch>(this.schedule);
            
            // Объявляем значение текущего критерия f2
            int newTime;

            // Вычисляем матрицу моментов времени начала выполнения заданий
            this.CalcStartProcessing();

            // Выполняем заявленное количество перестановок, заявленно количество раз
            for (int batchIndex = schedule.Count - 1; batchIndex > 0 && swapCount > 0; batchIndex--, swapCount--)
            {

                // Выполняем перестановку
                (this.schedule[batchIndex - 1], this.schedule[batchIndex]) = (this.schedule[batchIndex], this.schedule[batchIndex - 1]);

                // Вычисляем матрицу моментов времени начала выполнения заданий
                this.CalcStartProcessing();

                // Высчитываем новый критерий makespan
                newTime = this.GetPreMUtility();

                // Если новое время лучше, то выполняем переопределение
                if (newTime > bestTime)
                {

                    // Переопределяем лучшее расписание
                    bestSchedule = new List<Model.Batch>(schedule);

                    // Переопределяем лучшее время для лучшего расписания
                    bestTime = newTime;
                }
            }

            // Выполняем переопределение наилучшего раысписания составов ПЗ
            this.schedule = bestSchedule;
        }

        private int GetDowntimeByDevice(int device)
        {

            // Объявляем и инициализируем простои
            int downtime = 0;

            // Подсчитываем простои связанные с наладкой
            downtime += startProcessing[device].First().First();

            // Для кажого задания пакета на первой позиции
            for (int job = 1; job < startProcessing[device].First().Count(); job++)

                // Подсчитываем простои между заданиями
                downtime +=

                    // Момент начала времени выполнения текущего задания
                    startProcessing[device][0][job] -

                    // Момент окончания времени выполнения предыдущего задания
                    (
                        // Момент начала времени выполнения предыдущего задания
                        startProcessing[device][0][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device][schedule[0].Type]
                    );

            
            // Для каждого пакета со второго выполняем обработку
            for (int batchIndex = 1; batchIndex < startProcessing[device].Count(); batchIndex++)
            {

                // Подсчитываем простои между пакетами
                downtime +=

                    // Момент начала времени выполнения первого задания текущего пакет
                    startProcessing[device][batchIndex][0] -

                    // Момент начала времени выполнения последнего задания на предыдущем пакете
                    (startProcessing[device][batchIndex - 1].Last() +

                    // Время выполнения задания в предыдущем пакете
                    config.proccessingTime[device][schedule[batchIndex - 1].Type]);

                // Для кажого задания пакета на первой позиции
                for (int job = 1; job < startProcessing[device][batchIndex].Count(); job++)

                    // Подсчитываем простои между заданиями
                    downtime +=

                        // Момент начала времени выполнения текущего задания
                        startProcessing[device][batchIndex][job] -

                        // Момент окончания времени выполнения предыдущего задания
                        (
                            // Момент начала времени выполнения предыдущего задания
                            startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device][schedule[batchIndex].Type]
                        );
            }
            
            // Возвращаем результат
            return downtime;
        }

        public int GetPreMUtility()
        {

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < this.config.deviceCount; device++)
            {

                // Добавляем момент времени окончания всех заданий на приборе
                sum +=
                    
                    // Момент начала времени выполнения на последнем задании в последнем пакете
                    this.startProcessing[device].Last().Last() +

                    // Время выполнения последнего заданий в последенем пакете
                    this.config.proccessingTime[device][this.schedule.Last().Type];

                // Вычитаем простои
                sum -= this.GetDowntimeByDevice(device);
            }

            // Возвращаем критерий
            return sum;
        }

        private void CalcStartProcessing()
        {

            // Отчищаяем данные
            this.startProcessing?.Clear();

            // Инициализируем новые данные матрицы
            for (int device = 0; device < this.config.deviceCount; device++)
            {
                this.startProcessing.Add(device, new List<List<int>>(this.schedule.Count()));
                for (int batchIndex = 0; batchIndex < this.schedule.Count(); batchIndex++)
                    this.startProcessing[device].Add(ListUtils.InitVectorInt(this.schedule[batchIndex].Size));
            }

            // Количество пакетов для всех типов данных, так же известное как n_p
            int maxBatchesPositions = schedule.Count();

            // Предыдущий тип
            int previousDataType = 0;
            int previousJobCount = 0;

            // Для всех пакетов выполняем обработку. batchIndex так же известен, как h
            for (int batchIndex = 0; batchIndex < maxBatchesPositions; batchIndex++)
            {
                
                int currentJobCount = schedule[batchIndex].Size;
                int currentDataType = schedule[batchIndex].Type;

                // Для всех заданий выполняем обработку. job так же известен, как q
                for (int job = 0; job < currentJobCount; job++)
                {

                    // Для всех приборов выполняем обработку
                    for (int device = 0; device < config.deviceCount; device++)
                    {

                        // Для первого прибора выполняем обработку
                        if (device == 0)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            // 4.1 (4)
                            if (batchIndex == 0)
                            {

                                // Если данное задание является первым в пакете, добавляем его в матрицу
                                if (job == 0)

                                    // TODO: Релазиовать наладку приборов
                                    startProcessing[device][batchIndex][job] = 0;

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                if (job > 0 && job <= config.buffer)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device][batchIndex][job - 1];
                                    int procTime = config.proccessingTime[device][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Добавляем время конца выполнения задания матрицу
                                    startProcessing[device][batchIndex][job] = stopTime;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                if (job > config.buffer && job <= currentJobCount - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    // int startTime = tnMatrix[device + 1, batchIndex + 1, job + 1 - 1];
                                    int startTime = startProcessing[device][batchIndex][job - 1];
                                    int procTime = config.proccessingTime[device][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Время начала задания на следующем приборе 
                                    // int startBufferTime = tnMatrix[device + 1 + 1, batchIndex + 1, job + 1 - bufferSize];
                                    int startBufferTime = startProcessing[device + 1][batchIndex][job - config.buffer];

                                    // Выбираем между время между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    int resultTime = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][job] = resultTime;
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Для любого не первого пакета в последовательности выполняем обработку
                            if (batchIndex >= 1 && batchIndex <= maxBatchesPositions - 1)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job == 0)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device][batchIndex - 1].Last();
                                    int procTime = config.proccessingTime[device][schedule[batchIndex - 1].Type];

                                    // Высчитываем время переналадки с предыдущего типа на текущий
                                    // int changeTime = timeChangeover[device + 1, previousType, currentDataType];
                                    int changeTime = config.changeoverTime[device][previousDataType][currentDataType];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = changeTime + startTime + procTime;

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = 0;
                                    if (previousJobCount - config.buffer >= 0 && previousJobCount - config.buffer < startProcessing[device + 1][batchIndex - 1].Count())
                                        startBufferTime = startProcessing[device + 1][batchIndex - 1][previousJobCount - config.buffer];

                                    // Выбираем между время между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][job] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                if (job > 0 && job <= config.buffer - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device][batchIndex][job - 1];
                                    int procTime = config.proccessingTime[device][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = 0;
                                    if (previousJobCount - config.buffer + job >= 0 && previousJobCount - config.buffer + job < startProcessing[device + 1][batchIndex - 1].Count())
                                        startBufferTime = startProcessing[device + 1][batchIndex - 1][previousJobCount - config.buffer + job];

                                    // Выбираем между время между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    stopTime = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][job] = stopTime;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                if (job >= config.buffer && job <= currentJobCount - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device][batchIndex][job - 1];
                                    int procTime = config.proccessingTime[device][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = startProcessing[device + 1][batchIndex][job - config.buffer];

                                    // Выбираем между концом выполнения текущего задания и началом выполнения задания в буфере на следующем приборе
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][job] = result;
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }
                        }

                        // Для любого не первого и не последнего прибора выполняем обработку
                        //4.3 (6)
                        if (device >= 1 && device <= config.deviceCount - 2)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            if (batchIndex == 0)
                            {

                                // Для первого задания в пакете выполняем обработку
                                // 49
                                if (job == 0)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][0][0];
                                    int procTime = config.proccessingTime[device - 1][schedule[0].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTime = startTime + procTime;

                                    // Добавляем результат в матрицу
                                    startProcessing[device][0][0] = stopTime;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                //50
                                if (job > 0 && job <= config.buffer)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][0][job];
                                    int procTime = config.proccessingTime[device - 1][schedule[0].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[device][0][job - 1];
                                    procTime = config.proccessingTime[device][schedule[0].Type];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][0][job] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                if (job > config.buffer && job <= currentJobCount - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][0][job];
                                    int procTime = config.proccessingTime[device - 1][schedule[0].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[device][0][job];
                                    procTime = config.proccessingTime[device][schedule[0].Type];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе предыдущего задания через буфер
                                    int startBufferTime = startProcessing[device + 1][0][job - config.buffer];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][0][job] = result;
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Для любого не первого пакета в последовательности выполняем обработку
                            //4.4 (7)
                            if (batchIndex >= 1 && batchIndex <= maxBatchesPositions - 1)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job == 0)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][batchIndex][0];
                                    int procTime = config.proccessingTime[device - 1][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[device][batchIndex - 1].Last();
                                    procTime = config.proccessingTime[device][schedule[batchIndex - 1].Type];

                                    // Время переналадки прибора с предыдущего типа на текущий
                                    // int changeTime = timeChangeover[device + 1, previousType, currentDataType];
                                    int changeTime = config.changeoverTime[device][previousDataType][currentDataType];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime + changeTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе предыдущего задания через буфер

                                    // Время начала задания на следующем приборе предыдущего пакета предыдущего задания
                                    int startBufferTime = 0;
                                    if (previousJobCount - config.buffer >= 0 && previousJobCount - config.buffer < startProcessing[device + 1][batchIndex - 1].Count())
                                        startBufferTime = startProcessing[device + 1][batchIndex - 1][previousJobCount - config.buffer];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][0] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание не первое и не превышает размер буфера, выполняем обработку
                                //53
                                if (job > 0 && job <= config.buffer - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][batchIndex][job];
                                    int procTime = config.proccessingTime[device - 1][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[device][batchIndex][job - 1];
                                    procTime = config.proccessingTime[device][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе предыдущего задания через буфер
                                    int startBufferTime = 0;
                                    if (previousJobCount - config.buffer + job >= 0 && previousJobCount - config.buffer + job < startProcessing[device + 1][batchIndex - 1].Count())
                                        startBufferTime = startProcessing[device + 1][batchIndex - 1][previousJobCount - config.buffer + job];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][job] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 54
                                if (job >= config.buffer && job <= currentJobCount - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][batchIndex][job];
                                    int procTime = config.proccessingTime[device - 1][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[device][batchIndex][job - 1];
                                    procTime = config.proccessingTime[device][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего задания и концом выполнения предыдущего задания
                                    int stopTime = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Время начала задания на следующем приборе задания через буфер
                                    int startBufferTime = startProcessing[device + 1][batchIndex][job - config.buffer];

                                    // Выбираем между концом выполнения задания и концом выполнения задания через буфер
                                    int result = Math.Max(stopTime, startBufferTime);

                                    // Добавляем результат в матрицу
                                    startProcessing[device][batchIndex][job] = result;
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }
                        }

                        // Для последнего прибора выполняем обработку
                        if (device == config.deviceCount - 1)
                        {

                            // Для первого пакета в последовательности выполняем обработку
                            if (batchIndex == 0)
                            {

                                // Для первого задания в пакете выполняем обработку
                                if (job == 0)
                                {

                                    // Подсчитываем время выполнения для всех пакетов для предыдущих приборов
                                    int result = 0;
                                    for (int li = 0; li <= config.deviceCount - 2; li++)
                                        result += config.proccessingTime[li][schedule[0].Type];

                                    // Добавляем результат в матрицу
                                    startProcessing[config.deviceCount - 1][0][0] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }

                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 45
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (job > 0 && job <= currentJobCount - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][batchIndex][job];
                                    int procTime = config.proccessingTime[device - 1][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[device][batchIndex][job - 1];
                                    procTime = config.proccessingTime[config.deviceCount - 1][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего и предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    startProcessing[config.deviceCount - 1][batchIndex][job] = result;
                                }

                                // Продолжаем вычисления для следующего прибора
                                continue;
                            }

                            // Дл
                            // 4.5 (9)
                            if (batchIndex >= 1 && batchIndex <= maxBatchesPositions - 1)
                            {

                                // Для первого задания в пакете выполняем 
                                if (job == 0)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[device - 1][batchIndex][job];
                                    int procTime = config.proccessingTime[config.deviceCount - 2][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[config.deviceCount - 1][batchIndex - 1].Last();
                                    procTime = config.proccessingTime[config.deviceCount - 1][schedule[batchIndex - 1].Type];

                                    // Время переналадки с предыдущего типа на текущей
                                    // int changeTime = timeChangeover[config.deviceCount, previousType, currentDataType];
                                    int changeTime = config.changeoverTime[config.deviceCount - 1][previousDataType][currentDataType];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = changeTime + startTime + procTime;

                                    // Выбираем между концом выполнения текущего и предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    startProcessing[config.deviceCount - 1][batchIndex][job] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }


                                // Если данное задание превышает размер буфера, выполняем обработку
                                // 48
                                // TODO: нет необходимости обрабатывать случай (job + 1 <= currentJobCount), если значение job + 1 не имзеняется динамично, так как условие прописано в 
                                if (job > 0 && job <= currentJobCount - 1)
                                {

                                    // Высчитываем время начала и выполнения задания
                                    int startTime = startProcessing[config.deviceCount - 2][batchIndex][job];
                                    int procTime = config.proccessingTime[config.deviceCount - 2][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения задания
                                    int stopTimeCurrentJob = startTime + procTime;

                                    // Высчитываем время начала и выполнения предыдущего задания
                                    startTime = startProcessing[config.deviceCount - 1][batchIndex][job - 1];
                                    procTime = config.proccessingTime[config.deviceCount - 1][schedule[batchIndex].Type];

                                    // Высчитываем время конца выполнения предыдущего задания
                                    int stopTimePreviousJob = startTime + procTime;

                                    // Выбираем между концом выполнения текущего и предыдущего задания
                                    int result = Math.Max(stopTimeCurrentJob, stopTimePreviousJob);

                                    // Добавляем результат в матрицу
                                    startProcessing[config.deviceCount - 1][batchIndex][job] = result;

                                    // Продолжаем вычисления для следующего прибора
                                    continue;
                                }
                            }
                        }
                    }
                }

                // Переопределяем предыдущий тип и задание
                previousDataType = currentDataType;
                previousJobCount = currentJobCount;
            }
        }

        /// <summary>
        /// Вернёт тип данных по переданному индексу ПЗ
        /// </summary>
        /// <param name="batchIndex">Индекс ПЗ</param>
        /// <returns>Тип данных</returns>
        public int GetDataTypeByBatchIndex(int batchIndex)
        {
            return this.schedule[batchIndex].Type;
        }

        public override int GetMakespan()
        {
            return this.startProcessing[this.config.deviceCount - 1].Last().Last() + this.config.proccessingTime.Last()[this.schedule.Last().Type];
        }

        public override List<List<int>> GetMatrixP()
        {
            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(config.dataTypesCount);

            // Инициализируем матрицу
            for (int dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(this.schedule.Count));

            // Для каждого элемента матрицы schedule
            for (int batchIndex = 0; batchIndex < this.schedule.Count; batchIndex++)

                // Заполняем элементы матрицы количества заданий в пакетах
                res[this.schedule[batchIndex].Type][batchIndex] = 1;

            // Возвращаем результат
            return res;
        }

        public override List<List<int>> GetMatrixR()
        {
            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(this.config.dataTypesCount);

            // Инициализируем матрицу
            for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
            
                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(schedule.Count));

            // Для каждого элемента матрицы schedule
            for (int batchIndex = 0; batchIndex < this.schedule.Count; batchIndex++)

                // Заполняем элементы матрицы количества заданий в пакетах
                res[this.schedule[batchIndex].Type][batchIndex] = this.schedule[batchIndex].Size;

            // Возвращаем результат
            return res;
        }

        public override Dictionary<int, List<List<int>>> GetStartProcessing()
        {
            CalcStartProcessing();
            return startProcessing;
        }
    }
}
