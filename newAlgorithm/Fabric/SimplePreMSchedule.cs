using magisterDiplom.Model;
using magisterDiplom.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Xml.XPath;

namespace magisterDiplom.Fabric
{

    /// <summary>
    /// Класс простого расписания с ПТО, наследуется от PreMSchedule
    /// </summary>
    public class SimplePreMSchedule : PreMSchedule
    {
        bool IsDebug_ShiftMatrixY = true;
        bool IsDebug_SearchByPosition = true;
        bool IsDebug_CalcSysReliability = true;
        bool IsDebug_GetPreMUtility = true;
        private void PrintMatrixY()
        {
            Console.WriteLine("MatrixY:");
            for (int device = 0; device < this.config.deviceCount; device++) {

                // Выводим разделитель
                for (int j = 0; j < this.matrixY[device].Count; j++)
                    Console.Write($"+--");
                Console.WriteLine("+");

                for (int dataType = 0; dataType < this.matrixY[device].Count(); dataType++)
                    Console.Write($"|{matrixY[device][dataType],-2}");
                Console.WriteLine("|");
            }
            // Выводим разделитель
            for (int j = 0; j < this.matrixY[0].Count; j++)
                Console.Write($"+--");
            Console.WriteLine("+");
        }

        private void PrintSchedule() {

            List<List<int>> mP = GetMatrixP();
            List<List<int>> mR = GetMatrixR();

            Console.WriteLine("[P,R]:");

            // Для каждой строки
            for (int dataType = 0; dataType < this.config.dataTypesCount; dataType++) {

                // Выводим разделитель
                for (int j = 0; j < mP[dataType].Count; j++)
                    Console.Write($"+--");
                Console.Write("+   ");

                // Выводим разделитель
                for (int j = 0; j < mR[dataType].Count; j++)
                    Console.Write($"+--");
                Console.WriteLine("+");

                // Выводим элементы матрицы P
                for (int batch = 0; batch < mP[dataType].Count; batch++)
                    Console.Write($"|{mP[dataType][batch],-2}");
                Console.Write("|   ");

                // Выводим элементы матрицы R
                for (int batch = 0; batch < mR[dataType].Count; batch++)
                    Console.Write($"|{mR[dataType][batch],-2}");
                Console.WriteLine("|");
            }

            // Выводим разделитель
            for (int j = 0; j < mP[0].Count; j++)
                Console.Write($"+--");
            Console.Write("+   ");

            // Выводим разделитель
            for (int j = 0; j < mR[0].Count; j++)
                Console.Write($"+--");
            Console.WriteLine("+");
        }

        private void PrintStartProcessing()
        {
            Console.WriteLine("Матрица моментов начала времени выполнения заданий и ПТО.");
            CalcStartProcessing();
            if (this.startProcessing.Count == 0)
                return;

            // Объявляем максимальное время
            int times = 0;

            // Вычисляем максимальное время
            for (int device = 0; device < this.config.deviceCount; device++)
                times = Math.Max(times, this.startProcessing[device].Last().Last() + this.config.proccessingTime[device, this.schedule.Last().Type]);

            int mm = 0;
            for (int device = 0; device < this.config.deviceCount; device++)
                mm = Math.Max(mm, this.config.preMaintenanceTimes[device]);
            times += mm;

            // Выводим разделитель
            {
                for (int j = 0; j < times; j++)
                    Console.Write($"{j,-4}");
                Console.WriteLine();
            }

            bool flag = false;
            int batchIndexFlag = -1;

            // Для каждого прибора
            for (int device = 0; device < this.config.deviceCount; device++)
            {

                // Выводим разделитель
                {
                    Console.Write("+");
                    for (int i = 0; i < times; i++)
                        Console.Write("---+");
                    Console.WriteLine();
                }

                // Для каждого временного отделителя
                for (int time = 0; time < times; time++)
                {

                    // Для каждого пакета заданий
                    for (int batchIndex = 0; batchIndex < this.startProcessing[device].Count(); batchIndex++) { 

                        // Для каждого задания
                        for (int job = 0; job < this.startProcessing[device][batchIndex].Count(); job++)

                            // Если момент начала времени выполнения равен текущему
                            if (this.startProcessing[device][batchIndex][job] == time)
                            {

                                // Выводим k заданий
                                for (int k = 0; k < this.config.proccessingTime[device, this.schedule[batchIndex].Type]; k++)
                                    Console.Write($"|{this.schedule[batchIndex].Type}:{this.config.proccessingTime[device, this.schedule[batchIndex].Type]}");
                                
                                if (this.startProcessing[device][batchIndex][job] == this.startProcessing[device][batchIndex].Last()) {
                                    batchIndexFlag = batchIndex;
                                    flag = true;
                                }
                                
                                // Увеличиваем время
                                time += this.config.proccessingTime[device, this.schedule[batchIndex].Type];
                            }


                        if (batchIndexFlag >= 0 && this.matrixY[device][batchIndexFlag] == 1 && flag)
                        {
                            flag = false;
                            for (int k = 0; k < this.config.preMaintenanceTimes[device]; k++)
                                Console.Write($"|***");
                            time += this.config.preMaintenanceTimes[device];
                        }
                    }
                    
                    // Если время возможно
                    if (time < times)

                        // Отрисовывем пробел
                        Console.Write($"|{' ',-3}");
                }

                // Переводим курсор
                Console.WriteLine("|");
            }

            // Выводим разделитель
            {
                Console.Write("+");
                for (int i = 0; i < times; i++)
                    Console.Write("---+");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Вернёт индекс ПЗ за которым следует последнее ПТО
        /// </summary>
        /// <param name="device">Индекс прибора</param>
        /// <returns>Индекс ПЗ за которым следует последнее ПТО или -1</returns>
        private int GetLastPreMPos(int device)
        {

            // Выполняем обход по всем ПЗ
            for (int batchIndex = this.matrixY[device].Count() - 1; batchIndex >= 0; batchIndex--)

                // Если в текущей позиции существует ПТО
                if (this.matrixY[device][batchIndex] != 0)

                    // Возвращяем индекс данной позиции
                    return batchIndex;

            // Вернём -1
            return -1;
        }

        /// <summary>
        /// Выполняем сдвиг для матрицы ПТО приборов
        /// </summary>
        /// <param name="beta">Нижний уровень нажёности</param>
        private void ShiftMatrixY(double beta) 
        {

            if (IsDebug)
                Console.WriteLine("ShiftMatrixY start: Улучшаем позиции ПТО.");

            // Объявляем индекс прибора
            int bestDevice = -1;

            // Объявляем критерий текущего лучшего расписания
            int f2;

            // Объявляем критерий f2 для текущего расписания со сдвигом
            int new_f2;

            // Объявляем индекс ПЗ за которым следует последнее ПТО
            int last_prem_batch_index;

            // Объявляем индекс последнего ПЗ для текущего расписания
            int last_batch_index;

            // Выполняем обработку в цикле
            do {

                // Обнуляем критерий f2 для текущего расписания
                f2 = 0;

                if (IsDebug && IsDebug_ShiftMatrixY) {
                    Console.WriteLine("Новая итерация сдвигов");
                    PrintStartProcessing();
                    PrintMatrixY();
                    Console.WriteLine($"f2 для текущего расписания {this.GetPreMUtility()}");
                }

                // Для каждого прибора выполняем обработку
                for (int device = 0; device < config.deviceCount; device++)
                {
                    
                    // Определяем индекс ПЗ за которым следует последнее ПТО
                    last_prem_batch_index = this.GetLastPreMPos(device); // j*

                    // Определяем индекс последнего ПЗ для текущего расписания
                    last_batch_index = this.matrixY[device].Count() - 1; // j^max

                    if (IsDebug && IsDebug_ShiftMatrixY) {
                        Console.WriteLine($"Выполняем сдвиг для прибора {device} j*={last_prem_batch_index}; j^max={last_batch_index}");
                    }

                    // Проверяем на необходимость проведения операций перестановки
                    if (last_prem_batch_index == last_batch_index) {
                        if (IsDebug && IsDebug_ShiftMatrixY)
                            Console.WriteLine($"Пропускаем сдвиг для прибора: {device}");

                        // Пропускаем итерацию для текущего прибора
                        continue;
                    }

                    // Выполняем сдвиг ПТО на следующую позицию
                    this.matrixY[device][last_prem_batch_index] = 0;
                    this.matrixY[device][last_prem_batch_index + 1] = 1;


                    // Вычисляем матрицу моментов начала времени выполнения заданий
                    this.CalcStartProcessing();

                    // Вычисляем матрицу моментов окончания времени выполнения ПТО
                    this.CalcMatrixTPM();

                    if (IsDebug && IsDebug_ShiftMatrixY)
                    {
                        PrintStartProcessing();
                        PrintMatrixY();
                    }

                    // Если текущее решение не удовлетворяет условию надёжности
                    if (!this.IsSolutionAcceptable(beta))
                    {
                        if (IsDebug && IsDebug_ShiftMatrixY)
                            Console.WriteLine("РЕШЕНИЕ НЕ ДОПУСТИМО");
                        
                        // Выполняем обратный сдвиг ПТО
                        this.matrixY[device][last_prem_batch_index] = 1;
                        this.matrixY[device][last_prem_batch_index + 1] = 0;

                        // Пропускаем итерацию
                        continue;
                    }

                    // Вычисляем критерий f2 для текущего расписания со сдвигом
                    new_f2 = this.GetPreMUtility();
                    if (IsDebug && IsDebug_ShiftMatrixY)
                        Console.WriteLine($"РЕШЕНИЕ ДОПУСТИМО. G(f2)={f2};new_f2={new_f2}");

                    // Если текущее расписания лучше предыдущего
                    if (new_f2 > f2)
                    {

                        // Запоминаем новый лучший критерий f2
                        f2 = new_f2;

                        // Переопределяем индекс прибора
                        bestDevice = device;
                    }

                    // Выполняем обратный сдвиг ПТО
                    this.matrixY[device][last_prem_batch_index] = 1;
                    this.matrixY[device][last_prem_batch_index + 1] = 0;
                }

                if (IsDebug && IsDebug_ShiftMatrixY)
                    Console.WriteLine($"{Environment.NewLine}f2={f2}");

                // Если улучшений позиций ПТО не было найдено
                if (f2 == 0)
                {
                    if (IsDebug && IsDebug_ShiftMatrixY)
                        Console.WriteLine("Улучшений не было найдено");

                    // Прекращаем обработку
                    break;
                }

                if (IsDebug && IsDebug_ShiftMatrixY)
                    Console.WriteLine("Было найдено улучшение.");

                // Определяем индекс ПЗ за которым следует последнее ПТО
                last_prem_batch_index = this.GetLastPreMPos(bestDevice); // j*

                // Выполняем их переопределение
                this.matrixY[bestDevice][last_prem_batch_index] = 0;
                this.matrixY[bestDevice][last_prem_batch_index + 1] = 1;

                if (IsDebug && IsDebug_ShiftMatrixY) {
                    Console.WriteLine("Новое решение:");
                    PrintStartProcessing();
                    PrintMatrixY();
                }

                // Продолжаем улучшения
            } while (true);

            if (IsDebug && IsDebug_ShiftMatrixY)
            {
                Console.WriteLine("Было найдено решение с помощью сдвигов.");
                Console.WriteLine("Выполняется заполнение позиций ПТО не найденных с помощью сдвигов.");
            }

            // Для каждого прибора выполняем дополнение для матрицы ПТО 1
            for (int device = 0; device < this.config.deviceCount; device++)
            {
                if (IsDebug && IsDebug_ShiftMatrixY) {
                    Console.Write($"Для прибора: {device};");
                }

                // Определяем индекс ПЗ за которым следует последнее ПТО
                last_prem_batch_index = this.GetLastPreMPos(device); // j*

                // Определяем индекс последнего ПЗ для текущего расписания
                last_batch_index = this.matrixY[device].Count() - 1; // j^max

                // Если матрица Y не оканчивается 1
                if (last_prem_batch_index < last_batch_index) {

                    if (IsDebug && IsDebug_ShiftMatrixY)
                        Console.WriteLine($"ПТО добавляется.");

                    // Изменяем индекс последнего ПТО нп 1
                    this.matrixY[device][last_batch_index] = 1;
                }
                else if (IsDebug && IsDebug_ShiftMatrixY)
                    Console.WriteLine($"ПТО не добавляется.");
            }

            if (IsDebug && IsDebug_ShiftMatrixY)
            {
                PrintStartProcessing();
                PrintMatrixY();
                Console.WriteLine("ShiftMatrixY stop.");
            }
        }

        /// <summary>
        /// Данная функция выполняет локальную оптимизацию составов ПЗ
        /// </summary>
        /// <param name="swapCount">Количество перестановок</param>
        /// <param name="beta">Нижний порог надёжности</param>
        /// <returns>true, если была найдено перестановка удовлетворяющая условию надёжности. Иначе false</returns>
        private bool SearchByPosition(double beta, int swapCount = 999999)
        {

            // Выводим отладачную информацию
            if (IsDebug)
                Console.WriteLine($"SearchByPosition start: Изменяем позиции пакета заданий. beta:{beta}; swapCount:{swapCount}");

            // Флаг перестановки выполняющей условию надёжности
            bool isFind = false;

            // Объявляем лучшее расписание
            List<Model.Batch> bestSchedule = new List<Model.Batch>(this.schedule);
            
            // Объявляем значение наилучшего критерия f2
            int bestTime = 2000000000;

            // Объявляем значение текущего критерия f2
            int newTime;

            // Вычисляем матрицу моментов времени начала выполнения заданий
            this.CalcStartProcessing();

            // Вычисляем матрицу моментов окончания ПТО приборов
            this.CalcMatrixTPM();

            // Выводим отладачную информацию
            if (IsDebug && IsDebug_SearchByPosition)
            {
                Console.WriteLine("Начальное расписание:");
                PrintSchedule(); PrintStartProcessing();
            }

            // Проверяем допустимость текущего решения
            if (this.IsSolutionAcceptable(beta)) {

                // Устанавливаем флаг перестановки выполняющей условию надёжности
                isFind = true;

                // Получаем f2 критерий - момента времени окончания последнего задания
                bestTime = this.GetPreMUtility();

                // Устанавливаем лучшее расписание
                bestSchedule = new List<Batch>(this.schedule);

                if (IsDebug && IsDebug_SearchByPosition)
                {
                    Console.WriteLine("Начальное расписание допустимо");
                    Console.WriteLine($"f2 для текущего расписания:{bestTime}");
                }
            } else if (IsDebug && IsDebug_SearchByPosition)
            {
                Console.WriteLine("Начальное расписание не допустимо");
            }

            // Выполняем заявленное количество перестановок, заявленно количество раз
            for (int batchIndex = schedule.Count - 1; batchIndex > 0 && swapCount > 0; batchIndex--, swapCount--)
            {

                // Выполняем перестановку
                Batch batch = this.schedule[batchIndex];
                this.schedule[batchIndex] = this.schedule[batchIndex - 1];
                this.schedule[batchIndex - 1] = batch;

                if (IsDebug && IsDebug_SearchByPosition)
                {
                    Console.WriteLine($"Выполняем перестановку {batchIndex} и {batchIndex - 1}");
                    PrintSchedule();
                }

                // Вычисляем матрицу моментов времени начала выполнения заданий
                this.CalcStartProcessing();

                // Вычисляем матрицу моментов окончания времени ПТО приборов
                this.CalcMatrixTPM();

                // Проверяем допустимость текущего решения
                if (this.IsSolutionAcceptable(beta))
                {

                    // Устанавливаем флаг перестановки выполняющей условию надёжности
                    isFind = true;

                    // Высчитываем новый критерий makespan
                    newTime = this.GetPreMUtility();

                    if (IsDebug && IsDebug_SearchByPosition)
                    {
                        Console.WriteLine("Текущее расписание допустимо");
                        Console.WriteLine($"new_f2 для текущего расписания:{newTime}");
                    }

                    // Если новое время лучше, то выполняем переопределение
                    if (newTime > bestTime)
                    {

                        if (IsDebug && IsDebug_SearchByPosition)
                        {
                            Console.WriteLine($"Текущее расписание лучше предыдущего. ({newTime} > {bestTime}).");
                        }

                        // TODO: Избавиться от копирования списка в пользу использования индекса наилучшей позиции
                        // Переопределяем лучшее расписание
                        bestSchedule = new List<Batch>(schedule);

                        // Переопределяем лучшее время для лучшего расписания
                        bestTime = newTime;
                    }
                }
                else if (IsDebug && IsDebug_SearchByPosition)
                {
                    Console.WriteLine("Текущее расписание не допустимо");
                }
            }

            // Выполняем переопределение наилучшего раысписания составов ПЗ
            this.schedule = bestSchedule;

            // Выводим отладачную информацию
            if (IsDebug && IsDebug_SearchByPosition && isFind)
            {
                Console.WriteLine("Извлекаем лучшее расписание");
                PrintSchedule();
            }

            // Возвращяем результат
            return isFind;
        }

        /// <summary>
        /// Функция проверяет допустимость решения
        /// </summary>
        /// <param name="beta">Нижний уровень надёжности</param>
        /// <returns>true - если текущее решение допустимо. Иначе False</returns>
        private bool IsSolutionAcceptable(double beta)
        {

            // Выводим отладачную информацию
            if (IsDebug)
                Console.WriteLine($"IsSolutionAcceptable start: Проверяем допустимость решения. beta:{beta}");
                
            // Для каджого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            
                // Для каждого ПЗ в расписании выполняем обработку
                for (int batch = 0; batch < this.schedule.Count; batch++)

                    // Если для данной позиции существует ПТО
                    if (this.matrixY[device][batch] != 0)
                    {

                        // Вычисляем момент времени окончания ПТО на текущем приборе в текущей позиции
                        int time =

                            // Момент времени начала выполнения последнего задания на текущем приборе в текущей позиции
                            this.startProcessing[device][batch].Last() +

                            // Время выполнения задания на текущем приборе в текущей позиции 
                            this.config.proccessingTime[device, this.schedule[batch].Type];

                        CalcMatrixTPM();

                        // Проверяем ограничение надёжности
                        if (!IsConstraint_CalcSysReliability(time, beta)) {

                            // Выводим отладачную информацию
                            if (IsDebug && IsDebug_CalcSysReliability)
                                Console.WriteLine($"Ограничение не выполняется{Environment.NewLine}");

                            // Если ограничение не выполняется, вернуть false
                            return false;
                        }

                        // Выводим отладачную информацию
                        if (IsDebug && IsDebug_CalcSysReliability)
                            Console.WriteLine($"Ограничение выполняется{Environment.NewLine}");
                    }
            
            // Все ограничения выпоняются, вернуть true
            return true;
        }

        /// <summary>
        /// Конструктор выполняющий создание экземпляра данного класса 
        /// </summary>
        public SimplePreMSchedule(Config config) {
            this.config = config;

            // Если флаг оталдки установлен
            if (IsDebug) {

                // Выводим информацию о переданной конфигурационной структуре
                Console.WriteLine(config.ToString());
            }

            startProcessing = new Dictionary<int, List<List<int>>>();
            matrixTPM = new List<List<PreMSet>>();
        }

        /// <summary>
        /// Выполняет построение расписания
        /// </summary>
        /// <param name="matrixA">Матрица составов пакетов заданий</param>
        public double Build(List<List<int>> matrixA, double beta = 0.9)
        {

            // Если флаг оталдки установлен
            if (IsDebug)
            {
                Console.WriteLine("matrixA:");
                for (int _dataType = 0; _dataType < matrixA.Count(); _dataType++) {
                    Console.Write($"\t{_dataType}: ");
                    for (int _batchIndex = 0; _batchIndex < matrixA[_dataType].Count(); _batchIndex++)
                        Console.Write($"{matrixA[_dataType][_batchIndex]} ");
                    Console.Write(Environment.NewLine);
                }
            }

            // Объявляем тип данных
            int dataType;

            // Объявляем максимальное количество пакетов
            int maxBatchCount = 0;
            
            // Объявляем ПЗ
            int batch = 0;

            // Вычисляем максимальное количество пакетов среди всех типов данных
            calcMaxBatchCount();

            // Если флаг оталдки установлен
            if (IsDebug)

                // Выводим максимальное количество пакетов среди всех типов данных
                Console.WriteLine($"maxBatchCount: {maxBatchCount}");

            // Вернёт максимальное количество пакетов среди всех типов данных
            void calcMaxBatchCount()
            {
                // Выполняем обработку по типам
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                    // Выполняем поиск максимального количество пакетов
                    maxBatchCount = Math.Max(maxBatchCount, matrixA[dataType].Count);
            }

            Dictionary<int, double> m = new Dictionary<int, double>(capacity: this.config.dataTypesCount);
            List<int> dataTypes = new List<int>(capacity: this.config.dataTypesCount);
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++) {
                double sum = 0;
                for (int device = 1; device < this.config.deviceCount; device++)
                    sum += 
                        (double) this.config.proccessingTime[device, dataType] /
                        (double) this.config.proccessingTime[device - 1, dataType];
                m.Add(dataType, sum);
            }

            // Если флаг оталдки установлен
            if (IsDebug)

                // Выводим информацию
                Console.WriteLine("dataTypes:");

            while (m.Any())
            {
                int myDataType = m.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                // Если флаг оталдки установлен
                if (IsDebug)

                    // Выводим информацию
                    Console.WriteLine($"\t{myDataType}: {m[myDataType]}");

                dataTypes.Add(myDataType);
                m.Remove(myDataType);
            }


            // Если флаг оталдки установлен
            if (IsDebug) {

                // Выводим информацию
                Console.WriteLine("dataTypes:");

                // Для каждого типа
                for (int _dataType = 0; _dataType < this.config.dataTypesCount; _dataType++)

                    // Выводим информацию
                    Console.WriteLine($"\t{_dataType}: {dataTypes[_dataType]}");
            }

            // Сортируем матрицу A
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                matrixA[dataType].Sort();

            // Если флаг оталдки установлен
            if (IsDebug)
            {
                Console.WriteLine("matrixA':");
                for (int _dataType = 0; _dataType < matrixA.Count(); _dataType++)
                {
                    Console.Write($"\t{_dataType}: ");
                    for (int _batchIndex = 0; _batchIndex < matrixA[_dataType].Count(); _batchIndex++)
                        Console.Write($"{matrixA[_dataType][_batchIndex]} ");
                    Console.Write(Environment.NewLine);
                }
            }

            batch = 0;
            dataType = 0;

            // П.2 Добавляем 
            this.schedule = new List<Batch>();
            this.schedule.Add(new Batch(
                dataTypes[dataType],
                matrixA[dataTypes[dataType]][batch]
            ));
            dataType++;

            if (IsDebug)
                PrintSchedule();

            // П.3 Инициализируем матрицу Y
            this.matrixY = new List<List<int>>(capacity: this.config.deviceCount);
            for (int device = 0; device < this.config.deviceCount; device++) {
                this.matrixY.Add(new List<int>());
                this.matrixY[device].Add(1);
            }
            if (IsDebug)
            {
                PrintMatrixY();
            }

            // Для каждого типа данных выполняем обрабоку
            for (; dataType < this.config.dataTypesCount; dataType++)
            {
                
                // Добавляем ПЗ в расписание 
                this.schedule.Add(new Batch(dataTypes[dataType], matrixA[dataTypes[dataType]][batch]));
                for (int device = 0; device < this.config.deviceCount; device++)
                    this.matrixY[device].Add(0);

                if (IsDebug) {
                    PrintStartProcessing();
                }

                // Если не было найдено расписания удовлетворяющему условию надёжности
                if (!this.SearchByPosition(beta, 5))

                    // Возвращяем 0, как флаг неудачи
                    return 0.0;

                // Выполняем оптимизацию для позиций ПТО приборов
                this.ShiftMatrixY(beta);
                
                // Проверяем условие надёжности
                // TODO: СПОРНО (НЕВОЗМОЖНОАЯ СИТУАЦИЯ) - ПОДСЧИТАТЬ КОЛИЧЕСТВО ВЫЗОВОВ 
                if (!this.IsSolutionAcceptable(beta))
                {
                    Console.WriteLine("Тот самый случай");
                    // Возвращяем 0, как флаг неудачи
                    return 0.0;
                }
            }

            // Увеличиваем индекс вставляемого пакета задания
            batch++;

            // Выполняем обработку
            while (batch < maxBatchCount)
            {

                // Выполняем обработку для каждого типа данных
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                {

                    // Если индекс пакета превышает максимальный размер пакетов для типа данных dataType
                    if (batch >= matrixA[dataType].Count)

                        // Продолжаем обработку для следующего типа данных
                        continue;

                    // Добавляем ПЗ в расписание 
                    this.schedule.Add(new Batch(dataTypes[dataType], matrixA[dataTypes[dataType]][batch]));
                    for (int device = 0; device < this.config.deviceCount; device++)
                        this.matrixY[device].Add(0);

                    // Если не было найдено расписания удовлетворяющему условию надёжности
                    if (!this.SearchByPosition(beta, 5))
                    
                        // Возвращяем 0, как флаг неудачи
                        return 0.0;

                    // Выполняем оптимизацию для позиций ПТО приборов (ШАГ 15)
                    this.ShiftMatrixY(beta);

                    // Проверяем условие надёжности
                    // TODO: СПОРНО (НЕВОЗМОЖНОАЯ СИТУАЦИЯ) - ПОДСЧИТАТЬ КОЛИЧЕСТВО ВЫЗОВОВ 
                    if (!this.IsSolutionAcceptable(beta))
                    {
                        Console.WriteLine("Тот самый случай");
                        // Возвращяем 0, как флаг неудачи
                        return 0.0;
                    }
                }

                // Увеличиваем индекс пакета
                batch++;
            }

            
            return this.GetPreMUtility();
        }

        /// <summary>
        /// Выполняет построение матрицы моментов окончания времени выполнения ПТО.
        /// </summary>
        public void CalcMatrixTPM()
        {
            matrixTPM.Clear();

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

            // Выводим отладочную информацию
            if (IsDebug) {
                PrintStartProcessing();
                Console.WriteLine("GetPreMUtility start: Вычисляем сумму полезности и итервалов между ПТО");
            }
            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Выводим отладочную информацию
                if (IsDebug && IsDebug_GetPreMUtility) {
                    Console.WriteLine($"device:{device}");
                    Console.WriteLine($"\tМомент окончания последнего задания { this.startProcessing[device].Last().Last() + this.config.proccessingTime[device, this.schedule.Last().Type] }");
                }

                // Добавляем момент времени окончания всех заданий на приборе
                sum +=
                    
                    // Момент начала времени выполнения на последнем задании в последнем пакете
                    this.startProcessing[device].Last().Last() +

                    // Время выполнения последнего заданий в последенем пакете
                    this.config.proccessingTime[device, this.schedule.Last().Type];

                // Выводим отладочную информацию
                if (IsDebug && IsDebug_GetPreMUtility)
                    Console.WriteLine($"\tПростои для данного прибора с учётом ПТО { this.GetDowntimeByDevice(device) }");

                // Вычитаем простои
                sum -= this.GetDowntimeByDevice(device);

                int intervals = 0;
                intervals += matrixTPM[device].First().TimePreM;

                // Для каждого пакета выполняем обработку
                for (int batchIndex = 1; batchIndex < matrixTPM[device].Count(); batchIndex++)

                    // Добавляем интервалы времени между ПТО разных пакетов
                    intervals += matrixTPM[device][batchIndex].TimePreM - matrixTPM[device][batchIndex - 1].TimePreM;

                // Выводим отладочную информацию
                if (IsDebug && IsDebug_GetPreMUtility)
                    Console.WriteLine($"\tИнтервалы времени между ПТО { intervals }");
                
                // Выполняем подсчёт суммы интервалов времени на первом пакете ПТО
                sum += intervals;
            }

            // Выводим отладочную информацию
            if (IsDebug && IsDebug_GetPreMUtility)
                Console.WriteLine($"Критерий f2 {sum}{Environment.NewLine}");

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

        // Выражение 12
        /// <summary>
        /// Функция вернёт время активности прибора от предыдущего ПТО
        /// </summary>
        /// <param name="device">Индекс прибора для которого расчитывается время активности</param>
        /// <param name="time">Крайний момент времени</param>
        /// <returns>Время активности</returns>
        private int GetActivityTimeByDevice(int device, int time)
        {
            
            // Определяем начальный индекс
            int batchIndex = GetBatchIndex(device, time) + 1;

            // Определяем время активности
            int activityTime = 0;

            // Для каждого пакет выполняем обработку
            for (; batchIndex < schedule.Count; batchIndex++)

                // Для каждого задания выполняем обработку
                for (int job = 0; job < startProcessing[device][batchIndex].Count; job++)
                {

                    // Если момент начала времени выполнения выходит за границу
                    if (startProcessing[device][batchIndex][job] >= time)

                        // Вернём время активности
                        return activityTime;

                    // Высчитываем время выполнения
                    int proc_time = this.config.proccessingTime[device, this.schedule[batchIndex].Type];

                    // Высчитываем момент начала времени выполнения
                    int start_time = this.startProcessing[device][batchIndex][job];

                    // Если момент окончания задания выходит за указанные границы
                    if (start_time + proc_time > time)
                    {
                        // Увеличиваем время активности до прибора
                        activityTime += time - start_time;

                        // Возвращяем время активности
                        return activityTime;
                    }

                    // Увеличиваем время активности прибора
                    activityTime += proc_time;
                }

            // Возвращяем время активности
            return activityTime;
        }

        // ВЫРАЖЕНИЕ 12
        /// <summary>
        /// Возвращяет время активности для конкретного прибора
        /// </summary>
        /// <param name="device">Прибор для которого расчитывается время активности</param>
        /// <param name="time">Крайний момент времени</param>
        /// <returns>Время активности</returns>
        // private int GetActivityTimeByDevice(int device, int time)
        // {
        // 
        //     // Определяем начальный индекс
        //     int startIndex = GetBatchIndex(device, time) + 1;
        //     if (IsDebug)
        //     {
        //         Console.WriteLine($"device:{device}; startIndex:{startIndex}");
        //     }
        // 
        //     // Определяем индекс ПЗ
        //     int batchIndex = startIndex;
        //     
        //     // Определяем количество ПЗ
        //     int batchCount = 0;
        //     
        //     // Определяем количество заданий
        //     int jobCount = 0;
        //     
        //     // Определяем время активности
        //     int activityTime = 0;
        //     
        //     // Для каждого пакет выполняем обработку
        //     for (; batchIndex < schedule.Count; batchIndex++)
        //     {
        //     
        //         // Если первое задани в ПЗ удовлетворяет условию
        //         if (startProcessing[device][batchIndex][0] >= time)
        //     
        //             // Прекращаем обарботку
        //             break;
        //     
        //         // Увеличиваем количество ПЗ
        //         batchCount++;
        //     
        //         // Увеличиваем активность
        //         activityTime += schedule[batchIndex].Size * config.proccessingTime[device, batchIndex];
        //     }
        //     
        //     // Если количество ПЗ равно 0
        //     if (batchCount == 0)
        //     
        //         // Добавляем информацию
        //         return 0;
        //     
        //     // Для каждого задания в последнем пакете выполняем обработку
        //     for (int job = 0; job < schedule[startIndex + batchCount - 1].Size; job++)
        //     {
        //     
        //         // Если некоторое задание в последенем ПЗ не удовлетворяет условию
        //         if (startProcessing[device][startIndex + batchCount - 1][job] >= time)
        //     
        //             // Прекращаем обарботку
        //             break;
        //     
        //         // Увеличиваем количество заданий
        //         jobCount++;
        //     }
        //     
        //     // Уменьшаем время
        //     activityTime -= (schedule[startIndex + batchCount - 1].Size - jobCount) * config.proccessingTime[device, startIndex + batchCount - 1];
        // 
        //     // Добавляем информацию
        //     return activityTime;
        // }

        // ВЫРАЖЕНИЕ 13
        
        /// <summary>
        /// Возвращяет доступность для всех приборов для указанного момента времени
        /// </summary>
        /// <param name="time">Момент времени для которого выполняется расчёт надёжности</param>
        /// <returns>Доступность для всех приборов</returns>
        public double CalcSysReliability(int time)
        {
            if (IsDebug && IsDebug_CalcSysReliability)
            
                Console.WriteLine($"Вычисляем системную надёжность для момента времени {time}");
            
            // Объявляем надёжность
            double reliability = 1;

            // Объявляем время активности
            int activity_time = 0;

            // Для каждого прибора подсчитываем надёжность
            for (int device = 0; device < config.deviceCount; device++) {

                // Вычисляем время активности
                activity_time = this.GetActivityTimeByDevice(device, time);

                if (IsDebug && IsDebug_CalcSysReliability)

                    Console.WriteLine($"\tДля прибора {device} время активности {activity_time} и надёжность {this.CalcReliabilityByDevice(activity_time, device):0.000}");
                
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
            double sysTime = this.CalcSysReliability(time);
            if (IsDebug && IsDebug_CalcSysReliability)
                Console.WriteLine($"Системная надёжность {sysTime:0.000} >= {beta:0.000}?");
            return (sysTime >= beta);
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
            // TODO: Баг, определяет пакет по окончанию ПТО, когда необходимо по началу ПТО.
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
            for (int dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(this.schedule.Count));

            // Для каждого элемента матрицы schedule
            for (int batchIndex = 0; batchIndex < this.schedule.Count; batchIndex++)

                // Заполняем элементы матрицы количества заданий в пакетах
                res[this.schedule[batchIndex].Type][batchIndex] = 1;

            // Возвращяем результат
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
