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
        /// </summary>
        public static List<List<List<int>>> changeoverTime;

        /// <summary>
        /// Матрица количества данных i-ых типов в партиях,
        /// занимающих в последовательностях pi_l j-ые позиции
        /// Данная матрица имеет представление, где в строке все элементы 0,
        /// кроме одно например для матрицы 3x3:
        /// [ [1,0,0], [0,1,0], [0,0,1] ]
        /// TODO: Данная реализация подразумеваем перевернутую матрицу R, где количество строк, это возможные позиции в последовательность, а количество столбцов, это типы данных, необходимо будет исправить данную оплошность
        /// </summary>
        private List<List<int>> matrixR;

        public static Visualizer viz;
        private int _timeConstructShedule;
        private List<List<List<int>>> _startProcessing; // [deviceCount x ??? x ???]
        private List<List<List<int>>> _endProcessing; // [deviceCount x ??? x ???]
        private List<SheduleElement> _rWithTime;

        /// <summary>
        /// Данный конструктор вовзращает экземпляр данного класса на основе переданного параметра матрицы R
        /// </summary>
        /// <param name="matrixR">Матрица R - количества данных i-ых типов в партиях занимающих в последовательности pi_l j-е позиции</param>
        /// <param name="deviceCount">Количество приборов в конвейерной системе</param>
        public Shedule(List<List<int>> matrixR, int deviceCount)
        {
            this.matrixR = matrixR;
            Shedule.deviceCount = deviceCount;
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

            // Инициализируем новую матрицу R [dataTypesCount x max(mi)]
            matrixR = new List<List<int>>();

            // Подсчитываем количество элементов в каждой строке (sum(mi))
            // batchesList представляет из себя множество партий с одним типом
            // например, скажем для типа 1, это может быть [10, 2]. Тогда 
            // batchesList.Count = mi = 2 - Количество партий данных 1-ого типа
            // Тогда сумма всех количеств партий (batchPositionCount), представляет из себя
            // количество возможных позиций партий в последовательности pi_l
            var batchPositionCount = matrixA.Sum(batchesList => batchesList.Count);

            // Объявляем максимальный количество партий (max(mi))
            var maxBatchCount = 0;

            // Высчитываем максимальный размер колонки, 
            foreach (var mi in matrixA)
                maxBatchCount = (mi.Count > maxBatchCount) ? mi.Count : maxBatchCount;

            // Пробегаемся по всем возможным позициям партий в матрице R
            for (var batchPosition = 0; batchPosition < batchPositionCount; batchPosition++)
            {

                // Инициализируем каждую строку матрицы R, как возможную позицию партии
                matrixR.Add(new List<int>());

                // Каждой строке матрицы R (позиции batchPosition) присваиваем 0, для кадого столбца
                foreach (var mi in matrixA)
                    matrixR[batchPosition].Add(0);
            }
            
            // Данная переменная описывает позицию в последовательности pi_l
            var position = 0;

            // Пробегаемся по всем возможным пакетам - колонкам(max(mi)) матрицы R
            for (var batchIndex = 0; batchIndex < maxBatchCount; batchIndex++)

                // Пробегаемся по всем типам данных
                for (var dataType = 0; dataType < matrixA.Count; dataType++)

                    // Если количество партий данных i-ого типа (mi) > (h) индекс пакета
                    if (matrixA[dataType].Count > batchIndex)

                        // Выполяем присвоение
                        matrixR[position++][dataType] = matrixA[dataType][batchIndex];

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<SheduleElement> RetyrnR()
        {
            _rWithTime = new List<SheduleElement>();
            for (int i = 0; i < _endProcessing[_endProcessing.Count - 1].Count; i++)
            {

                // Получаем индекс ненулевого элемента в строке
                var index = ReturnRIndex(i);
                _rWithTime.Add(new SheduleElement(matrixR[i][index], index, _endProcessing[_endProcessing.Count - 1][i]));
            }
            return _rWithTime;
        }


        /// <summary>
        /// 
        /// </summary>
        private void CalculateShedule()
        {
            // Выполняем инициализацию трёхмерных матриц
            _startProcessing = new List<List<List<int>>>(deviceCount);
            _startProcessing.AddRange(Enumerable.Repeat(new List<List<int>>(matrixR.Count), deviceCount));
            _endProcessing = new List<List<List<int>>>(deviceCount);
            _endProcessing.AddRange(Enumerable.Repeat(new List<List<int>>(matrixR.Count), deviceCount));

            // Для каждого прибора выполняем перебор
            for (var device = 0; device < deviceCount; device++)

                // Для каждой возможной позиции выполняем обработку
                for (var position = 0; position < matrixR.Count; position++)//количество партий
                {

                    // Получаем индекс ненулевого элемента в строке
                    var index = ReturnRIndex(position);

                    // Получаем элемент по ненулевому индексу в матрице R
                    var elem = matrixR[position][index];
                    if (elem == -1)
                        elem = 1;
                    
                    // Для временных матриц выполняем иницаиализацию 
                    _startProcessing[device].Add(new List<int>(elem));
                    _endProcessing[device].Add(new List<int>(elem));
                    for (var p = 0; p < elem; p++)//количество требований
                    {
                        _startProcessing[device][position].Add(0);
                        _endProcessing[device][position].Add(0);
                    }
                }
            
            var previousPosition = 0;
            var previousBatch = 0;
            var previousIndex = 0;

            // Для каждого прибора выполняем высчитывание времён конца и начала обработки партий
            for (var device = 0; device < deviceCount; device++)
            {

                // Для каждой возможной позиции выполняем обработку
                for (var position = 0; position < matrixR.Count; position++)
                {

                    // Получаем индекс ненулевого элемента в строке
                    var index = ReturnRIndex(position);

                    // Перебираем все партии
                    for (var batch = 0; batch < matrixR[position][index]; batch++)
                    {

                        // Для каждого требования высчитываем время переналадки
                        var timeToSwitch = (index == previousIndex && position != 0) ? 0 : changeoverTime[0][previousIndex][index];

                        // Выполяем высчитывания времён начала обработки пакета batch на приборе device в позиции position
                        if (device > 0)

                            // Время начала выполнения партии batch на приборе device в позиции position
                            // высчитываем, как время конца выполнения предыдущей партии previousBatch на приборе device в предыдущей позиции 
                            //                + время переналадки на выполнение партии данного типа, которое может быть равно 0 или
                            // как время конца выполнения данной партии batch на предыдущем приборе device - 1 в текущей позиции position
                            _startProcessing[device][position][batch] = Math.Max(_endProcessing[device][previousPosition][previousBatch] + timeToSwitch, _endProcessing[device - 1][position][batch]);
                        
                        else

                            // Время начала выполнения партии batch на приборе device в позиции position
                            // высчитываем, как время конца выполнения предыдущей партии previousBatch на приборе device в предыдущей позиции 
                            //                + время переналадки на выполнение партии данного типа, которое может быть равно 0.
                            _startProcessing[device][position][batch] = _endProcessing[device][previousPosition][previousBatch] + timeToSwitch;

                        _endProcessing[device][position][batch] = _startProcessing[device][position][batch] + proccessingTime[device][index];
                        _timeConstructShedule = _endProcessing[device][position][batch];
                        previousPosition = position;
                        previousBatch = batch;
                        previousIndex = index;
                    }
                }

                previousPosition = 0;
                previousBatch = 0;
                previousIndex = 0;
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
            for (int position = 0; position < matrixR.Count; position++)
            {
                int dataType = ReturnRIndex(position);
                rMatrix.AddNode(dataType + 1, matrixR[position][dataType]);
            }

            // Выполяем инициализацию двумерного списка P
            List<List<int>> pMatr = new List<List<int>>(matrixR[0].Count);

            // Для каждого типа данных выполяем перебор
            for (int dataType = 0; dataType < matrixR[0].Count; dataType++)
            {

                // Инициализируем временную матрицу
                List<int> tmp = new List<int>();
                for (int position = 0; position < matrixR.Count; position++)
                    tmp.Add(0);
                
                // В каждую строку матрицы P добавляем элементы из 0
                pMatr.Add(tmp);
            }

            // Для каждой позиции в последовательности выполняем перебор
            for (int position = 0; position < matrixR.Count; position++)
            
                // Для каждого типа данных выполняем перебор
                for (int dataType = 0; dataType < matrixR[position].Count; dataType++)
                
                    // Если значение в позиции и типа не равно 0, инвертируем его в новой матрице pMatr
                    if (matrixR[position][dataType] != 0)
                        pMatr[dataType][position] = 1;
                    

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
        /// Данная функция возвращает индекс ненулевого элемента в матрице R в позиции position
        /// </summary>
        /// <param name="position">Позиция в последовательности</param>
        /// <returns>Индекс ненулевого элемента или -1 в случае неудачи</returns>
        public int ReturnRIndex(int position)
        {
            for (var i = 0; i < matrixR[position].Count; i++)
                if (matrixR[position][i] > 0)
                    return i;
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<List<int>> ConstructShedule()
        {
            var tempTime = 9999999;
            CalculateShedule();
            var tempR = ListUtils.MatrixIntDeepCopy(matrixR);
            tempTime = _timeConstructShedule;
            for (var i = 0; i < matrixR.Count - 1; i++)
            {
                for (var j = i + 1; j < matrixR.Count; j++)
                
                    // Ранее использовалась функция "ChangeColum(i, j);", которая в результате выполняла перестановку строку
                    // Выполяем перестановку местами строки i и j
                    ListUtils.MatrixIntRowSwap(matrixR, i, j);
                
                CalculateShedule();
                if (tempTime >= _timeConstructShedule) continue;
                matrixR = tempR;
                _timeConstructShedule = tempTime;
            }
            return matrixR;
        }

        /// <summary>
        /// Данная функция выполняет построение расписания с буфером
        /// </summary>
        /// <param name="bufferSize">Размер буфера</param>
        /// <param name="dataTypesCount">Количество типов данных</param>
        /// <returns></returns>
        public List<List<int>> ConstructSheduleWithBuffer(int bufferSize, int dataTypesCount)
        {
            var tempTime = 9999999;
            CalculateSheduleWithBufer(bufferSize, dataTypesCount);
            var tempR = ListUtils.MatrixIntDeepCopy(matrixR);
            tempTime = _timeConstructShedule;

            // Выполяем все возможные перестановки позиций
            // Для всех позиций от 0 до matrixR.Count - 1 выполняем перебор
            for (var pos1 = 0; pos1 < matrixR.Count - 1; pos1++)
            {

                // Для всех позиций от pos1 + 1 до matrixR.Count выполняем все перестановку строк (представляющие из себя позицию)
                for (var pos2 = pos1 + 1; pos2 < matrixR.Count; pos2++)
                
                    // Ранее использовалась функция "ChangeColum(i, j);", которая в результате выполняла перестановку строку
                    // Выполяем перестановку местами строки i и j
                    ListUtils.MatrixIntRowSwap(matrixR, pos1, pos2);
                
                // Выполяем высчитывание расписания с буфером для всех возможных последовательностей
                CalculateSheduleWithBufer(bufferSize, dataTypesCount);

                // Если результат перестановки по времени хуже 
                if (tempTime >= _timeConstructShedule)
                    continue;
                matrixR = tempR;
                _timeConstructShedule = tempTime;
            }
            return matrixR;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tz"></param>
        /// <param name="crit"></param>
        /// <returns></returns>
        public int GetTimeWithCriterium(int tz, out int crit)
        {
            var criterier = 0;
            ConstructShedule();

            for (int numberPocess = 0; numberPocess < deviceCount; numberPocess++)
            {
                for (int numberBatch = 0; numberBatch < _startProcessing[numberPocess].Count; numberBatch++)
                {
                    for (int numberWork = 0; numberWork < _startProcessing[numberPocess][numberBatch].Count; numberWork++)
                    {
                        criterier += _endProcessing[numberPocess][numberBatch][numberWork] - _startProcessing[numberPocess][numberBatch][numberWork];
                    }
                }

                criterier -= _startProcessing[numberPocess][0][0];
            }

            crit = (tz * deviceCount) - criterier;
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
