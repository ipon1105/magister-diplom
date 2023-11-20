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
        /// </summary>
        public static int deviceCount;

        /// <summary>
        /// Данный двумерный список представляет из себя матрицу выполнения задания на приборе l типа i
        /// </summary>
        public static List<List<int>> proccessingTime;

        /// <summary>
        /// Данный трёхмерный список представляет из себя матрицу переналадки прибора l с задания i на задание j
        /// </summary>
        public static List<List<List<int>>> changeoverTime;


        public static Visualizer viz;
        private List<List<int>> _r;
        private int _timeConstructShedule;
        private List<List<List<int>>> _startProcessing;
        private List<List<List<int>>> _endProcessing;
        private List<SheduleElement> _rWithTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrixR">Матрица R - количества данных i-ых типов в партиях занимающих в последовательности pi_l j-е позиции</param>
        /// <param name="deviceCount">Количество приборов в конвейерной системе</param>
        public Shedule(List<List<int>> matrixR, int deviceCount)
        {
            this._r = matrixR;
            Shedule.deviceCount = deviceCount;
            viz = new Visualizer(deviceCount, matrixR[0].Count);
        }

        /// <summary>
        /// Формирование матрицы для передачи её в модуль расписания
        /// </summary>
        /// <param name="m">входная матрица А</param>
        /// <returns>сформированная матрица для уровня расписания</returns>
        private List<List<int>> GenerateMatrixR(IReadOnlyList<List<int>> m)
        {

            // Инициализируем матрицу R [dataTypesCount x max(mi)]
            var matrixR = new List<List<int>>();

            // Подсчитываем количество элементов в каждой строке
            var elementsSize = m.Sum(mi => mi.Count);

            // Объявляем максимальный размер колонки
            var maxColumnSize = 0;

            // Высчитываем максимальный размер колонки
            foreach (var mi in m)
                maxColumnSize = (mi.Count > maxColumnSize) ? mi.Count : maxColumnSize;

            for (var element = 0; element < elementsSize; element++)
            {

                // Инициализируем каждую строку матрицы R
                matrixR.Add(new List<int>());

                // Каждой строке матрицы R присваиваем 0, для кадого столбца
                foreach (var mi in m)
                    matrixR[element].Add(0);

            }
            
            
            var ind = 0;
            for (var j = 0; j < maxColumnSize; j++)
            {
                for (var i = 0; i < m.Count; i++)
                {
                    if (m[i].Count > j)
                    {
                        matrixR[ind][i] = m[i][j];
                        ind++;
                    }
                }
            }
            return matrixR;
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
                var ind = ReturnRIndex(i);
                _rWithTime.Add(new SheduleElement(_r[i][ind], ind, _endProcessing[_endProcessing.Count - 1][i]));
            }
            return _rWithTime;
        }
        
        /// <summary>
        /// Данный конструктор вовзращает экземпляр данного класса на основе переданного параметра r
        /// </summary>
        /// <param name="r">Матрица количества данных</param>
        public Shedule(List<List<int>> r)
        {
            _r = GenerateMatrixR(r);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateShedule()
        {
            _startProcessing = new List<List<List<int>>>();
            _endProcessing = new List<List<List<int>>>();
            for (var i = 0; i < deviceCount; i++)//количество приборов
            {
                _startProcessing.Add(new List<List<int>>());
                _endProcessing.Add(new List<List<int>>());
                for (var k = 0; k < _r.Count; k++)//количество партий
                {
                    var ind = ReturnRIndex(k);
                    var elem = _r[k][ind];
                    if (elem == -1)
                    {
                        elem = 1;
                    }
                    _startProcessing[i].Add(new List<int>());
                    _endProcessing[i].Add(new List<int>());
                    for (var p = 0; p < elem; p++)//количество требований
                    {
                        _startProcessing[i][k].Add(0);
                        _endProcessing[i][k].Add(0);
                    }
                }
            }
            var yy = 0;
            var zz = 0;
            var xx = 0;
            for (var device = 0; device < deviceCount; device++)
            {
                for (var j = 0; j < _r.Count; j++)
                {
                    var index = ReturnRIndex(j);


                    for (var k = 0; k < _r[j][index]; k++)
                    {
                        var timeToSwitch = (index == xx && j != 0) ? 0 : changeoverTime[0][xx][index];
                        if (device > 0)
                        {
                            _startProcessing[device][j][k] = Math.Max(_endProcessing[device][yy][zz] + timeToSwitch, _endProcessing[device - 1][j][k]);
                        }
                        else
                        {
                            _startProcessing[device][j][k] = _endProcessing[device][yy][zz] + timeToSwitch;
                        }
                        _endProcessing[device][j][k] = _startProcessing[device][j][k] + proccessingTime[device][index];
                        _timeConstructShedule = _endProcessing[device][j][k];
                        yy = j;
                        zz = k;
                        xx = index;
                    }
                }
                yy = 0;
                zz = 0;
                xx = 0;
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

            for (int column = 0; column < _r.Count; column++)
            {
                for (int row = 0; row < _r[column].Count; row++)
                {
                    var val = _r[column][row];
                    if(val != 0)
                    {
                        rMatrix.AddNode(row + 1, val);
                        break;
                    }
                }
            }

            List<List<int>> pMatr = new List<List<int>>();
            for (int row = 0; row < _r[0].Count; row++)
            {
                List<int> tmp = new List<int>();
                for (int column = 0; column < _r.Count; column++)
                {
                    tmp.Add(0);
                }
                pMatr.Add(tmp);
            }
            for (int column = 0; column < _r.Count; column++)
            {
                for (int row = 0; row < _r[column].Count; row++)
                {
                    if (_r[column][row] != 0)
                    {
                        pMatr[row][column] = 1;
                    }
                }
            }

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
            int type = rMatrix[lastNode.fromDataType].Type;

            int value = proccessingTimeMatrix[lastNode.device-1, type-1];

            _timeConstructShedule = count + value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        public int ReturnRIndex(int j)
        {
            for (var i = 0; i < _r[j].Count; i++)
            {
                if (_r[j][i] > 0)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inMatrix"></param>
        /// <returns></returns>
        private static List<List<int>> CopyMatrix(IReadOnlyList<List<int>> inMatrix)
        {
            var ret = new List<List<int>>();
            for (var i = 0; i < inMatrix.Count; i++)
            {
                ret.Add(new List<int>());
                for (var j = 0; j < inMatrix[i].Count; j++)
                {
                    ret[i].Add(inMatrix[i][j]);
                }
            }
            return ret;
        }

        /// <summary>
        /// Данная функция выполняет 
        /// </summary>
        /// <param name="ind1"></param>
        /// <param name="ind2"></param>
        private void ChangeColum(int ind1, int ind2)
        {
            var indd1 = 0;
            var indd2 = 0;
            for (var i = 0; i < _r[ind1].Count; i++)
            {
                if (_r[ind1][i] > 0)
                {
                    indd1 = i;
                }
            }
            for (var i = 0; i < _r[ind2].Count; i++)
            {
                if (_r[ind2][i] > 0)
                {
                    indd2 = i;
                }
            }
            var temp = _r[ind1][indd1];
            _r[ind1][indd1] = 0;
            _r[ind1][indd2] = _r[ind2][indd2];
            _r[ind2][indd2] = 0;
            _r[ind2][indd1] = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<List<int>> ConstructShedule()
        {
            var tempTime = 9999999;
            CalculateShedule();
            var tempR = CopyMatrix(_r);
            tempTime = _timeConstructShedule;
            for (var i = 0; i < _r.Count - 1; i++)
            {
                for (var j = i + 1; j < _r.Count; j++)
                {
                    ChangeColum(i, j);
                }
                CalculateShedule();
                if (tempTime >= _timeConstructShedule) continue;
                _r = tempR;
                _timeConstructShedule = tempTime;
            }
            return _r;
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
            var tempR = CopyMatrix(_r);
            tempTime = _timeConstructShedule;
            for (var i = 0; i < _r.Count - 1; i++)
            {
                for (var j = i + 1; j < _r.Count; j++)
                {
                    ChangeColum(i, j);
                }
                CalculateSheduleWithBufer(bufferSize, dataTypesCount);
                if (tempTime >= _timeConstructShedule) continue;
                _r = tempR;
                _timeConstructShedule = tempTime;
            }
            return _r;
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
    }
}
