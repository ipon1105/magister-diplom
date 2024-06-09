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
            for (int device = 0; device < this.config.deviceCount; device++){
                this.startProcessing.Add(device, new List<List<int>>(this.schedule.Count()));
                for (int batchIndex = 0; batchIndex < this.schedule.Count(); batchIndex++)
                    this.startProcessing[device].Add(ListUtils.InitVectorInt(this.schedule[batchIndex].Size));
            }

            // Выполняем построение матрицы времён начала заданий
            CalculationService.CalculateTnMatrix(
                this.GetMatrixR(),
                this.GetMatrixP(),
                this.config.proccessingTime,
                this.config.changeoverTime,
                this.config.buffer,
                this.config.deviceCount,
                ref this.startProcessing
            );
        }

        public override int GetMakespan()
        {
            return this.startProcessing[this.config.deviceCount - 1].Last().Last();
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
            return startProcessing;
        }
    }
}
