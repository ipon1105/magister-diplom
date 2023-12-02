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
        /// Данная переменная определяет длину конвейера, как количество приборов
        /// TODO: Убрать статическое определение количества приборов, с целью масштабируемости
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
        private int _timeConstructShedule;

        /// <summary>
        /// Данный словарь определяет для каждого прибора собственную матрицу начала времени обработки. Каждая строка матрицы представляет из себя вектор заданий.
        /// </summary>
        private Dictionary<int, List<List<int>>> startProcessing; // [deviceCount] : [maxBatchesPositions x jobCount]

        /// <summary>
        /// Данный словарь определяет для каждого прибора собственную матрицу конца времени обработки. Каждая строка матрицы представляет из себя вектор заданий.
        /// </summary>
        private Dictionary<int, List<List<int>>> stopProcessing; // [deviceCount] : [maxBatchesPositions x jobCount]

        public static Visualizer viz;
        private List<SheduleElement> _rWithTime;

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
            viz = new Visualizer(deviceCount, matrixR[0].Count);
        }

        /// <summary>
        /// Данный конструктор вовзращает экземпляр данного класса на основе переданной матрицы A
        /// </summary>
        /// <param name="matrixA">Матрица составов партий - матрица А</param>
        public Shedule(List<List<int>> matrixA)
        {
            InitMatrixR(matrixA);
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
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SheduleElement> RetyrnR()
        {
            _rWithTime = new List<SheduleElement>();
            for (int batchIndex = 0; batchIndex < stopProcessing[stopProcessing.Count - 1].Count; batchIndex++)
            {

                // Получаем индекс ненулевого элемента в строке
                var dataType = ReturnRDataType(batchIndex);
                _rWithTime.Add(new SheduleElement(matrixR[batchIndex][dataType], dataType, stopProcessing[stopProcessing.Count - 1][batchIndex]));
            }
            return _rWithTime;
        }


        /// <summary>
        /// Данная функция выполняет инициализацию матриц _startProcessing и _endProcessing в зависимости от матрицы matrixR
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
                        _timeConstructShedule = stopProcessing[device][batchIndex][job];
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
                int dataType = ReturnRDataType(batchIndex);
                rMatrix.AddNode(dataType + 1, matrixR[batchIndex][dataType]);
            }

            // Выполяем инициализацию матрицы P
            List<List<int>> pMatr = ListUtils.InitMatrixInt(matrixR[0].Count, matrixR.Count, 0);

            // Для каждой позиции в последовательности выполняем перебор
            for (int batchIndex = 0; batchIndex < matrixR.Count; batchIndex++)
            
                // Для каждого типа данных выполняем перебор
                for (int dataType = 0; dataType < matrixR[batchIndex].Count; dataType++)
                
                    // Если значение в позиции и типа не равно 0, инвертируем его в новой матрице pMatr
                    if (matrixR[batchIndex][dataType] != 0)
                        pMatr[dataType][batchIndex] = 1;

            // Инициализируем матрицу P
            Matrix pMatrix = new Matrix(pMatr);

            // Инициализируем матрицу переналадки приборов
            TreeDimMatrix timeChangeover = new TreeDimMatrix(changeoverTime);


            TreeDimMatrix tnMatrix = CalculationService.CalculateTnMatrix(rMatrix, pMatrix, proccessingTimeMatrix, timeChangeover, bufferSize);

            //if (viz == null)
            //{
            //    viz = new Visualizer(deviceCount, countType);
            //}
            //else
            //{
            //    viz.CreateExcelAppList(deviceCount, countType);
            //}

            //viz.Visualize(tnMatrix, timeProcessing, rMatrix);

            TreeDimMatrixNode lastNode = tnMatrix.treeDimMatrix.Last();
            int count = lastNode.time;
            int type = rMatrix[lastNode.fromDataType].dataType;

            int value = proccessingTimeMatrix[lastNode.device-1, type-1];

            _timeConstructShedule = count + value;
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
        /// 
        /// </summary>
        /// <returns></returns>
        public void ConstructShedule()
        {
            CalculateShedule();
            var matrixRCopy = ListUtils.MatrixIntDeepCopy(matrixR);
            var tempTime = _timeConstructShedule;

            for (var batchIndex_i = 0; batchIndex_i < matrixR.Count - 1; batchIndex_i++)
            {
                for (var batchIndex_j = batchIndex_i + 1; batchIndex_j < matrixR.Count; batchIndex_j++)
                
                    // Ранее использовалась функция "ChangeColum(i, j);", которая в результате выполняла перестановку строку
                    // Выполяем перестановку местами строки i и j
                    ListUtils.MatrixIntRowSwap(matrixR, batchIndex_i, batchIndex_j);
                
                CalculateShedule();
                if (tempTime >= _timeConstructShedule) continue;

                // TODO: Данная строчка может являеться не корректной, так как выполняет поверхностое
                // копирования и после первого прохода будет ссылаться на одну и ту же матрицу
                matrixR = matrixRCopy;
                _timeConstructShedule = tempTime;
            }
            
        }

        /// <summary>
        /// Данная функция выполняет построение расписания с буфером
        /// </summary>
        /// <param name="bufferSize">Размер буфера</param>
        /// <param name="dataTypesCount">Количество типов данных</param>
        /// <returns></returns>
        public void ConstructSheduleWithBuffer(int bufferSize, int dataTypesCount)
        {
            CalculateSheduleWithBufer(bufferSize, dataTypesCount);
            var tempR = ListUtils.MatrixIntDeepCopy(matrixR);
            var tempTime = _timeConstructShedule;

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

                // Если результат перестановки по времени хуже 
                if (tempTime >= _timeConstructShedule)
                    continue;
                
                // TODO: Данная строчка может являеться не корректной, так как выполняет поверхностое
                // копирования и после первого прохода будет ссылаться на одну и ту же матрицу
                matrixR = tempR;
                _timeConstructShedule = tempTime;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tz"></param>
        /// <param name="crit"></param>
        /// <returns></returns>
        public int GetTimeWithCriterium(int tz, out int crit)
        {

            // Данная переменная необходима для подсчёта времени выполнения всеъ заданий в системе
            var proccessingTime = 0;
            ConstructShedule();

            // Для каждого прибора высчитываем время выполнения
            for (int device = 0; device < deviceCount; device++)
            {
                for (int numberBatch = 0; numberBatch < startProcessing[device].Count; numberBatch++)
                {
                    for (int numberWork = 0; numberWork < startProcessing[device][numberBatch].Count; numberWork++)
                    {
                        proccessingTime += stopProcessing[device][numberBatch][numberWork] - startProcessing[device][numberBatch][numberWork];
                    }
                }

                proccessingTime -= startProcessing[device][0][0];
            }

            crit = (tz * deviceCount) - proccessingTime;
            return _timeConstructShedule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetTime()
        {
            //ConstructShedule();
            return _timeConstructShedule;
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

        #endregion
    }
}
