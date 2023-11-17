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
        /// Данная статическая переменная определяет длину конвейерной ленты по сути количество приборов
        /// </summary>
        public static int conveyorLenght;

        public static Visualizer viz;

        private List<List<int>> _r;
        public static List<List<int>> Treatment;
        public static List<List<List<int>>> Switching;
        private int _timeConstructShedule;
        private List<List<List<int>>> _startProcessing;
        private List<List<List<int>>> _endProcessing;
        private List<SheduleElement> _rWithTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="l"></param>
        public Shedule(List<List<int>> r, int l)
        {
            this._r = r;
            conveyorLenght = l;
            viz = new Visualizer(l, r[0].Count);
        }

        /// <summary>
        /// Формирование матрицы для передачи её в модуль расписания
        /// </summary>
        /// <param name="m">входная матрица А</param>
        /// <returns>сформированная матрица для уровня расписания</returns>
        private List<List<int>> GenerateR(IReadOnlyList<List<int>> m)
        {
            var result = new List<List<int>>();
            var summ = m.Sum(t => t.Count);
            var maxColumn = 0;
            for (var j = 0; j < summ; j++)
            {
                result.Add(new List<int>());
                for (var i = 0; i < m.Count; i++)
                {
                    result[j].Add(0);
                }
            }
            for (var i = 0; i < m.Count; i++)
            {
                if (m[i].Count > maxColumn)
                {
                    maxColumn = m[i].Count;
                }
            }
            var ind = 0;
            for (var j = 0; j < maxColumn; j++)
            {
                for (var i = 0; i < m.Count; i++)
                {
                    if (m[i].Count > j)
                    {
                        result[ind][i] = m[i][j];
                        ind++;
                    }
                }
            }
            return result;
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
        /// 
        /// </summary>
        /// <param name="r"></param>
        public Shedule(List<List<int>> r)
        {
            _r = GenerateR(r);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateShedule()
        {
            _startProcessing = new List<List<List<int>>>();
            _endProcessing = new List<List<List<int>>>();
            for (var i = 0; i < conveyorLenght; i++)//количество приборов
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
            for (var i = 0; i < conveyorLenght; i++)
            {
                for (var j = 0; j < _r.Count; j++)
                {
                    var index = ReturnRIndex(j);


                    for (var k = 0; k < _r[j][index]; k++)
                    {
                        var timeToSwitch = (index == xx && j != 0) ? 0 : Switching[0][xx][index];
                        if (i > 0)
                        {
                            _startProcessing[i][j][k] = Math.Max(_endProcessing[i][yy][zz] + timeToSwitch, _endProcessing[i - 1][j][k]);
                        }
                        else
                        {
                            _startProcessing[i][j][k] = _endProcessing[i][yy][zz] + timeToSwitch;
                        }
                        _endProcessing[i][j][k] = _startProcessing[i][j][k] + Treatment[i][index];
                        _timeConstructShedule = _endProcessing[i][j][k];
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

        private void CalculateSheduleWithBufer(int b, int deviceCount, int countType)
        {
            Matrix timeProcessing = new Matrix(Treatment);

            RMatrix rMatrix = new RMatrix(countType);

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

            Matrix pMatrix = new Matrix(pMatr);

            TreeDimMatrix timeChangeover = new TreeDimMatrix(deviceCount);
            for (int device = 0; device < Switching.Count; device++)
            {
                for (int fromType = 0; fromType < Switching[device].Count; fromType++)
                {
                    for (int toType = 0; toType < Switching[device][fromType].Count; toType++)
                    {
                        var val = Switching[device][fromType][toType];
                        if (val != 0)
                        {
                            timeChangeover.AddNode(device + 1, fromType + 1, toType + 1, Switching[device][fromType][toType]);
                        }
                    }
                }
            }

            TreeDimMatrix tnMatrix = CalculationService.CalculateTnMatrix(rMatrix, pMatrix, timeProcessing, timeChangeover, b);

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
            int count = lastNode.Count;
            int type = rMatrix.Find(lastNode.Type).Type;

            int value = timeProcessing.GetItem(lastNode.DeviceNumber, type);

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
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        public List<List<int>> ConstructSheduleWithBuffer(int b, int countType)
        {
            var tempTime = 9999999;
            CalculateSheduleWithBufer(b, conveyorLenght, countType);
            var tempR = CopyMatrix(_r);
            tempTime = _timeConstructShedule;
            for (var i = 0; i < _r.Count - 1; i++)
            {
                for (var j = i + 1; j < _r.Count; j++)
                {
                    ChangeColum(i, j);
                }
                CalculateSheduleWithBufer(b, conveyorLenght, countType);
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

            for (int numberPocess = 0; numberPocess < conveyorLenght; numberPocess++)
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

            crit = (tz * conveyorLenght) - criterier;
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
