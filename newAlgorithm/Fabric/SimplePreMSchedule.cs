using magisterDiplom.Model;
using magisterDiplom.Utils;
using newAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magisterDiplom.Fabric
{

    /// <summary>
    /// Класс простого расписания с ПТО, наследуется от PreMSchedule
    /// </summary>
    public class SimplePreMSchedule : PreMSchedule
    {

        List<List<int>> T_P;
        List<List<int>> T_PM;

        private void BUILD_T_PM()
        {
            T_PM.Clear();
            T_PM = new List<List<int>>();

            for (int device = 0; device < config.deviceCount; device++)
            {
                T_PM.Add(new List<int>());
                for (int batch = 0; batch < schedule.Count; batch++)
                {

                    // Добавляем н
                    T_PM[device].Add(
                        (
                            this.startProcessing[device][batch].Last() +
                            this.config.proccessingTime[device, schedule[batch].Type] +
                            this.config.preMaintenanceTimes[device]
                        ) * this.matrixY[device][batch]
                    );
                }
            }
        }

        /// <summary>
        /// Вычисляет значения матрицы моментов времени окончания ПТО приборов
        /// </summary>
        private void BUILD_T_P()
        {
            T_P.Clear();
            T_P = new List<List<int>>();

            int device;
            int batch;

            for (device = 0; device < config.deviceCount; device++)
            {
                T_P.Add(new List<int>());
                for (batch = 0; batch < this.schedule.Count; batch++)
                
                    if (this.matrixY[device][batch] != 0)
                        T_P[device].Add(

                            // Момент начала времени выполнения последнего задания в ПЗ
                            this.startProcessing[device][batch].Last() +

                            // Время выполнения последнего задания в ПЗ на текущей позиции
                            config.proccessingTime[device, schedule[batch].Type] +

                            // Время выполнения ПТО для текущего прибора
                            config.preMaintenanceTimes[device]
                         );
            }
        }

        private bool func_1()
        {

            for (int device = 0; device < config.deviceCount; device++)
            {
                for (int batch = 0; batch < this.schedule.Count; batch++)
                {
                    if (this.matrixY[device][batch] != 0)
                    {
                        int time = this.startProcessing[device][batch].Last() +
                            this.config.proccessingTime[device, this.schedule[batch].Type];

                        // TODO: ВЫЧИСЛИТЬ N_P и N_PQ для момента времени time
                        // TODO: ВЫЧИСЛИТЬ T_l ПО ФОРМУЛЕ 21

                        //-------------------------
                        // TODO: ВЫЧИСЛИТЬ P_0^l(time?) ПО ФОРМУЛАМ 17 И 22.
                        // TODO: ВЫЧИСЛИТЬ P_sys^l(time?) ПО ФОРМУЛЕ 18

                        // TODO: ЕСЛИ P_sys^l(time?) < B TO return false;
                        if (IsConstraint_CalcSysReliability(time, 0))
                            return false;
                    }
                }
            }
            return true;
        }

        private void test()
        {
            this.matrixY.Clear();
            this.matrixY = new List<List<int>>();
            List<int> I, _I;

            // ПУНКТ 3
            for (int device = 0; device < config.deviceCount; device++)
                matrixY.Add(new List<int> { 1 });

        }

        /// <summary>
        /// Конструктор выполняющий создание экземпляра данного класса 
        /// </summary>
        SimplePreMSchedule(Config config) {
            this.config = config;
        }

        /// <summary>
        /// Выполняет построение расписания
        /// </summary>
        /// <param name="matrixA">Матрица составов пакетов заданий</param>
        public void Build(List<List<int>> matrixA)
        {
            this.schedule.Clear();
            this.matrixY.Clear();

            // Объявляем тип данных
            int dataType;

            // П.1 Инициализируем множества I и I_
            List<int> I = new List<int>(capacity: this.config.dataTypesCount);
            List<int> I_ = new List<int>(capacity: this.config.dataTypesCount);
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++) {
                I.Add(dataType); I_.Add(dataType);
            }

            // П.2 Определяем тип заданий
            int _dataType = -1;
            double maxValue = -1;
            double tmpValue = -1;
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                for (int device = 0; device < this.config.deviceCount - 1; device++)
                    if (maxValue < (tmpValue = (double)(this.config.proccessingTime[device, dataType] / this.config.proccessingTime[device + 1, dataType])))
                    {
                        maxValue = tmpValue;
                        _dataType = dataType;
                    }

            
            // П.2 Объявляем ПЗ
            int batch = 0;

            // П.2 Добавляем 
            this.schedule.Add(new Model.Batch(_dataType, matrixA[_dataType][batch]));

            // П.3 Инициализируем матрицу Y
            this.matrixY = new List<List<int>>(capacity: this.config.deviceCount);
            for (int device = 0; device < this.config.deviceCount; device++)
                this.matrixY[device].Add(1);

            // П.3 Инициализируем критерий f2
            double f2_opt_criteria = 0;

            // Объявляем максимальное количество пакетов
            int maxBatchCount = 0;

            // Инициализируем максимальное количество пакетов
            for (dataType = 0; dataType < config.dataTypesCount; dataType++)
                maxBatchCount = Math.Max(maxBatchCount, matrixA[dataType].Count);

            // Выполняем обработку
            while (batch < maxBatchCount)
            {

                // Выполняем обработку для каждого типа данных
                for (dataType = 0; dataType < config.dataTypesCount; dataType++)
                {

                    // Если индекс пакета превышает максимальный размер пакетов для типа данных dataType
                    if (batch >= matrixA[dataType].Count)

                        // Продолжаем обработку для следующего типа данных
                        continue;

                    // Добавляем ПЗ в расписание 
                    schedule.Add(new magisterDiplom.Model.Batch(dataType, matrixA[dataType][batch]));

                    // Выполняем локальную оптимизацию составов ПЗ
                    // schedule = Optimization(config, schedule);
                }

                // Увеличиваем индекс пакета
                batch++;
            }
        }

        /// <summary>
        /// Выполняет построение матрицы моментов окончания времени выполнения ПТО.
        /// </summary>
        public void CalcMatrixTPM()
        {

            // Для каждого прибора выполням обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Инициализируем ПТО для прибора
                matrixTPM.Add(new List<PreMSet>());

                // Для каждого прибора в расписании выполняем обработку
                for (int batchIndex = 0; batchIndex < schedule.Count; batchIndex++)

                    // Если для текущей позиции есть ПТО
                    if (matrixY[device][batchIndex] == 1)

                        // Момент окончания времени выполнения ПТО на позиции batchIndex
                        matrixTPM[device].Add(

                            // Добавляем структуры данных
                            new PreMSet(

                                // Индекс ПЗ после которого будет выполняться ПТО
                                batchIndex,

                                // Момент начала времени выполнения последнего задания в пакете batchIndex на приборе device
                                startProcessing[device][batchIndex].Last() +

                                // Время выполнения задания с типов пакета на позиции batchIndex
                                config.proccessingTime[device, schedule[batchIndex].Type] +

                                // Время выполнения ПТО
                                config.preMaintenanceTimes[device]
                            )
                        );
            }
        }

        // ВЫРАЖЕНИЯ 1-6
        /// <summary>
        /// Выполняет построение матрицы начала времени выполнения заданий
        /// </summary>
        public void CalcStartProcessing()
        {

            // Объявляем индекс ПЗ
            int batchIndex = 0;

            // Объявляем индекс прибора
            int device = 0;

            // Объявляем индекс задания
            int job = 0;

            // Отчищаяем матрицу моментов начала времени выполнения заданий
            startProcessing.Clear();

            // Инициалиизруем матрицу заданий в пакете
            List<List<int>> times = new List<List<int>>();
            for (batchIndex = 0; batchIndex < schedule.Count(); batchIndex++)
                times.Add(ListUtils.InitVectorInt(schedule[batchIndex].Size));

            // Инициализируем словарь соответствий
            for (device = 0; device < config.deviceCount; device++)
                startProcessing.Add(device, ListUtils.MatrixIntDeepCopy(times));

            // Выполняем обработку для первого прибора
            {

                // Инициализируем индекс первого прибора
                device = 0;

                // Инициализируем индекс первого ПЗ
                batchIndex = 0;

                // Инициализируем индекс первого задания
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на 1 приборе, как наладку
                startProcessing[device][batchIndex][job] = config.changeoverTime[device][schedule[batchIndex].Type, schedule[batchIndex].Type];

                // Пробегаемся по всем заданиям пакета в первой позиции
                for (job = 1; job < schedule[batchIndex].Size; job++)

                    // Устанавливаем момент начала времени выполнения задания job
                    startProcessing[device][batchIndex][job] =

                        // Момент начала времени выполнения предыдущего задания
                        startProcessing[device][batchIndex][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device, schedule[batchIndex].Type];

                // Пробегаемся по всем возможным позициям cо второго пакета
                for (batchIndex = 1; batchIndex < schedule.Count(); batchIndex++)
                {

                    // Инициализируем индекс первого задания
                    job = 0;

                    // Момент начала времени выполнения 1 задания в пакете на позиции batchIndex
                    startProcessing[device][batchIndex][job] =

                        // Момент начала времени выполнения последнего задания в предыдущем пакете
                        startProcessing[device][batchIndex - 1].Last() +

                        // Время выполнения задания в предыдущем пакете
                        config.proccessingTime[device, schedule[batchIndex - 1].Type] +

                        // Время переналадки с предыдущего типа на текущий
                        config.changeoverTime[device][schedule[batchIndex - 1].Type, schedule[batchIndex].Type] +

                        // Время выполнения ПТО после предыдущего ПЗ
                        config.preMaintenanceTimes[0] * matrixY[device][batchIndex - 1];

                    // Пробегаемся по всем заданиям пакета в позиции batchIndex
                    for (job = 1; job < schedule[batchIndex].Size; job++)

                        // Вычисляем момент начала времени выполнения задания job в позиции batchIndex на 1 приборе
                        startProcessing[device][batchIndex][job] =

                            // Момент начала времени выполнения предыдущего задания
                            startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device, schedule[batchIndex].Type];
                }
            }

            // Пробегаемся по всем приборам со второго
            for (device = 1; device < config.deviceCount; device++)
            {

                // Инициализируем индекс первого ПЗ
                batchIndex = 0;

                // Инициализируем индекс первого задания
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на приборе device, как
                // Максимум, между временем наладки прибора на выполнение 1 задания в 1 пакете
                // и временем окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                startProcessing[device][batchIndex][job] = Math.Max(

                    // Время наладки прибора на выполнение 1 задания в 1 пакете
                    config.changeoverTime[device][schedule[batchIndex].Type, schedule[batchIndex].Type],

                    // Время окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                    startProcessing[device - 1][batchIndex][job] + config.proccessingTime[device - 1, schedule[batchIndex].Type]
                );

                // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                for (job = 1; job < schedule[batchIndex].Size; job++)

                    // Устанавливаем момент начала времени выполнения текущего задания job, как
                    // Максимум, между временем окончания предыдущего задания на текущем приборе и
                    // временем окончания текущего задания на предыдущем приборе
                    startProcessing[device][batchIndex][job] = Math.Max(

                        // Момент начала времени выполнения предыдущего задания
                        startProcessing[device][batchIndex][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device, schedule[batchIndex].Type],

                        // Момент начала времени выполнения текущего задания на предыдущем приборе
                        startProcessing[device - 1][batchIndex][job] +

                        // Время выполнения текущего задания на предыдущем приборе
                        config.proccessingTime[device - 1, schedule[batchIndex].Type]
                    );

                // Пробегаемся по всем возможным позициям пакетов
                for (batchIndex = 1; batchIndex < schedule.Count(); batchIndex++)
                {

                    // Инициализируем индекс задания
                    job = 0;

                    // Устанавливаем момент начала времени выполнения 1 задания в пакете batchIndex на приборе device,
                    // как Максимум, между временем окончания выполнения последнего задания в предыдущем пакете вместе с переналадкой и ПТО
                    // и временем окончания выполнения 1 задания в пакете на в batchIndex на предыдущем приборе
                    startProcessing[device][batchIndex][job] = Math.Max(

                        // Момент начала времени выполнения последнего задания в предыдущем ПЗ
                        startProcessing[device][batchIndex - 1].Last() +

                        // Время выполнения последнего задания в предыдущем ПЗ
                        config.proccessingTime[device, schedule[batchIndex - 1].Type] +

                        // Время переналадки с предыдущего типа на текущий
                        config.changeoverTime[device][schedule[batchIndex - 1].Type, schedule[batchIndex].Type] +

                        // Время выполнения ПТО
                        config.preMaintenanceTimes[device] * matrixY[device][batchIndex - 1],

                        // Момент начала времени выполнения 1 задания на предыдущем приборе
                        startProcessing[device - 1][batchIndex][job] +

                        // Время выполнения 1 задания на предыдущем приборе
                        config.proccessingTime[device - 1, schedule[batchIndex].Type]);

                    // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                    for (job = 1; job < schedule[batchIndex].Size; job++)

                        // Устанавливаем момент начала времени выполнения текущего задания job, как
                        // Максимум, между временем окончания предыдущего задания на текущем приборе и
                        // временем окончания текущего задания на предыдущем приборе
                        startProcessing[device][batchIndex][job] = Math.Max(

                            // Момент начала времени выполнения предыдущего задания
                            startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device, schedule[batchIndex].Type],

                            // Момент начала времени выполнения задания на предыдущем приборе
                            startProcessing[device - 1][batchIndex][job] +

                            // Время выполнения задания на предыдущем приборе
                            config.proccessingTime[device - 1, schedule[batchIndex].Type]
                        );
                }
            }
        }

        // ВЫРАЖЕНИЕ 7
        /// <summary>
        /// Возвращяет простои для переданного индекса прибора, данного расписания
        /// </summary>
        /// <returns>Время простоя для переданного индекса прибора</returns>
        public int GetDowntimeByDevice(int device)
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
                        config.proccessingTime[device, schedule[0].Type]
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
                    config.proccessingTime[device, schedule[batchIndex - 1].Type]);

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
                            config.proccessingTime[device, schedule[batchIndex].Type]
                        );
            }
            
            // Возвращаем результат
            return downtime;
        }

        // ВЫРАЖЕНИЕ 7 ДЛЯ ВСЕХ ПРИБОРОВ
        /// <summary>
        /// Возвращяет общие простои для данного расписания
        /// </summary>
        /// <returns>Время простоя</returns>
        public int GetDowntime()
        {

            // Объявляем и инициализируем простои
            int downtime = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Подсчитываем простои связанные с наладкой
                downtime += startProcessing[device].First().First();

                // Для кажого задания пакета на первой позиции
                for (int job = 1; job < startProcessing[device].First().Count(); job++)

                    // Подсчитываем простои между заданиями
                    downtime +=

                        // Момент начала времени выполнения текущего задания
                        startProcessing[device][0][job] -

                        // Момент начала времени выполнения предыдущего задания
                        (startProcessing[device][0][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device, schedule[0].Type]);

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
                        config.proccessingTime[device, schedule[batchIndex - 1].Type]);

                    // Для кажого задания пакета на первой позиции
                    for (int job = 1; job < startProcessing[device][batchIndex].Count(); job++)

                        // Подсчитываем простои между заданиями
                        downtime +=

                            // Момент начала времени выполнения текущего задания
                            startProcessing[device][batchIndex][job] -

                            // Момент начала времени выполнения предыдущего задания
                            (startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device, schedule[batchIndex].Type]);
                }
            }

            // Возвращаем результат
            return downtime;
        }

        // ВЫРАЖЕНИЕ 8 ДЛЯ ОДНОГО ПРИБОРА
        /// <summary>
        /// Возвращяем полезность для прибора по переданному индексу
        /// </summary>
        /// <returns>Критерий полезности</returns>
        public int GetUtilityByDevice(int device)
        {

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Добавляем момент времени окончания всех заданий на приборе
            sum +=

                // Момент начала времени выполнения на последнем задании в последнем пакете
                this.startProcessing[device].Last().Last() +

                // Время выполнения последнего заданий в последенем пакете
                this.config.proccessingTime[device, this.schedule.Last().Type];

            // Вычитаем простои
            sum -= this.GetDowntimeByDevice(device);

            // Возвращяем критерий
            return sum;
        }

        // ВЫРАЖЕНИЕ 8
        /// <summary>
        /// Возвращяем полезность для данного расписания
        /// </summary>
        /// <returns>Время полезности</returns>
        public int GetUtility()
        {

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < this.config.deviceCount; device++)
            
                // Добавляем критерий для данного прибора
                sum += this.GetUtilityByDevice(device);
            
            // Возвращяем критерий
            return sum;
        }

        // ВЫРАЖЕНИЕ 9
        /// <summary>
        /// Возвращяет сумму полезности и интервалов между ПТО для данного расписания
        /// </summary>
        /// <returns>Сумма полезности и интервалов между ПТО</returns>
        public int GetPreMUtility()
        {

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Добавляем момент времени окончания всех заданий на приборе
                sum +=
                    
                    // Момент начала времени выполнения на последнем задании в последнем пакете
                    this.startProcessing[device].Last().Last() +

                    // Время выполнения последнего заданий в последенем пакете
                    this.config.proccessingTime[device, this.schedule.Last().Type];

                // Вычитаем простои
                sum -= this.GetDowntimeByDevice(device);

                // Выполняем подсчёт суммы интервалов времени на первом пакете ПТО
                sum += matrixTPM[device].First().TimePreM;

                // Для каждого пакета выполняем обработку
                for (int batchIndex = 1; batchIndex < matrixTPM[device].Count(); batchIndex++)

                    // Добавляем интервалы времени между ПТО разных пакетов
                    sum += matrixTPM[device][batchIndex].TimePreM - matrixTPM[device][batchIndex - 1].TimePreM;
            }

            // Возвращяем критерий
            return sum;
        }

        // ВЫРАЖЕНИЕ 10
        /// <summary>
        /// Возвращает надёжность, которая определяет вероятность находится ли некий прибор в работоспособном состоянии
        /// </summary>
        /// <param name="activity_time">Время активности прибора с момента последнего ПТО</param>
        /// <param name="device">Индекс прибора для которого расчитывается надёжность</param>
        /// <returns>Надёжность прибора по индексу device</returns>
        private double CalcReliabilityByDevice(int activity_time, int device)
        {

            // Выполняем расчёт и возврат доступности
            return (activity_time == 0) ? 1 :
                (double)config.failureRates[device] / (double)(config.restoringDevice[device] + config.failureRates[device]) +
                (double)config.restoringDevice[device] / (double)(config.restoringDevice[device] + config.failureRates[device]) *
                (double)Math.Exp(-1 * (double)(config.restoringDevice[device] + config.failureRates[device]) * (double)activity_time);
        }

        // ВЫРАЖЕНИЕ 11
        /// <summary>
        /// Возвращает надёжность, которая определяет вероятность находится ли некий прибор в работоспособном состоянии
        /// </summary>
        /// <param name="activity_time">Время активности прибора с момента старта КС</param>
        /// <param name="prem_time">Момент времени окончания последнего ПТО</param>
        /// <param name="device">Индекс прибора для которого расчитывается надёжность</param>
        /// <returns>Надёжность прибора по индексу device</returns>
        private double CalcReliabilityByDevice(int activity_time, int prem_time, int device)
        {

            // Выполняем расчёт и возврат доступности по выражению 10
            return CalcReliabilityByDevice(activity_time - prem_time, device);
        }

        // ВЫРАЖЕНИЕ 12
        /// <summary>
        /// Возвращяет время активности для конкретного прибора
        /// </summary>
        /// <param name="device">Прибор для которого расчитывается время активности</param>
        /// <param name="time">Крайний момент времени</param>
        /// <returns>Время активности</returns>
        private int GetActivityTimeByDevice(int device, int time)
        {

            // Определяем начальный индекс
            int startIndex = GetBatchIndex(device, time) + 1;
            
            // Определяем индекс ПЗ
            int batchIndex = startIndex;
            
            // Определяем количество ПЗ
            int batchCount = 0;
            
            // Определяем количество заданий
            int jobCount = 0;
            
            // Определяем время активности
            int activityTime = 0;
            
            // Для каждого пакет выполняем обработку
            for (; batchIndex < schedule.Count; batchIndex++)
            {
            
                // Если первое задани в ПЗ удовлетворяет условию
                if (startProcessing[device][batchIndex][0] >= time)
            
                    // Прекращаем обарботку
                    break;
            
                // Увеличиваем количество ПЗ
                batchCount++;
            
                // Увеличиваем активность
                activityTime += schedule[batchIndex].Size * config.proccessingTime[device, batchIndex];
            }
            
            // Если количество ПЗ равно 0
            if (batchCount == 0)
            
                // Добавляем информацию
                return 0;
            
            // Для каждого задания в последнем пакете выполняем обработку
            for (int job = 0; job < schedule[startIndex + batchCount - 1].Size; job++)
            {
            
                // Если некоторое задание в последенем ПЗ не удовлетворяет условию
                if (startProcessing[device][startIndex + batchCount - 1][job] >= time)
            
                    // Прекращаем обарботку
                    break;
            
                // Увеличиваем количество заданий
                jobCount++;
            }
            
            // Уменьшаем время
            activityTime -= (schedule[startIndex + batchCount - 1].Size - jobCount) * config.proccessingTime[device, startIndex + batchCount - 1];

            // Добавляем информацию
            return activityTime;
        }

        // ВЫРАЖЕНИЕ 13
        /// <summary>
        /// Возвращяет доступность для всех приборов для указанного момента времени
        /// </summary>
        /// <param name="time">Момент времени для которого выполняется расчёт надёжности</param>
        /// <returns>Доступность для всех приборов</returns>
        public double CalcSysReliability(int time)
        {

            // Объявляем надёжность
            double reliability = 1;

            // Объявляем время активности
            int activity_time = 0;

            // Для каждого прибора подсчитываем надёжность
            for (int device = 0; device < config.deviceCount; device++) {

                // Вычисляем время активности
                activity_time = this.GetActivityTimeByDevice(device, time);

                // Если прибор не был активным
                if (activity_time == 0)

                    // Пропускаем итерацию
                    continue;

                // Выполняем расчёт надёжности
                reliability *= this.CalcReliabilityByDevice(activity_time, device);
            }

            // Возвращяем надёжность
            return reliability;
        }

        // ВЫРАЖЕНИЕ 14
        public override int GetMakespan()
        {
            return startProcessing[config.deviceCount - 1].Last().Last() + config.proccessingTime[config.deviceCount - 1, schedule.Last().Type];
        }

        // ВЫРАЖЕНИЕ 15
        /// <summary>
        /// Возвращяет результат совпадения количества заданий
        /// </summary>
        /// <param name="matrixA">Матрица количества заданий каждого типа на пакет[dataTypesCount x mi]</param>
        /// <returns>Если количество заданий совпадают - true, иначе false</returns>
        public bool IsConstraint_JobCount(List<List<int>> matrixA)
        {

            // Объявляем количество заданий для текущего расписания
            int cur_jobCount = 0;

            // Объявляем необходимое количество заданий 
            int tar_jobCount = 0;


            // Для каждого пакета подсчитываем количество заданий
            for (int batch = 0; batch < this.schedule.Count; batch++)
            
                // Увеличиваем количество заданий в текущем расписании
                cur_jobCount += this.schedule[batch].Size;
            
            // Выполняем обход по типам
            for (int dataType = 0; dataType < matrixA.Count; dataType++)
            

                // Выполняем обход по пакетам
                for (int batch = 0; batch < matrixA[dataType].Count; batch++)

                    // Увеличиваем количество необходимых заданий
                    tar_jobCount += matrixA[dataType][batch];

            // Возвращяем результат сравнения
            return (cur_jobCount == tar_jobCount);
        }

        // ВЫРАЖЕНИЕ 16 = 9

        // ВЫРАЖЕНИЕ 17 = 11

        // ВЫРАЖЕНИЕ 18
        /// <summary>
        /// Возвращяем результат расчёта ограничения на общую надёжность
        /// </summary>
        /// <param name="time">Момент времени для которого выполняется расчёт надёжности</param>
        /// <param name="beta">Нижний уровень надёжность</param>
        /// <returns>true, если ограничение выполняется. Иначе false</returns>
        public bool IsConstraint_CalcSysReliability(int time, double beta)
        {
            return (this.CalcSysReliability(time) >= beta);
        }

        // ВЫРАЖЕНИЕ 19 ИЗБЫТОЧНО

        // ВЫРАЖЕНИЕ 20 
        /// <summary>
        /// Возвращяет индекс последнего ПЗ после которого выполняется ПТО до заданного времени
        /// </summary>
        /// <param name="device">Индекс прибора по которому будет выполняться выборка</param>
        /// <param name="time">Крайний момент времени окончания ПТО</param>
        /// <returns>Индекс ПЗ после которого будет выполняться последнее ПТО</returns>
        private int GetBatchIndex(int device, int time)
        {

            // Если список пустой
            if (matrixTPM[device].Count == 0)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Если в списке первый элемент не удовлетворяет условию
            if (matrixTPM[device][0].TimePreM > time)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Если в списке первый элемент не удовлетворяет условию
            if (matrixTPM[device][0].TimePreM == time)

                // Вернём индекс начальный индекс ПЗ
                return matrixTPM[device][0].BatchIndex;

            // Объявляем индекс
            int index = 1;

            // Для каждой ПТО выполняем обработку
            for (; index < matrixTPM[device].Count; index++)
            {

                // Если момент окончания ПТО в позиции index не удовлетворяет условиям
                if (matrixTPM[device][index].TimePreM > time)

                    // Возвращяем индекс ПЗ после которого выполнится последнее ПТО
                    return matrixTPM[device][index - 1].BatchIndex;

                // Если момент окончания ПТО в позиции index не удовлетворяет условиям
                if (matrixTPM[device][index].TimePreM == time)

                    // Возвращяем индекс ПЗ после которого выполнится последнее ПТО
                    return matrixTPM[device][index].BatchIndex;
            }

            // Индекс ПЗ после которог выполниться ПТО, последний в списке
            return matrixTPM[device][index - 1].BatchIndex;
        }

        // ВЫРАЖЕНИЕ 21 = 12

        // ВЫРАЖЕНИЕ 22 ИЗБЫТОЧНО

        // ВЫРАЖЕНИЕ 23
        /// <summary>
        /// Возвращяет результат совпадения количества пакетов заданий
        /// </summary>
        /// <param name="matrixA">Матрица количества заданий каждого типа на пакет[dataTypesCount x mi]</param>
        /// <returns>Если количество пакетов заданий совпадают - true, иначе false</returns>
        public bool IsConstraint_BatchCount(List<List<int>> matrixA)
        {

            // Объявляем количество пакетов заданий
            int cur_batchCount = this.schedule.Count;

            // Объявляем необходимое количество пакетов заданий 
            int tar_batchCount = 0;

            // Выполняем обход по типам
            for (int dataType = 0; dataType < matrixA.Count; dataType++)
            
                // Увеличиваем количество пакетов
                tar_batchCount += matrixA[dataType].Count;

            // Возвращяем true
            return (cur_batchCount == tar_batchCount);
        }

        // ВЫРАЖЕНИЕ 24
        /// <summary>
        /// Возвращяем результат совпадения одного пакета на позицию расписания
        /// </summary>
        /// <returns>Если пакет на позицию 1, то true. Иначе false</returns>
        public bool IsConstraint_OneBatchOnPos()
        {
            // Существующая реализация расписания обязывает иметь один пакет на позицию
            return true;
        }

        // ВЫРАЖЕНИЕ 25
        /// <summary>
        /// Возвращяет результат совпадения количества пакетов заданий каждого типа
        /// </summary>
        /// <param name="matrixA">Матрица количества заданий каждого типа на пакет[dataTypesCount x mi]</param>
        /// <returns>Если количество пакетов заданий по типам совпадают - true, иначе false</returns>
        public bool IsConstraint_BatchCountByType(List<List<int>> matrixA)
        {

            // Объявляем количество пакетов заданий
            int cur_batchCountByType = 0;

            // Объявляем необходимое количество пакетов заданий 
            int tar_batchCountByType = 0;

            // Выполняем обход по типам
            for (int dataType = 0; dataType < matrixA.Count; dataType++)
            {

                // Увеличиваем количество пакетов
                tar_batchCountByType = matrixA[dataType].Count;

                // Обнуляем значение количества пакето заданий заданного типа
                cur_batchCountByType = 0;

                // Для каждого пакета заданий выполняем обработку
                for (int batch = 0; batch < this.schedule.Count; batch++)

                    // Увеличиваем количество пакетов заданий текущего типа
                    cur_batchCountByType += (this.schedule[batch].Type == dataType) ? 1 : 0;

                // Выполяем проверку
                if (tar_batchCountByType != cur_batchCountByType)

                    // Возвращяем false
                    return false;
            }

            // Возвращяем true
            return true;
        }

        public override List<List<int>> GetMatrixP()
        {

            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(config.dataTypesCount);

            // Инициализируем матрицу
            for (int dataType = 0; dataType < config.dataTypesCount; dataType++)

                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(schedule.Count));

            // Заполняем результирующую матрицу
            for (int dataType = 0; dataType < config.dataTypesCount; dataType++)

                // Для каждого элемента матрицы schedule
                for (int batchIndex = 0; batchIndex < schedule.Count; batchIndex++)

                    // Заполняем элементы матрицы количества заданий в пакетах
                    res[dataType][batchIndex] = 1;

            // Возвращяем результат
            return res;
        }

        public override List<List<int>> GetMatrixR()
        {
            
            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(config.dataTypesCount);

            // Инициализируем матрицу
            for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
            
                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(schedule.Count));

            // Заполняем результирующую матрицу
            for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
            
                // Для каждого элемента матрицы schedule
                for (int batchIndex = 0; batchIndex < schedule.Count; batchIndex++)

                    // Заполняем элементы матрицы количества заданий в пакетах
                    res[dataType][batchIndex] = schedule[batchIndex].Size;

            // Возвращяем результат
            return res;
        }

        public override List<List<int>> GetMatrixTPM()
        {

            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(matrixTPM.Count);

            // Инициализируем матрицу
            for (int device = 0; device < config.deviceCount; device++) {

                // Инициализируем строки матрицы
                res.Add(new List<int>(matrixTPM[device].Count));

                // Для каждого элемента матрицы matrixTPM
                for (int batchIndex = 0; batchIndex < matrixTPM.Count; batchIndex++)

                    // Инициализируем столбцы матрицы
                    res[device].Add(matrixTPM[device][batchIndex].TimePreM);
            }

            // Возвращяем результат
            return res;
        }

        public override List<List<int>> GetMatrixY()
        {
            return matrixY;
        }

        public override Dictionary<int, List<List<int>>> GetStartProcessing()
        {
            return startProcessing;
        }
    }
}
