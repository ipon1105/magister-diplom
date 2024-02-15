using magisterDiplom.Model;
using magisterDiplom.Utils;
using newAlgorithm.Model;
using newAlgorithm.Service;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.PerformanceData;
using System.Linq;

namespace newAlgorithm
{
    class Shedule
    {

        /// <summary>
        /// Данная структура данных содержит информацию о конфигурации конвейерной системы
        /// Данная переменная определяет длину конвейера, как количество приборов
        /// TODO: Убрать статическое определение количества приборов, с целью масштабируемости
        /// NOTE: Данная переменная присваивается только из файлов Form1.cs и Shedule.cs 
        /// </summary>
        public static int deviceCount;

        /// <summary>
        /// Данный двумерный список представляет из себя матрицу выполнения задания на приборе l типа i
        /// TODO: Убрать статическое определение матрицы выполнения задания, с целью масштабируемости
        /// </summary>
        public static List<List<int>> proccessingTime;

        /// <summary>
        /// Данный трёхмерный список представляет из себя матрицу переналадки прибора l с задания i на задание j
        /// TODO: Убрать статическое определение матрицы переналадки приборов, с целью масштабируемости
        /// TODO: Переопределить трёхмерный матрица, как словарь с двумерными матрицами
        /// </summary>
        public static List<List<List<int>>> changeoverTime;

        /// <summary>
        /// Матрица количества данных i-ых типов в партиях,
        /// занимающих в последовательностях pi_l j-ые позиции
        /// Данная матрица имеет представление, где в строке 
        /// (строка есть позиция) все элементы 0 (столбец есть тип данных),
        /// кроме одно например для матрицы 3x3:
        /// [ [3,0,0],
        ///   [0,3,0],
        ///   [0,0,3],]
        /// TODO: Данная реализация подразумеваем перевернутую матрицу R, где количество строк, это возможные позиции в последовательность, а количество столбцов, это типы данных, необходимо будет исправить данную оплошность
        /// </summary>
        private List<List<int>> matrixR;

        /// <summary>
        /// Данная переменная определяет время выполнения пакета в последней позиции на последнем устройстве
        /// </summary>
        private int timeOfLastScheduleExecution;

        /// <summary>
        /// Данный словарь определяет для каждого прибора собственную матрицу начала времени обработки. Каждая строка матрицы представляет из себя вектор заданий.
        /// </summary>
        private Dictionary<int, List<List<int>>> startProcessing; // [deviceCount] : [maxBatchesPositions x jobCount]

        /// <summary>
        /// Данный словарь определяет для каждого прибора собственную матрицу конца времени обработки. Каждая строка матрицы представляет из себя вектор длиной в количество заданий и каждый элемент вектора это время конца обработки.
        /// </summary>
        private Dictionary<int, List<List<int>>> stopProcessing; // [deviceCount] : [maxBatchesPositions x jobCount]

        /// <summary>
        /// Экземпляр класса визуализации для отображения в Excel
        /// </summary>
        public static Visualizer viz;

        /// <summary>
        /// Представляем матрицу R в виде списка классов SheduleElement
        /// </summary>
        private List<SheduleElement> matrixRWithTime;

        /// <summary>
        /// Данный конструктор вовзращает экземпляр данного класса на основе переданного параметра матрицы R
        /// </summary>
        /// <param name="matrixR">Матрица R - количества данных i-ых типов в партиях занимающих в последовательности pi_l j-е позиции</param>
        /// <param name="deviceCount">Количество приборов в конвейерной системе</param>
        public Shedule(List<List<int>> matrixR, int deviceCount)
        {

            // Проверяем инициализацю матрицы
            if (this.matrixR != null)

                // Если матрицу уже существует - отчищаем её
                this.matrixR.Clear();

            // Выполняем переопределение новой матрицы
            this.matrixR = matrixR;

            // Определяем новое количество приборов
            Shedule.deviceCount = deviceCount;

            // Инициализируем экземпляр класс для визуализации
            if (Form1.vizualizationOn)
                viz = new Visualizer(deviceCount, matrixR[0].Count);
        }

        /// <summary>
        /// Данный конструктор вовзращает экземпляр данного класса на основе переданной матрицы A
        /// </summary>
        /// <param name="matrixA">Матрица составов партий - матрица А</param>
        public Shedule(List<List<int>> matrixA)
        {
            InitMatrixR(matrixA);

            // Инициализируем экземпляр класс для визуализации
            if (Form1.vizualizationOn)
                viz = new Visualizer(deviceCount, matrixR[0].Count);
        }

        /// <summary>
        /// Данная функция инициализируем матрицу R с помощью переданной в неё матрицы A
        /// </summary>
        /// <param name="matrixA">Матрица составов партий - матрица A </param>
        private void InitMatrixR(IReadOnlyList<List<int>> matrixA)
        {

            // Отчищаем существующие матрицы R, если они есть
            if (matrixR != null)
                matrixR.Clear();

            // Подсчитываем количество элементов в каждой строке (sum(mi))
            // batchesList представляет из себя множество партий с одним типом
            // например, скажем для типа 1, это может быть [10, 2]. Тогда 
            // batchesList.Count = mi = 2 - Количество партий данных 1-ого типа
            // Тогда сумма всех количеств партий (batchesForAllDataTypes, так же
            // известное, как n_p), представляет из себя количество возможных позиций
            // партий в последовательности pi_l
            var maxBatchesPositions = matrixA.Sum(batchesList => batchesList.Count);

            // Объявляем максимальный количество партий (max(mi))
            var maxBatchSize = 0;

            // Высчитываем максимальный размер колонки, 
            foreach (var mi in matrixA)
                maxBatchSize = (mi.Count > maxBatchSize) ? mi.Count : maxBatchSize;

            // Создаём матрицу из 0, как [maxBatchesPositions x dataTypesCount] = [n_p x n]
            matrixR = ListUtils.InitMatrixInt(maxBatchesPositions, matrixA.Count, 0);
            
            // Данная переменная описывает позицию в последовательности pi_l
            var batchIndex = 0;

            // Пробегаемся по всем возможным пакетам - колонкам(max(mi)) матрицы R
            for (var batchSize = 0; batchSize < maxBatchSize; batchSize++)

                // Пробегаемся по всем типам данных матрицы A
                for (var dataType = 0; dataType < matrixA.Count; dataType++)

                    // Если количество партий данных i-ого типа (mi) > (h) индекс пакета
                    if (matrixA[dataType].Count > batchSize)

                        // Выполяем присвоение
                        matrixR[batchIndex++][dataType] = matrixA[dataType][batchSize];
            
        }

        /// <summary>
        /// Выполяем построение матрицы R со временем окончания заданий
        /// </summary>
        public void BuildMatrixRWithTime()
        {

            // Если матрица уже существует, отчищаем её
            if (matrixRWithTime != null)
                matrixRWithTime.Clear();

            // Инициализируем матрицу R со временем
            matrixRWithTime = new List<SheduleElement>();

            // Для последнего прибора вычисляем матрицу R со временем
            for (int batchIndex = 0; batchIndex < stopProcessing[stopProcessing.Count - 1].Count; batchIndex++)
            {

                // Получаем тип данных для матрицы R
                var dataType = ReturnRDataType(batchIndex);
                var jobCount = matrixR[batchIndex][dataType];
                var stopProcessingVector = stopProcessing[stopProcessing.Count - 1][batchIndex];

                // Для каждой позиции создаём элемент расписания
                SheduleElement element = new SheduleElement(
                    jobCount,
                    dataType,
                    stopProcessingVector
                );

                // Добавляем элемент в список
                matrixRWithTime.Add(element);
            }
        }

        /// <summary>
        /// Возвращаем список из элементов ScheduleElement
        /// </summary>
        /// <returns>Список из элементов ScheduleElement</returns>
        public List<SheduleElement> ReturnMatrixRWithTime()
        {

            // Если матрица не существует, вызываем функцию построения матрицы
            if (matrixRWithTime == null)
                BuildMatrixRWithTime();

            // Возвращаем список матрицы R с временем
            return matrixRWithTime;
        }

        /// <summary>
        /// Данная функция выполняет инициализацию startProcessing и stopProcessing в зависимости от матрицы matrixR
        /// </summary>
        private void CalculateShedule()
        {

            // Проверка на инициализацию словаря
            if (startProcessing != null)
                startProcessing.Clear();
            
            // Проверка на инициализацию словаря
            if (stopProcessing != null)
                stopProcessing.Clear();

            // Выполняем инициализацию словарей
            startProcessing = new Dictionary<int, List<List<int>>>();
            stopProcessing = new Dictionary<int, List<List<int>>>();

            // Для каждого прибора выполняем перебор
            for (var device = 0; device < deviceCount; device++) { 
                List<List<int>> startProccessingMatrix = new List<List<int>>();
                List<List<int>> stopProccessingMatrix = new List<List<int>>();

                // Для каждой возможной позиции выполняем обработку
                for (var batchIndex = 0; batchIndex < matrixR.Count; batchIndex++)//количество партий
                {

                    // Получаем количество заданий в позиции batchIndex
                    var jobCount = ReturnRJobCount(batchIndex);
                    if (jobCount == -1)
                        jobCount = 1;

                    // Для временных матриц выполняем инициализацию 
                    startProccessingMatrix.Add(ListUtils.InitVectorInt(jobCount, 0));
                    stopProccessingMatrix.Add(ListUtils.InitVectorInt(jobCount, 0));
                }

                // Добавляем матрицы в словари
                startProcessing.Add(device, startProccessingMatrix);
                stopProcessing.Add(device, stopProccessingMatrix);
            }

            var previousBatchIndex = 0;
            var previousJob = 0;
            var previousDataType = 0;

            // Для каждого прибора выполняем высчитывание времён конца и начала обработки партий
            for (var device = 0; device < deviceCount; device++)
            {

                // Для каждой возможной позиции выполняем обработку
                for (var batchIndex = 0; batchIndex < matrixR.Count; batchIndex++)
                {

                    // Получаем индекс ненулевого элемента в строке
                    var dataType = ReturnRDataType(batchIndex);

                    // Перебираем все задания в партии
                    for (var job = 0; job < matrixR[batchIndex][dataType]; job++)
                    {

                        // Для каждого требования высчитываем время переналадки
                        var timeToSwitch = (dataType == previousDataType && batchIndex != 0) ? 0 : changeoverTime[0][previousDataType][dataType];

                        // Выполяем высчитывания времён начала обработки пакета batch на приборе device в позиции position
                        if (device > 0)

                            // Время начала выполнения партии batch на приборе device в позиции position
                            // высчитываем, как время конца выполнения предыдущей партии previousBatch на приборе device в предыдущей позиции 
                            //                + время переналадки на выполнение партии данного типа, которое может быть равно 0 или
                            // как время конца выполнения данной партии batch на предыдущем приборе device - 1 в текущей позиции position
                            startProcessing[device][batchIndex][job] = Math.Max(stopProcessing[device][previousBatchIndex][previousJob] + timeToSwitch, stopProcessing[device - 1][batchIndex][job]);
                        
                        else

                            // Время начала выполнения партии batch на приборе device в позиции position
                            // высчитываем, как время конца выполнения предыдущей партии previousBatch на приборе device в предыдущей позиции 
                            //                + время переналадки на выполнение партии данного типа, которое может быть равно 0.
                            startProcessing[device][batchIndex][job] = stopProcessing[device][previousBatchIndex][previousJob] + timeToSwitch;

                        stopProcessing[device][batchIndex][job] = startProcessing[device][batchIndex][job] + proccessingTime[device][dataType];

                        // Определяем данную переменную, как время выполнения пакета в последней позиции на последнем устройстве
                        timeOfLastScheduleExecution = stopProcessing[device][batchIndex][job];
                        previousBatchIndex = batchIndex;
                        previousJob = job;
                        previousDataType = dataType;
                    }
                }

                previousBatchIndex = 0;
                previousJob = 0;
                previousDataType = 0;
            }
        }

        /// <summary>
        /// Данная функция выполняет высчитывание расписания с буфером
        /// </summary>
        /// <param name="bufferSize">Размер буфера</param>
        /// <param name="dataTypesCount">Количество типов данных</param>
        private void CalculateSheduleWithBufer(int bufferSize, int dataTypesCount)
        {

            // Инициализируем матрицу времени выполнения заданий
            Matrix proccessingTimeMatrix = new Matrix(proccessingTime);

            // Инициализируем матрицу R - количества данных i-ых типов в партиях занимающих в последовательноси pi_l j-е позиции
            RMatrix rMatrix = new RMatrix(dataTypesCount);

            // Для каждого позиции в последовательности выполняем перебор
            for (int batchIndex = 0; batchIndex < matrixR.Count; batchIndex++)
            {
                int _dataType = ReturnRDataType(batchIndex);
                rMatrix.AddNode(_dataType + 1, matrixR[batchIndex][_dataType]);
            }

            // Выполяем инициализацию матрицы P
            List<List<int>> pMatr = ListUtils.InitMatrixInt(matrixR[0].Count, matrixR.Count, 0);

            // Для каждой позиции в последовательности выполняем перебор
            for (int batchIndex = 0; batchIndex < matrixR.Count; batchIndex++)
            
                // Для каждого типа данных выполняем перебор
                for (int _dataType = 0; _dataType < matrixR[batchIndex].Count; _dataType++)
                
                    // Если значение в позиции и типа не равно 0, инвертируем его в новой матрице pMatr
                    if (matrixR[batchIndex][_dataType] != 0)
                        pMatr[_dataType][batchIndex] = 1;

            // Инициализируем матрицу P
            Matrix pMatrix = new Matrix(pMatr);

            // Инициализируем матрицу переналадки приборов
            TreeDimMatrix timeChangeover = new TreeDimMatrix(changeoverTime);

            // Выполняем построение матрицы времён начала заданий
            TreeDimMatrix tnMatrix = CalculationService.CalculateTnMatrix(rMatrix, pMatrix, proccessingTimeMatrix, timeChangeover, bufferSize);

            // Если визуализация включена отображаем Excel
            if (Form1.vizualizationOn)
            {
                viz.CreateExcelAppList(deviceCount, dataTypesCount);
                viz.Visualize(tnMatrix, proccessingTimeMatrix, rMatrix);
            }

            // Достаём последний элемент из матрицы времён начала заданий
            TreeDimMatrixNode lastNode = tnMatrix.treeDimMatrix.Last();

            // Из последнего элемента матрицы достаём время начала обработки последнего задания
            int startTime = lastNode.time;

            // Определяем тип последнего задания
            int dataType = rMatrix[lastNode.fromDataType].dataType;

            // Определяем время обработки последнего задания в системе
            int procTime = proccessingTimeMatrix[lastNode.device-1, dataType-1];

            // Определяем новое время выполнения последнего задания в расписании
            timeOfLastScheduleExecution = startTime + procTime;
        }

        /// <summary>
        /// Данная функция возвращает тип данных в матрице R в позиции batchIndex
        /// </summary>
        /// <param name="batchIndex">Индекс партии в последовательности</param>
        /// <returns>Идентификатор типа данных или -1 в случае неудачи</returns>
        public int ReturnRDataType(int batchIndex)
        {
            for (var dataType = 0; dataType < matrixR[batchIndex].Count; dataType++)
                if (matrixR[batchIndex][dataType] > 0)
                    return dataType;
            return -1;
        }

        /// <summary>
        /// Данная функция возвращает количество заданий из матрице R в позиции batchIndex
        /// </summary>
        /// <param name="batchIndex">Индекс партии в последовательности, так же известный, как h</param>
        /// <returns>Количество заданий или -1 в случае неудачи</returns>
        public int ReturnRJobCount(int batchIndex)
        {
            for (var dataType = 0; dataType < matrixR[batchIndex].Count; dataType++)
                if (matrixR[batchIndex][dataType] > 0)
                    return matrixR[batchIndex][dataType];
            return -1;
        }

        /// <summary>
        /// Выполняем построения расписания, что подразумевает поиск наилучшего времени выполнения всех пакетов заданий в разных позициях
        /// </summary>
        public void ConstructShedule(int swapCount = 0)
        {

            // Если значение количество перестановок != 0,
            // то включаем перестановки
            bool swapping = (swapCount != 0);

            // Высчитываем временные критерии
            CalculateShedule();

            // Выполняем копирование матрицы R
            var bestMatrixR = ListUtils.MatrixIntDeepCopy(matrixR);

            // Сохраняем лучший результат времени выполнения последнего задания
            var bestTime = timeOfLastScheduleExecution;

            // Определяем индекс последнего элемента для перестановок
            int swapIndex = matrixR.Count - 1;

            // До тех пор, пока не выполнили все перестановки
            while(swapIndex - 1 >= 0)
            {

                // Если счётчик перестановок включён и количество перестановок закончилось
                if (swapping && swapCount <= 0)

                    // Прекаращаем обработку
                    break;

                // Выполняем перестановку двух позиций рядом
                ListUtils.MatrixIntRowSwap(matrixR, swapIndex, swapIndex - 1);

                // Для новой матрицы выполняем высчитывание временных критерией
                CalculateShedule();

                // Если высчитаное время хуже наилучшего
                if (timeOfLastScheduleExecution > bestTime)
                {
                    // Уменьшаем количество перестановок и индекс смещения
                    swapCount--;
                    swapIndex--;

                    // Пропускаем итерацию
                    continue;
                }

                // Переопределяем наилучшее время и матрицу R
                bestTime = timeOfLastScheduleExecution;
                bestMatrixR = ListUtils.MatrixIntDeepCopy(matrixR);

                // Уменьшаем количество перестановок и индекс смещения
                swapCount--;
                swapIndex--;
            }

            // Переопределяем общее время и матрицу R
            timeOfLastScheduleExecution = bestTime;
            matrixR = bestMatrixR;
        }

        /// <summary>
        /// Данная функция выполняет построение расписания с буфером
        /// </summary>
        /// <param name="bufferSize">Размер буфера</param>
        /// <param name="dataTypesCount">Количество типов данных</param>
        /// <returns></returns>
        public void ConstructSheduleWithBuffer(int bufferSize, int dataTypesCount, int swapCount = 0)
        {

            // Если значение количество перестановок != 0,
            // то включаем перестановки
            bool swapping = (swapCount != 0);

            // Высчитываем временные критерии
            CalculateSheduleWithBufer(bufferSize, dataTypesCount);

            // Выполняем копирование матрицы R и 
            // Сохраняем лучший результат времени выполнения последнего задания
            var bestMatrixR = ListUtils.MatrixIntDeepCopy(matrixR);
            var bestTime = timeOfLastScheduleExecution;

            // Определяем индекс последнего элемента для перестановок
            int swapIndex = matrixR.Count - 1;

            // До тех пор, пока не выполнили все перестановки
            while (swapIndex - 1 >= 0)
            {

                // Если счётчик перестановок включён и количество перестановок закончилось
                if (swapping && swapCount <= 0)

                    // Прекаращаем обработку
                    break;

                // Выполняем перестановку двух позиций рядом
                ListUtils.MatrixIntRowSwap(matrixR, swapIndex, swapIndex - 1);

                // Для новой матрицы выполняем высчитывание временных критерией
                CalculateSheduleWithBufer(bufferSize, dataTypesCount);

                // Если высчитаное время хуже наилучшего
                if (bestTime <= timeOfLastScheduleExecution)
                {
                    // Уменьшаем количество перестановок и индекс смещения
                    swapCount--;
                    swapIndex--;

                    // Пропускаем итерацию
                    continue;
                }

                // Переопределяем наилучшее время и матрицу R
                bestTime = timeOfLastScheduleExecution;
                bestMatrixR = ListUtils.MatrixIntDeepCopy(matrixR);

                // Уменьшаем количество перестановок и индекс смещения
                swapCount--;
                swapIndex--;
            }

            // Переопределяем общее время и матрицу R
            timeOfLastScheduleExecution = bestTime;
            matrixR = bestMatrixR;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tz">Бог видит, tz это Тарчок Запоздалый(Неизвестная переменная)</param>
        /// <param name="crit">Переменная для записи критерий</param>
        /// <returns>Время выполнения всех заданий</returns>
        public int GetTimeWithCriterium(int tz, out int crit)
        {

            // Данная переменная необходима для подсчёта времени выполнения всех заданий в системе
            var proccessingTime = 0;

            // Выполяем построение расписания
            ConstructShedule();

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < deviceCount; device++)
            {

                // Для каждой партии выполняем обработку
                for (int batchIndex = 0; batchIndex < startProcessing[device].Count; batchIndex++)
                
                    // Для каждого задания выполняем обработку
                    for (int job = 0; job < startProcessing[device][batchIndex].Count; job++)

                        // Сумируем разницу между времен конца и временем начала обработки задания job в партии batchIndex на приборе device
                        proccessingTime += stopProcessing[device][batchIndex][job] - startProcessing[device][batchIndex][job];
                    
                // Отнимаем время начала первого задания в первом пакете (наладку)
                proccessingTime -= startProcessing[device][0][0];
            }

            // Переопределяем критерий
            crit = (tz * deviceCount) - proccessingTime;
            
            // Возвращаем время выполнения последнего задания в последнем пакете на последнем приборе
            return timeOfLastScheduleExecution;
        }

        /// <summary>
        /// Возвращаем значение времени выполнения всех заданий
        /// </summary>
        /// <returns>Время выполнения всех заданий</returns>
        public int GetTime()
        {
            return timeOfLastScheduleExecution;
        }

        #region Неиспользуемые фукнции, которые страшно удалять

        /// <summary>
        /// Данная функция выполняет 
        /// </summary>
        /// <param name="position1"></param>
        /// <param name="position2"></param>
        private void ChangeColum(int position1, int position2)
        {
            var indd1 = 0;
            var indd2 = 0;
            for (var i = 0; i < matrixR[position1].Count; i++)
            {
                if (matrixR[position1][i] > 0)
                {
                    indd1 = i;
                }
            }
            for (var i = 0; i < matrixR[position2].Count; i++)
            {
                if (matrixR[position2][i] > 0)
                {
                    indd2 = i;
                }
            }
            var temp = matrixR[position1][indd1];
            matrixR[position1][indd1] = 0;
            matrixR[position1][indd2] = matrixR[position2][indd2];
            matrixR[position2][indd2] = 0;
            matrixR[position2][indd1] = temp;
        }

        /// <summary>
        /// Выполняем построения расписания, что подразумевает поиск наилучшего времени выполнения всех пакетов заданий в разных позициях
        /// </summary>
        public void ConstructSheduleCOPY()
        {

            // Высчитываем временные критерии
            CalculateShedule();

            // Выполняем копирование матрицы R
            var matrixRCopy = ListUtils.MatrixIntDeepCopy(matrixR);

            // Сохраняем лучший результат времени выполнения последнего задания
            var tempTime = timeOfLastScheduleExecution;

            // Пробегаемся по всем позициям матриц R
            for (var batchIndex_i = 0; batchIndex_i < matrixR.Count - 1; batchIndex_i++)
            {

                // Пробегаемся по всем позициям матриц R
                for (var batchIndex_j = batchIndex_i + 1; batchIndex_j < matrixR.Count; batchIndex_j++)

                    // Ранее использовалась функция "ChangeColum(i, j);", которая в результате выполняла перестановку строк
                    // Выполяем перестановку местами строки i и j
                    ListUtils.MatrixIntRowSwap(matrixR, batchIndex_i, batchIndex_j);

                // Для новой матрицы выполняем высчитывание временных критерией
                CalculateShedule();

                // Если сохранённый результат хуже, чем новый, пропускаем итерацию
                if (tempTime >= timeOfLastScheduleExecution)
                    continue;

                // TODO: Данная строчка может являеться не корректной, так как выполняет поверхностое
                // копирования и после первого прохода будет ссылаться на одну и ту же матрицу
                matrixR = matrixRCopy;

                // Выполняем переопределение нового лучшего времени
                timeOfLastScheduleExecution = tempTime;
            }
        }

        /// <summary>
        /// Данная функция выполняет построение расписания с буфером
        /// </summary>
        /// <param name="bufferSize">Размер буфера</param>
        /// <param name="dataTypesCount">Количество типов данных</param>
        /// <returns></returns>
        public void ConstructSheduleWithBuffer1(int bufferSize, int dataTypesCount)
        {

            // Высчитываем временные критерии
            CalculateSheduleWithBufer(bufferSize, dataTypesCount);

            // Выполняем копирование матрицы R
            var tempR = ListUtils.MatrixIntDeepCopy(matrixR);

            // Сохраняем лучший результат времени выполнения последнего задания
            var tempTime = timeOfLastScheduleExecution;

            // Выполяем все возможные перестановки позиций
            // Для всех позиций от 0 до matrixR.Count - 1 выполняем перебор
            for (var batchIndex_i = 0; batchIndex_i < matrixR.Count - 1; batchIndex_i++)
            {

                // Для всех позиций от pos1 + 1 до matrixR.Count выполняем все перестановку строк (представляющие из себя позицию)
                for (var batchIndex_j = batchIndex_i + 1; batchIndex_j < matrixR.Count; batchIndex_j++)

                    // Ранее использовалась функция "ChangeColum(i, j);", которая в результате выполняла перестановку строку
                    // Выполяем перестановку местами строки i и j
                    ListUtils.MatrixIntRowSwap(matrixR, batchIndex_i, batchIndex_j);

                // Выполяем высчитывание расписания с буфером для всех возможных последовательностей
                CalculateSheduleWithBufer(bufferSize, dataTypesCount);

                // Если сохранённый результат хуже, чем новый, пропускаем итерацию
                if (tempTime >= timeOfLastScheduleExecution)
                    continue;

                // TODO: Данная строчка может являеться не корректной, так как выполняет поверхностое
                // копирования и после первого прохода будет ссылаться на одну и ту же матрицу
                matrixR = tempR;

                // Выполняем переопределение нового лучшего времени
                timeOfLastScheduleExecution = tempTime;
            }
        }

        #endregion
    }

    /// <summary>
    /// Данный класс описывает корректное расписание 
    /// </summary>
    public class CorrectSchedule
    {

        /// <summary>
        /// Данная переменная определяет будет ли выводиться отладачная информация для функции Build
        /// </summary>
        private static readonly bool isDebugBuild = true;

        /// <summary>
        /// Данная переменная определяет будет ли выводиться отладачная информация для функции Optimization
        /// </summary>
        private static readonly bool isDebugOptimization = true;

        /// <summary>
        /// Данная переменная определяет будет ли выводиться отладачная информация для функции GetDowntimeFrom
        /// </summary>
        private static readonly bool isDebugGetDowntimeFrom = true;

        /// <summary>
        /// Данная функция выполняет локальную оптимизацию составов ПЗ
        /// </summary>
        /// <param name="config">Конфигурационная структура содержит информацию о конфигурации системы</param>
        /// <param name="schedule">Последовательность ПЗ. Данная переменная представляет
        /// множество ПЗ для последующих перестановок.</param>
        /// <param name="swapCount">Количество перестановок.</param>
        /// <returns>Наилучший порядок ПЗ</returns>
        private static List<magisterDiplom.Model.Batch> Optimization(
            Config config,
            List<magisterDiplom.Model.Batch> schedule,
            int swapCount = 999999
        ) {

            // Объявляем и инициализируем индекс перестановки
            int swapIndex = 0;

            // Объявляем лучшее расписание
            List<magisterDiplom.Model.Batch> bestSchedule = new List<magisterDiplom.Model.Batch>(schedule);

            // Объявляем переменную для соответствий приборов к матрицам начала времени выполнения
            Dictionary<int, List<List<int>>> matrixT = PreM.Build(config, schedule, null);

            // Объявляем и инициализируем временной критерий, как момент времени окончания
            // выполнения последнего задания в последнем пакете на последнем приборе
            int bestTime = CorrectSchedule.GetMakespanFrom(config, matrixT, schedule);

            // Объявляем и инициализируем новый временной критерий, как момент времени окончания
            // выполнения последнего задания в последнем пакете на последнем приборе
            int newTime = 0;

            // Выполняем заявленное количество перестановок, заявленно количество раз
            for (int batchIndex = schedule.Count - 1; batchIndex >= 1 && swapIndex < swapCount; batchIndex--, swapIndex++)
            {

                // Выполняем перестановку
                magisterDiplom.Model.Batch batch = schedule[batchIndex];
                schedule[batchIndex] = schedule[batchIndex - 1];
                schedule[batchIndex - 1] = batch;

                // Для каждой перестановки выполняем просчёт матрицы
                // начала времени выполнения задания на приборах
                matrixT = PreM.Build(config, schedule, null);

                // Высчитываем новый критерий makespan
                newTime = CorrectSchedule.GetMakespanFrom(config, matrixT, schedule);

                // Если лучшее время хуже (больше) чем время после перестановки
                if (newTime <= bestTime)
                {

                    // TODO: Избавиться от копирования списка в пользу использования индекса наилучшей позиции
                    // Переопределяем лучшее расписание
                    bestSchedule = new List<magisterDiplom.Model.Batch>(schedule);

                    // Переопределяем лучшее время для лучшего расписания
                    bestTime = newTime;
                }
            }

            // Выполняем переопределение наилучшего раысписания составов ПЗ
            return bestSchedule;
        }

        public static List<magisterDiplom.Model.Batch> Build(
            Config config,
            List<List<int>> matrixA
        ) {
            
            // Выводим отладачную информацию
            if (Config.isDebug && CorrectSchedule.isDebugBuild)
            {
                Console.WriteLine("\t\t+---------------------+");
                Console.WriteLine("Start\t|   CorrectSchedule   |");
                Console.WriteLine("\t\t+---------------------+");
                Console.WriteLine("Input:\t");

                Console.WriteLine("\t\t+---------------------+");
                Console.WriteLine("\t\t|       config        |");
                Console.WriteLine("\t\t+---------------------+");
                Console.WriteLine(config.ToString("\t\t"));

                Console.WriteLine("\t\t+---------------------+");
                Console.WriteLine("\t\t|       matrixA       |");
                Console.WriteLine("\t\t+---------------------+");

                for (int _dataType = 0; _dataType < config.dataTypesCount; _dataType++)
                {
                    Console.Write("\t\t|{0,-2}|", _dataType);
                    for (int _batchIndex = 0; _batchIndex < matrixA[_dataType].Count; _batchIndex++)
                        Console.Write("{0, -2}|", matrixA[_dataType][_batchIndex]);
                    Console.Write(Environment.NewLine);
                }
                Console.Write(Environment.NewLine);
            }

            // Объявляем тип данных
            int dataType;

            // Объявляем ПЗ
            int batch = 0;

            // Объявляем максимальное количество пакетов
            int maxBatchCount = 0;

            // Инициализируем максимальное количество пакетов
            for (dataType = 0; dataType < config.dataTypesCount; dataType++)   
                maxBatchCount = Math.Max(maxBatchCount, matrixA[dataType].Count);

            // Объявляем список данных состоящий из ПЗ
            List<magisterDiplom.Model.Batch> schedule = new List<magisterDiplom.Model.Batch>(maxBatchCount);

            // Выполняем обработку
            while (batch < maxBatchCount) {

                // Выполняем обработку для каждого типа данных
                for (dataType = 0; dataType < config.dataTypesCount; dataType++)
                {

                    // Если индекс пакета превышает максимальный размер пакетов для типа данных dataType
                    if (batch >= matrixA[dataType].Count)

                        // Продолжаем обработку для следующего типа данных
                        continue;

                    // Добавляем ПЗ в расписание 
                    schedule.Add(new magisterDiplom.Model.Batch(dataType, matrixA[dataType][batch]));
                    
                    // Выводим отладачную информацию
                    if (Config.isDebug && CorrectSchedule.isDebugBuild)
                    {

                        // Объявляем индекс ПЗ в расписании
                        int batchIndex = 0;

                        // Добавляем ПЗ в расписание
                        Console.WriteLine("Add:\t[{0}:{1, -2}]", schedule.Last().Type, schedule.Last().Size);
                        Console.Write(Environment.NewLine);

                        // Выводим всё расписание
                        Console.WriteLine("Before:\t");
                        for (; batchIndex < schedule.Count; batchIndex++)
                            Console.WriteLine("\t\t[{0}:{1, -2}]", schedule[batchIndex].Type, schedule[batchIndex].Size);
                        Console.Write(Environment.NewLine);
                    }

                    // Выполняем локальную оптимизацию составов ПЗ
                    schedule = Optimization(config, schedule);

                    // Выводим отладачную информацию
                    if (Config.isDebug && CorrectSchedule.isDebugBuild)
                    {

                        // Объявляем индекс ПЗ в расписании
                        int batchIndex = 0;

                        // Выводим всё расписание
                        Console.WriteLine("After:\t");
                        for (; batchIndex < schedule.Count; batchIndex++)
                            Console.WriteLine("\t\t[{0}:{1, -2}]", schedule[batchIndex].Type, schedule[batchIndex].Size);
                        Console.Write(Environment.NewLine);
                    }
                }

                // Увеличиваем индекс пакета
                batch++;
            }

            // Выводим отладачную информацию
            if (Config.isDebug && CorrectSchedule.isDebugBuild)
            {
                Console.WriteLine("      +---------------------+");
                Console.WriteLine("Stop  |   CorrectSchedule   |");
                Console.WriteLine("      +---------------------+");
            }

            // Вызвращаем локально оптимизированное распиание составов ПЗ
            return schedule;
        }

        /// <summary>
        /// Данная функция возвращает критерий оптимизации makespan определяющий время выполнения всех заданий в конвейерной системе
        /// </summary>
        /// <param name="config">Структура описывающая конфигурацию по
        /// которой будет выполняться построение расписания
        /// </param>
        /// <param name="matrixT">Словарь соответсвий приборов к
        /// матрицам моментов начала времени выполнений
        /// </param>
        /// <param name="schedule">Последовательность ПЗ. Данная переменная представляет
        /// множество ПЗ.
        /// </param>
        /// <returns>Makespan или время выполнения всех заданий в системе</returns>
        public static int GetMakespanFrom(
            Config config,
            Dictionary<int, List<List<int>>> matrixT,
            List<magisterDiplom.Model.Batch> schedule
            )
        {
            return matrixT[config.deviceCount - 1].Last().Last() + config.proccessingTime[config.deviceCount - 1, schedule.Last().Type];
        }

        /// <summary>
        /// Данная функция выполняет подсчёт простоев для переданного расписания и матрицы моментов начала времени выполнения
        /// </summary>
        /// <param name="config">Структура описывающая конфигурацию по
        /// которой будет выполняться построение расписания
        /// </param>
        /// <param name="matrixT">Словарь соответсвий приборов к
        /// матрицам моментов начала времени выполнений
        /// </param>
        /// <param name="schedule">Последовательность ПЗ. Данная переменная представляет
        /// множество ПЗ.
        /// </param>
        /// <returns>Время простоя</returns>
        public static int GetDowntimeFrom(
            Config config,
            Dictionary<int, List<List<int>>> matrixT,
            List<magisterDiplom.Model.Batch> schedule
            )
        {

            // Выводим отладачную информацию
            if (Config.isDebug && CorrectSchedule.isDebugGetDowntimeFrom)
                Console.WriteLine($"Start: GetDowntimeFrom;");
            
            // Объявляем и инициализируем простои
            int downtime = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Подсчитываем простои связанные с наладкой
                downtime += matrixT[device][0][0];

                // Выводим отладачную информацию
                if (Config.isDebug && CorrectSchedule.isDebugGetDowntimeFrom)
                    Console.WriteLine($"\tdevice:{device}; downtime+{matrixT[device][0][0]}={downtime}");

                // TODO: Для первого прибора не будет простоев между заданиями. Вынести рассчёт для первого прибора в отдельный блок
                // Для кажого задания пакета на первой позиции
                for (int job = 1; job < matrixT[device][0].Count(); job++)
                {

                    // Подсчитываем простои между заданиями
                    downtime +=
                        
                        // Момент начала времени выполнения текущего задания
                        matrixT[device][0][job] -

                        // Момент начала времени выполнения предыдущего задания
                        (matrixT[device][0][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device, schedule[0].Type]);

                    // Выводим отладачную информацию
                    if (Config.isDebug && CorrectSchedule.isDebugGetDowntimeFrom) {

                        // Время простоя
                        int time =

                            // Момент начала времени выполнения текущего задания
                            matrixT[device][0][job] -

                            // Момент начала времени выполнения предыдущего задания
                            (matrixT[device][0][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device, schedule[0].Type]);

                        Console.WriteLine($"\t  job:{job,-2}; downtime+{time}={downtime}");
                    }
                }

                // Для каждого пакета со второго выполняем обработку
                for (int batchIndex = 1; batchIndex < matrixT[device].Count(); batchIndex++)
                {

                    // Подсчитываем простои между пакетами
                    downtime +=

                        // Момент начала времени выполнения первого задания текущего пакет
                        matrixT[device][batchIndex][0] -

                        // Момент начала времени выполнения последнего задания на предыдущем пакете
                        (matrixT[device][batchIndex - 1].Last() +

                        // Время выполнения задания в предыдущем пакете
                        config.proccessingTime[device, schedule[batchIndex-1].Type]);

                    // Выводим отладачную информацию
                    if (Config.isDebug && CorrectSchedule.isDebugGetDowntimeFrom)
                    {

                        // Время простоя
                        int time =

                            // Момент начала времени выполнения первого задания текущего пакет
                            matrixT[device][batchIndex][0] -

                            // Момент начала времени выполнения последнего задания на предыдущем пакете
                            (matrixT[device][batchIndex - 1].Last() +

                            // Время выполнения задания в предыдущем пакете
                            config.proccessingTime[device, schedule[batchIndex - 1].Type]);

                        Console.WriteLine($"\t  batchIndex:{batchIndex,-2}; downtime+{time}={downtime}");
                    }

                    // Для кажого задания пакета на первой позиции
                    for (int job = 1; job < matrixT[device][batchIndex].Count(); job++)
                    {

                        // Подсчитываем простои между заданиями
                        downtime +=

                            // Момент начала времени выполнения текущего задания
                            matrixT[device][batchIndex][job] -

                            // Момент начала времени выполнения предыдущего задания
                            (matrixT[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device, schedule[batchIndex].Type]);

                        // Выводим отладачную информацию
                        if (Config.isDebug && CorrectSchedule.isDebugGetDowntimeFrom)
                        {

                            // Время простоя
                            int time =

                                // Момент начала времени выполнения текущего задания
                                matrixT[device][batchIndex][job] -

                                // Момент начала времени выполнения предыдущего задания
                                (matrixT[device][batchIndex][job - 1] +

                                // Время выполнения предыдущего задания
                                config.proccessingTime[device, schedule[batchIndex].Type]);

                            Console.WriteLine($"\t    job:{job, -2}; downtime+{time}={downtime}");
                        }
                    }
                }
            }

            // Выводим отладачную информацию
            if (Config.isDebug && CorrectSchedule.isDebugGetDowntimeFrom)
                Console.WriteLine("Stop: GetDowntimeFrom");

            // Возвращаем результат
            return downtime;
        }

        /// <summary>
        /// Данная функция выполняет подсчёт полезности для переданного расписания и матрицы моментов начала времени выполнения
        /// Полезность - есть время, которое приборы тратят на обработку ПЗ, без учёта ПТО, наладки и переналадки.
        /// Полезность высчитывается, как время выполнения всех заданий минус время простоя
        /// </summary>
        /// <param name="config">Структура описывающая конфигурацию по
        /// которой будет выполняться построение расписания
        /// </param>
        /// <param name="matrixT">Словарь соответсвий приборов к
        /// матрицам моментов начала времени выполнений
        /// </param>
        /// <param name="schedule">Последовательность ПЗ. Данная переменная представляет
        /// множество ПЗ.
        /// </param>
        /// <returns>Время полезности</returns>
        public static int GetUtilityFrom(
            Config config,
            Dictionary<int, List<List<int>>> matrixT,
            List<magisterDiplom.Model.Batch> schedule
            )
        {

            // Возвращяем полезность
            return

                // Подсчитываем время выполнения всех заданий
                GetMakespanFrom(config, matrixT, schedule) -
                
                // Время простоя
                GetDowntimeFrom(config, matrixT, schedule);
        }


        /// <summary>
        /// Данная функция выполняет подсчёт полезности и суммы интервалов времени между ПТО для переданного расписания и матрицы моментов начала времени выполнения
        /// Полезность - есть время, которое приборы тратят на обработку ПЗ, без учёта ПТО, наладки и переналадки.
        /// Полезность высчитывается, как время выполнения всех заданий минус время простоя
        /// </summary>
        /// <param name="config">Структура описывающая конфигурацию по
        /// которой будет выполняться построение расписания
        /// </param>
        /// <param name="matrixT">Словарь соответсвий приборов к
        /// матрицам моментов начала времени выполнений
        /// </param>
        /// <param name="schedule">Последовательность ПЗ. Данная переменная представляет множество ПЗ. </param>
        /// <param name="matrixTPreM">Матрица моментов окончания времени выполнения ПТО для каждого пакета. [device x batchIndex]</param>
        /// <returns>Время полезности и ПТО</returns>
        public static int GetPreMUtilityFrom(
            Config config,
            Dictionary<int, List<List<int>>> matrixT,
            List<magisterDiplom.Model.Batch> schedule,
            List<List<int>> matrixTPreM
        ) {

            // Объявляем и инициализируем сумму интервалов времени между ПТО
            int sumPreMIntervals = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++) {

                // Проверяем вектор матрицы на пустоту
                if (matrixTPreM[device].Count() == 0)
                    continue;

                // Выполняем подсчёт суммы интервалов времени на первом пакете ПТО
                sumPreMIntervals += matrixTPreM[device][0];

                // Для каждого пакета выполняем обработку
                for (int batchIndex = 1; batchIndex < matrixT[device].Count(); batchIndex++)

                    // Выполняем подсчёт суммы интервалов времени между ПТО разных пакетов
                    sumPreMIntervals += matrixTPreM[device][batchIndex] - matrixTPreM[device][batchIndex - 1];
    }

            // Возвращяем полезность и ПТО
            return

                // Получаем полезность
                GetUtilityFrom(config, matrixT, schedule) +

                // Получаем ПТО
                sumPreMIntervals;
        }
    }

    /// <summary>
    /// Класс PreM (Preliminary maintenance) предоставляет функции для работы с предварительным обслуживанием приборов
    /// </summary>
    public class PreM
    {

        /// <summary>
        /// Данная функция выполняет построение матрицы начала времени выполнения заданий.
        /// </summary>
        /// <param name="config">Структура описывающая конфигурацию по
        /// которой будет выполняться построение расписания
        /// </param>
        /// <param name="schedule">Последовательность ПЗ. 
        /// Каждый ПЗ содержит в себе информацию о количестве заданий и типе задания.
        /// </param>
        /// <param name="matrixY">Матрица порядка выполнения ПТО.
        /// Y[deviceCount x maxBatchCount]</param>
        /// <returns>
        /// Словарь соответствий прибора к матрице начала времени выполнения
        /// заданий в пакетах. [device]:[batchIndex x job]
        /// </returns>
        public static Dictionary<int, List<List<int>>> Build(
            Config config,
            List<magisterDiplom.Model.Batch> schedule,
            List<List<int>> matrixY
        ) {

            // Выпоняем проверку на пустую матрицу Y
            if (matrixY == null)
            {
                
                // Выполняем инициализацию матрицы Y
                matrixY = new List<List<int>>(config.dataTypesCount);
                for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
                    matrixY.Add(ListUtils.InitVectorInt(schedule.Count, 0));
            }

            // Объявляем словарь соответствий
            Dictionary<int, List<List<int>>> matrixT = new Dictionary<int, List<List<int>>>();

            // Объявляем индекс ПЗ
            int batchIndex = 0;

            // Объявляем индекс прибора
            int device = 0;

            // Объявляем индекс задания
            int job = 0;

            // Инициалиизруем матрицу заданий в пакете
            List<List<int>> times = new List<List<int>>();
            for (batchIndex = 0; batchIndex < schedule.Count(); batchIndex++)
                times.Add(ListUtils.InitVectorInt(schedule[batchIndex].Size));

            // Инициализируем словарь соответствий
            for (device = 0; device < config.deviceCount; device++)
                matrixT.Add(device, ListUtils.MatrixIntDeepCopy(times));

            // Выводим отладачную информацию
            if (Config.isDebug) {

                // Выводим информацию о всей матрице T
                Console.WriteLine("Before calculation");
                for (device = 0; device < config.deviceCount; device++)
                    Console.WriteLine($"device:{device}" + Environment.NewLine + ListUtils.MatrixIntToString(matrixT[device]));
        }

            // Выполняем обработку для первого прибора
        {

                // Инициализируем индекс первого прибора
                device = 0;

                // Инициализируем индекс первого ПЗ
                batchIndex = 0;

                // Инициализируем индекс первого задания
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на 1 приборе, как наладку
                matrixT[device][batchIndex][job] = config.changeoverTime[device][schedule[batchIndex].Type, schedule[batchIndex].Type];

                // Пробегаемся по всем заданиям пакета в первой позиции
                for (job = 1; job < schedule[batchIndex].Size; job++)

                    // Устанавливаем момент начала времени выполнения задания job
                    matrixT[device][batchIndex][job] =

                        // Момент начала времени выполнения предыдущего задания
                        matrixT[device][batchIndex][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device, schedule[batchIndex].Type];

                // Пробегаемся по всем возможным позициям cо второго пакета
                for (batchIndex = 1; batchIndex < schedule.Count(); batchIndex++)
                {

                    // Инициализируем индекс первого задания
                    job = 0;

                    // Момент начала времени выполнения 1 задания в пакете на позиции batchIndex
                    matrixT[device][batchIndex][job] =

                        // Момент начала времени выполнения последнего задания в предыдущем пакете
                        matrixT[device][batchIndex - 1].Last() +

                        // Время выполнения задания в предыдущем пакете
                        config.proccessingTime[device, schedule[batchIndex - 1].Type] +

                        // Время переналадки с предыдущего типа на текущий
                        config.changeoverTime[device][schedule[batchIndex - 1].Type, schedule[batchIndex].Type] + 

                        // Время выполнения ПТО после предыдущего ПЗ
                        config.preMaintenanceTimes[0] * matrixY[device][batchIndex - 1];

                    // Пробегаемся по всем заданиям пакета в позиции batchIndex
                    for (job = 1; job < schedule[batchIndex].Size; job++)

                        // Вычисляем момент начала времени выполнения задания job в позиции batchIndex на 1 приборе
                        matrixT[device][batchIndex][job] =

                            // Момент начала времени выполнения предыдущего задания
                            matrixT[device][batchIndex][job - 1] +

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
                matrixT[device][batchIndex][job] = Math.Max(

                    // Время наладки прибора на выполнение 1 задания в 1 пакете
                    config.changeoverTime[device][schedule[batchIndex].Type, schedule[batchIndex].Type],

                    // Время окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                    matrixT[device - 1][batchIndex][job] + config.proccessingTime[device-1, schedule[batchIndex].Type]
                );

                // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                for (job = 1; job < schedule[batchIndex].Size; job++)

                    // Устанавливаем момент начала времени выполнения текущего задания job, как
                    // Максимум, между временем окончания предыдущего задания на текущем приборе и
                    // временем окончания текущего задания на предыдущем приборе
                    matrixT[device][batchIndex][job] = Math.Max(

                        // Момент начала времени выполнения предыдущего задания
                        matrixT[device][batchIndex][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device, schedule[batchIndex].Type],

                        // Момент начала времени выполнения текущего задания на предыдущем приборе
                        matrixT[device - 1][batchIndex][job] +

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
                    matrixT[device][batchIndex][job] = Math.Max(

                        // Момент начала времени выполнения последнего задания в предыдущем ПЗ
                        matrixT[device][batchIndex - 1].Last() +

                        // Время выполнения последнего задания в предыдущем ПЗ
                        config.proccessingTime[device, schedule[batchIndex - 1].Type] +

                        // Время переналадки с предыдущего типа на текущий
                        config.changeoverTime[device][schedule[batchIndex - 1].Type, schedule[batchIndex].Type] +

                        // Время выполнения ПТО
                        config.preMaintenanceTimes[device] * matrixY[device][batchIndex - 1],

                        // Момент начала времени выполнения 1 задания на предыдущем приборе
                        matrixT[device - 1][batchIndex][job] +

                        // Время выполнения 1 задания на предыдущем приборе
                        config.proccessingTime[device - 1, schedule[batchIndex].Type]);

                    // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                    for (job = 1; job < schedule[batchIndex].Size; job++)

                        // Устанавливаем момент начала времени выполнения текущего задания job, как
                        // Максимум, между временем окончания предыдущего задания на текущем приборе и
                        // временем окончания текущего задания на предыдущем приборе
                        matrixT[device][batchIndex][job] = Math.Max(

                            // Момент начала времени выполнения предыдущего задания
                            matrixT[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device, schedule[batchIndex].Type],

                            // Момент начала времени выполнения задания на предыдущем приборе
                            matrixT[device - 1][batchIndex][job] +

                            // Время выполнения задания на предыдущем приборе
                            config.proccessingTime[device - 1, schedule[batchIndex].Type]
                        );
                    }
            }

            // Выводим отладачную информацию
            if (Config.isDebug)
            {

                // Выводим информацию о всей матрице T
                Console.WriteLine("After calculation");
                for (device = 0; device < config.deviceCount; device++)
                    Console.WriteLine($"device:{device}" + Environment.NewLine + ListUtils.MatrixIntToString(matrixT[device]));
            }

            // Возвращаем матрицу
            return matrixT;
        }

        /// <summary>
        /// Данная функция выполняет построение матрицы моментов окончания времени выполнения ПТО.
        /// </summary>
        /// <param name="config">Структура описывающая конфигурацию по
        /// которой будет выполняться построение расписания
        /// </param>
        /// <param name="schedule">Последовательность ПЗ. 
        /// Каждый ПЗ содержит в себе информацию о количестве заданий и типе задания.
        /// </param>
        /// <param name="matrixT">Словарь соответствий прибора к матрице начала времени выполнения
        /// заданий в пакетах. [device]:[batchIndex x job]
        /// </param>
        /// <param name="matrixY">Матрица порядка выполнения ПТО.
        /// Y[deviceCount x maxBatchCount]</param>
        /// <returns>
        /// Матрица моментов окончания времени выполнения ПТО для каждого пакета. [device x size]
        /// </returns>
        public static List<List<PreMSet>> BuildMatrixTPreM(
            Config config,
            List<magisterDiplom.Model.Batch> schedule,
            Dictionary<int, List<List<int>>> matrixT,
            List<List<int>> matrixY
        ) {

            // Объявляем матрицу моментов окончания реализации ПТО
            List<List<PreMSet>> matrixTPreM = new List<List<PreMSet>>(config.deviceCount);

            // Для каждого прибора выполням обработку
            for (int device = 0; device < config.deviceCount; device++) {

                // Инициализируем ПТО для прибора
                matrixTPreM.Add(new List<PreMSet>());

                // Для каждого прибора в расписании выполняем обработку
                for (int batchIndex = 0; batchIndex < schedule.Count; batchIndex++)

                    // Если для текущей позиции есть ПТО
                    if (matrixY[device][batchIndex] == 1)

                        // Момент окончания времени выполнения ПТО на позиции batchIndex
                        matrixTPreM[device].Add(

                            // Добавляем структуры данных
                            new PreMSet(

                                // Индекс ПЗ после которого будет выполняться ПТО
                                batchIndex,
                                
                            // Момент начала времени выполнения последнего задания в пакете batchIndex на приборе device
                            matrixT[device][batchIndex].Last() +

                            // Время выполнения задания с типов пакета на позиции batchIndex
                            config.proccessingTime[device, schedule[batchIndex].Type] +

                            // Время выполнения ПТО
                            config.preMaintenanceTimes[device]
                            )

                            
                        );
            }

            // Возвращяем результат
            return matrixTPreM;
        }

        /// <summary>
        /// Данная функция возврабщает надёжность, которая определяет вероятность находится ли некий прибор в работоспособном состоянии.
        /// Данная функция не учитывает перерывы в работе прибора связанные с:
        ///     1. Неготовностью к выполнению заданий, входящих в пакеты;
        ///     2. Переналадкой приборов.
        /// </summary>
        /// <param name="failureRates">Данный список определяет интенсивность отказов для приборов соответсвенно: [deviceCount]</param>
        /// <param name="restoringDevice">Данный список определяет востановление прибора соответсвенно: [deviceCount]</param>
        /// <param name="t">
        /// Когда t = 0, прибор расценивается, "как новый".
        /// После окончания ПТО, прибора находится в состоянии, "как новый".
        /// </param>
        /// <param name="device">Индекс прибора для расчёта вероятности его работоспособности</param>
        /// <returns>Вероятность работоспособности прибора по индексу device</returns>
        public static double ReliabilityCalc(
            List<int> failureRates,
            List<int> restoringDevice,
            int t, int device
        ) {
            // deviceTime = tl = общее время пребывания l-ого прибора в активном состоянии, после 
            //     окончания последней реализации ПТО этого прибора в момент времени (tl^p (tl^p >= 0))
            // TODO: Заменить failureRates и restoringDevice на доступ через config.
            double reliability =
                failureRates[device] / (restoringDevice[device] + failureRates[device]) +
                restoringDevice[device] / (restoringDevice[device] + failureRates[device]) *
                Math.Exp(-1 * (restoringDevice[device] + failureRates[device]) * t);

            return reliability;
        }

        /// <summary>
        /// Данная фукнция выполняет возврат последнего индекса ПЗ,
        /// после которого выполняется ПТО с моментов окончания времени выполнения moment
        /// </summary>
        /// <param name="matrixTPreM">
        /// Матрица моментов окончания времени выполнения ПТО для каждого пакета. [device x batchIndex]</param>
        /// <param name="device">Индекс прибора по которому будет выполняться выборка</param>
        /// <param name="moment">Левый диапазон выборки</param>
        /// <returns>Индекс после которого будет выполняться последнее ПТО в диапазоне</returns>
        public static int GetBatchIndex(
            List<List<PreMSet>> matrixTPreM,
            int device,
            int moment
        ) {

            // Если список пустой
            if (matrixTPreM[device].Count == 0)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Если в списке первый элемент не удовлетворяет условию
            if (matrixTPreM[device][0].TimePreM >= moment)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Объявляем индекс
            int index = 1;

            // Для каждой ПТО выполняем обработку
            for (; index < matrixTPreM[device].Count; index++)

                // Если момент окончания ПТО в позиции index не удовлетворяет условиям
                if (matrixTPreM[device][index].TimePreM >= moment)

                    // Возвращяем индекс ПЗ после которого выполнится последнее ПТО
                    return matrixTPreM[device][index - 1].BatchIndex;

            // Индекс ПЗ после которог выполниться ПТО, последний в списке
            return matrixTPreM[device][index - 1].BatchIndex;
        }

        /// <summary>
        /// Данная функция выполняет подсчёт информационных полей, для каждого прибора,
        /// таких что находятся на заданном интервале времени
        /// </summary>
        /// <param name="config">Конфигурационная структура</param>
        /// <param name="schedule">Расписания ПЗ [batchIndex]</param>
        /// <param name="matrixT">Матрица моментов начала времени выполнения задания [device]:[batchIndex][jobCount]</param>
        /// <param name="matrixTPreM">Матрица структур состоящая из индексов ПЗ за которыми следует ПТО и моментов времени окончания данного ПТО</param>
        /// <param name="endPreMTime">Крайний момент времени окончания ПТО</param>
        /// <returns>Список информационных структур</returns>
        public static List<PreMInfo> BuildVectorPreMInfo(
            Config config,
            List<magisterDiplom.Model.Batch> schedule,
            Dictionary<int, List<List<int>>> matrixT,
            List<List<PreMSet>> matrixTPreM,
            int endPreMTime
        ) {

            // Формируем список активностей приборов
            List<PreMInfo> info = new List<PreMInfo> (config.deviceCount);
            
            // Для каждого прибора выполняем подсчёт времени активности
            for (int device = 0; device < config.deviceCount; device++) {

                // Определяем начальный индекс
                int startIndex = PreM.GetBatchIndex(matrixTPreM, device, endPreMTime) + 1;

                // Определяем индекс ПЗ
                int batchIndex = startIndex;

                // Определяем количество ПЗ
                int batchCount = 0;

                // Определяем количество заданий
                int jobCount   = 0;

                // Для каждого пакет выполняем обработку
                for (; batchIndex < schedule.Count; batchIndex++) {

                    // Если первое задани в ПЗ удовлетворяет условию
                    if (matrixT[device][batchIndex][0] >= endPreMTime)

                        // Прекращаем обарботку
                        break;

                    // Увеличиваем количество ПЗ
                    batchCount++;
                }

                // Если количество ПЗ равно 0
                if (batchCount == 0)
                {

                    // Добавляем информацию
                    info.Add( new magisterDiplom.Model.PreMInfo( 0, 0 ) );

                    // Пропускаем обработку
                    continue;
                }

                // Для каждого задания в последнем пакете выполняем обработку
                for (int job = 0; job < schedule[startIndex + batchCount - 1].Size; job++) {
                
                    // Если некоторое задание в последенем ПЗ не удовлетворяет условию
                    if (matrixT[device][startIndex + batchCount - 1][job] >= endPreMTime)

                        // Прекращаем обарботку
                        break;

                    // Увеличиваем количество заданий
                    jobCount++;
                }

                // Добавляем информацию
                info.Add(
                    new magisterDiplom.Model.PreMInfo(
                        batchCount,
                        schedule[startIndex + batchCount - 1].Size - jobCount
                    )
                );
            }

            // Возвращяем результат
            return info;
        }
    }
}
