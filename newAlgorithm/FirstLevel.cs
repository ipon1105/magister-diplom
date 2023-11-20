﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using magisterDiplom.Utils;

namespace newAlgorithm
{
    class FirstLevel
    {

        /// <summary>
        /// Данная переменная определяет фиксированные ли партии
        /// </summary>
        private readonly bool isFixedBatches;

        /// <summary>
        /// Данная переменная определяет количество типов данных
        /// </summary>
        private readonly int dataTypesCount;

        /// <summary>
        /// Данная переменная определяет вектор данных для интерпритации типов данных
        /// </summary>
        private readonly List<int> _i;                  // Вектор интерпритируемых типов данных
        
        private List<List<int>> _ai;                    // Буферизированная матрица составов партий требований на k+1 шаге 
        private List<List<int>> _abuf;                  // Буферизированная матрица составов партий требований на k+1 шаге
        private List<List<List<int>>> _a1;              // Матрица составов партий требований на k+1 шаге 
        private List<List<List<int>>> _a2;              // Матрица составов партий требований фиксированного типа
        public List<List<int>> _a { get; private set; }                     // Матрица составов партий требований на k шаге
        
        /// <summary>
        /// Данная переменная определяет вектор количества требований для каждого типа данных
        /// </summary>
        private readonly List<int> batchCountList;        // Начальное количество требований для каждого типа данных
        private int _f1;                                // Критерий текущего решения для всех типов
        private int _f1Buf;                             // Критерий текущего решения для всех типов
        private List<int> _nTemp;
        private bool _typeSolutionFlag;


        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="dataTypesCount">Количество типов данных</param>
        /// <param name="batchCountList">Вектор длиной dataTypesCount, каждый элемент это количество элементов в партии</param>
        /// <param name="isFixedBatches">Являются ли партии фиксированными</param>
        public FirstLevel(int dataTypesCount, List<int> batchCountList, bool isFixedBatches)
        {
            this.dataTypesCount = dataTypesCount;
            this.batchCountList = batchCountList;
            this.isFixedBatches = isFixedBatches;
            _i = new List<int>(this.dataTypesCount);
        }

        /// <summary>
        /// Алгоритм формирования фиксированных партий
        /// Данная функция выполняет инициализацию матрицы _a и вектора _i.
        /// Вектор _i инициализируется, как вектор из 1 длиной dataTypesCount.
        /// Матрица _a инициализируется, как матрица [dataTypesCount x 1].
        /// </summary>
        public void GenerateFixSolution()
        {
            // Инициализируем строки матрицы A
            _a = new List<List<int>>();

            // Для каждого типа данных выполняем обработку
            for (var dataType = 0; dataType < dataTypesCount; dataType++)
            {

                // Добавляем в вектор _i элемент 1 в конец списка
                _i.Add(1);

                // Инициализируем столбцы матрицы A
                _a.Add(new List<int>());

                // Для каждой строки матрицы A добавляем вектор количеств элементов в партии
                _a[dataType].Add(batchCountList[dataType]);
            }
        }

        /// <summary>
        /// Алгоритм формирования начальных решений по составам партий всех типов
        /// </summary>
        public void GenerateStartSolution()
        {
            const int claim = 2;
            _a = new List<List<int>>();
            for (var dataType = 0; dataType < dataTypesCount; dataType++)
            {
                _i.Add(1);
                _a.Add(new List<int>());
                _a[dataType].Add(batchCountList[dataType] - claim);
                _a[dataType].Add(claim);
            }
            for (var dataType = 0; dataType < dataTypesCount; dataType++)
            {
                if (_a[dataType][0] < 2 || _a[dataType][0] < _a[dataType][1])
                {
                    _a[dataType].Clear();
                    _a[dataType].Add(batchCountList[dataType]);
                    _i[dataType] = 0;
                }
            }
        }

        /// <summary>
        /// Функция проверки наличия оставшихся в расмотрении типов
        /// </summary>
        /// <param name="type">Список всех рассматриваемых типов</param>
        /// <returns>True, если в наличие еще есть рассматриваемые типы, иначе False</returns>
        private bool CheckType(IReadOnlyList<int> type)
        {
            // Для каждого типа данных выполняем проверку на не нулевое количество типов
            for (var dataType = 0; dataType < dataTypesCount; dataType++)

                // Проверяем количество типов
                if (type[dataType] > 0)

                    // Если в списке количество больше 0, значит типы ещё есть в расмотрении
                    return true;

            // Все типы были расмотрены
            return false;
        }


        /// <summary>
        /// Построчное формирование матрицы промежуточного решени
        /// </summary>
        /// <param name="type">тип рассматриваемого решения</param>
        /// <param name="ind2">индекс подставляемого решения</param>
        /// <returns>матрица А с подставленным новым решением в соответствующий тип</returns>
        private List<List<int>> SetTempAFromA2(int type, int ind2)
        {
            var result = ListUtils.MatrixDeepCopy(_a);
            if (ind2 < _a2[type].Count)
                result[type] = ListUtils.VectorDeepCopy(_a2[type][ind2]);
            return result;
        }


        /// <summary>
        /// Функция получения неповторяющихся решений в матрице А2 на шаге 9
        /// </summary>
        /// <param name="inMatrix">входная матрица сформированных решений</param>
        /// <returns>Новые решения без повторений</returns>
        public List<List<int>> SortedMatrix(List<List<int>> inMatrix)
        {
            var temp = ListUtils.MatrixDeepCopy(inMatrix);
            //Удаление повторяющихся строк
            var countLoops = 0;
            while (true)
            {
                for (var i = 1; i < temp.Count; i++)
                {
                    var lastIndexForDelete = temp.FindLastIndex(delegate(List<int> inList)
                    {
                        if (inList.Count != temp[i].Count)
                        {
                            return false;
                        }
                        var countFind = inList.Where((t, k) => t == temp[i][k]).Count();
                        return countFind == inList.Count ? true : false;
                    });
                    if (lastIndexForDelete == i) continue;
                    temp.RemoveAt(lastIndexForDelete);
                    inMatrix.RemoveAt(lastIndexForDelete);
                }
                countLoops++;
                if (countLoops > 100)
                    break;
            }
            return inMatrix;
        }

        
        /// <summary>
        /// Удаление повторений новых решений совпадающих с A1
        /// </summary>
        /// <param name="inMatrix">матрица новых решений</param>
        /// <param name="type">рассматриваемый тип</param>
        /// <returns>Полученные новые решения</returns>
        private List<List<int>> CheckMatrix(List<List<int>> inMatrix, int type)
        {

            foreach (var row2 in _a1[type])
            {
                foreach (var rowMatrix in inMatrix.ToList())
                {
                    if (rowMatrix.Zip(row2, (a, b) => new { a, b }).All(pair => pair.a == pair.b))
                    {
                        inMatrix.Remove(rowMatrix);
                    }
                }
            }
            return inMatrix;
        }


        /// <summary>
        /// Формирование новых решений по составим партий текущего типа данных
        /// </summary>
        /// <param name="type">рассматриваемый тип</param>
        /// <returns>новые решения для этого типа</returns>
        private List<List<int>> NewData(int type)
        {
            var result = new List<List<int>>();
            foreach(var row in _a1[type])
            {
                for (var j = 1; j < row.Count; j++)
                {
                    result.Add(ListUtils.VectorDeepCopy(row));
                    if (row[0] <= row[j] + 1) continue;
                    result[result.Count - 1][0]--;
                    result[result.Count - 1][j]++;
                }
                if (result[result.Count - 1][0] != row[0]) continue;
                {
                    var summ = row[0];
                    result[result.Count - 1].Add(2);
                    for (var j = 1; j < row.Count; j++)
                    {
                        summ += row[j];
                        result[result.Count - 1][j] = 2;
                    }
                    result[result.Count - 1][0] = summ - 2 * (result[result.Count - 1].Count - 1);
                }
            }
            var count = 0;
            while (true)
            {
                for (var i = 1; i < result.Count; i++)
                {
                    for (var j = 1; j < result[i].Count; j++)
                    {
                        if (result[i][j] <= result[i][j - 1]) continue;
                        result.Remove(result[i]);
                        break;
                    }
                }
                count++;
                if (count > 3)
                    break;
            }
            
            result = SortedMatrix(result);
            result = CheckMatrix(result,type);
            return result;
        }


        /// <summary>
        /// Формирование новых решений по составим партий текущего типа данных
        /// </summary>
        /// <param name="m">матрица А для печати</param>
        /// <returns>строка с составами партий по типам</returns>
        private static string PrintA(IEnumerable<List<int>> m)
        {
            var result = "";
            foreach (var t in m)
            {
                for (var j = 0; j < t.Count - 1; j++)
                {
                    result += t[j] + ", ";
                }
                result += t[t.Count - 1] + ";\n";
            }
            return result;
        }


        /// <summary>
        /// Рекурсивная комбинация всех типов _a2 с фиксированным решением _a
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tempA"></param>
        /// <param name="type"></param>
        public void CombinationType(StreamWriter file, List<List<List<int>>> tempMatrix, int type, List<List<int>> tempM, ref bool solutionFlag )
        {
            if (type < dataTypesCount)
            {
                for (var variantOfSplitIndex = 0; variantOfSplitIndex < _a2[type].Count; variantOfSplitIndex++)
                {
                    List<List<int>> tempB = (tempM != null) ? tempB = ListUtils.MatrixDeepCopy(tempM) : tempB = new List<List<int>>();

                    tempB.Add(tempMatrix[type][variantOfSplitIndex]);
                    CombinationType(file, tempMatrix, type + 1, tempB, ref solutionFlag);
                }
            } else
            {
                var shedule = new Shedule(tempM);
                //shedule.ConstructShedule();
                shedule.ConstructSheduleWithBuffer(Form1.buff, dataTypesCount);
                var fBuf = shedule.GetTime();
                string s = ListUtils.MatrixIntToString(tempM, ", ", "", ";");
                file.Write(s + " " + fBuf);
                MessageBox.Show(s + " Время обработки " + fBuf);
                if (fBuf < _f1Buf)
                {
                    _abuf = ListUtils.MatrixDeepCopy(tempM);
                    solutionFlag = true;
                    _f1Buf = fBuf;
                    file.Write(" +");
                    return;
                }
                file.WriteLine();
            }
        }

        /// <summary>
        /// Алгоритм формирования решения по составам паритй всех типов данных
        /// </summary>
        public void GenetateSolutionForAllTypes(string fileName)
        {
            using (var file = new StreamWriter(fileName))
            {
                GenerateFixSolution();
                var shedule = new Shedule(_a);
                //shedule.ConstructShedule();
                shedule.ConstructSheduleWithBuffer(Form1.buff, dataTypesCount);
                _f1 = shedule.GetTime();

                MessageBox.Show(ListUtils.MatrixIntToString(_a, ", ", "", ";") + "Время обработки " + _f1);
                _f1Buf = _f1;
                file.WriteLine(_f1Buf);
                var maxA = ListUtils.MatrixDeepCopy(_a);
                _typeSolutionFlag = true;

                // Выполяем отчистку вектора _i и матрицы _a
                _i.Clear();
                _a.Clear();

                GenerateStartSolution();
                shedule = new Shedule(_a);
                //shedule.ConstructShedule();
                shedule.ConstructSheduleWithBuffer(Form1.buff, dataTypesCount);
                _f1 = shedule.GetTime();
                MessageBox.Show(PrintA(_a) + " Время обработки " + _f1);
                if (_f1 < _f1Buf)
                {
                    _abuf = ListUtils.MatrixDeepCopy(_a);
                    _typeSolutionFlag = true;
                    _f1Buf = _f1;
                    file.Write(" +");
                }
                if (!isFixedBatches)
                {

                    // До тех пор, поа не расмотрели все типы выполняем обработку
                    while (CheckType(_i))
                    {
                        // Буферезируем текущее решение для построение нового на его основе
                        _ai = ListUtils.MatrixDeepCopy(_a);
                        if (_typeSolutionFlag)
                        {
                            _a1 = new List<List<List<int>>>();
                            for (var dataType = 0; dataType < dataTypesCount; dataType++)
                            {
                                _a1.Add(new List<List<int>>());
                                _a1[dataType].Add(new List<int>());
                                _a1[dataType][0] = ListUtils.VectorDeepCopy(_a[dataType]);
                            }
                            _typeSolutionFlag = false;
                        }

                        var tempA = ListUtils.MatrixDeepCopy(_ai);
                        _abuf = ListUtils.MatrixDeepCopy(_ai);
                        _f1Buf = _f1;

                        // Для каждого типа и каждого решения в типе строим новое решение и проверяем его на критерий
                        // Строим A2 и параллельно проверяем критерий
                        _a2 = new List<List<List<int>>>();
                        string s;
                        file.WriteLine("окрестность 1 вида");
                        for (var dataType = 0; dataType < dataTypesCount; dataType++)
                        {
                            _a2.Add(new List<List<int>>());
                            if (_i[dataType] <= 0) continue;
                            _a2[dataType] = NewData(dataType);
                            for (var j = 0; j < _a2[dataType].Count; j++)
                            {
                                tempA = SetTempAFromA2(dataType, j);
                                shedule = new Shedule(tempA);
                                //shedule.ConstructShedule();
                                shedule.ConstructSheduleWithBuffer(Form1.buff, dataTypesCount);
                                var fBuf = shedule.GetTime();
                                s = PrintA(tempA);
                                file.Write(s + " " + fBuf);
                                MessageBox.Show(s + " Время обработки " + fBuf);                                    
                                if (fBuf < _f1Buf)
                                {
                                    _abuf = ListUtils.MatrixDeepCopy(tempA);
                                    _typeSolutionFlag = true;
                                    _f1Buf = fBuf;
                                    file.Write(" +");
                                }
                                file.WriteLine();
                            }
                        }
                        if (!_typeSolutionFlag)
                        {
                            file.WriteLine("комбинации типов");
                            CombinationType(file, _a2, 0, null, ref _typeSolutionFlag);
                        }

                        if (_typeSolutionFlag)
                        {
                            MessageBox.Show("Лучшее решение " + PrintA(_abuf) + " Время обработки " + _f1Buf);
                            _a = ListUtils.MatrixDeepCopy(_abuf);
                            _f1 = _f1Buf;
                        }
                        else
                        {
                            for (int dataType = 0; dataType < dataTypesCount; dataType++)
                            {
                                _a1[dataType] = ListUtils.MatrixDeepCopy(_a2[dataType]);
                                if (!_a1[dataType].Any() || !_a1[dataType][0].Any())
                                {
                                    _i[dataType] = 0;
                                }
                            }
                        }
                    }
                }
                file.WriteLine(_f1);
                file.Close();
                MessageBox.Show("Решения найдены");
            }
        }

        /// <summary>
        /// Формирование перебора для всех возможных решений из А2
        /// </summary>
        /// <param name="ind">текущий индекс изменяемого решения для 1 типа</param>
        /// <param name="_n">Матрица номеров решений из А2</param>
        /// <param name="f">Файл для записей логов</param>
        ///Менят здесь для _\*РУСЛАН*/_
        private void GenerateCombination(int ind, List<int> _n)
        {
            for (int dataType = dataTypesCount - 1; dataType >= 0; dataType--)
            {
                for (int j = 0;j <_a2[dataType].Count; j++)
                {
                    _n[dataType]=j;
                    //f.WriteLine(PrintList(_n));
                        GetSolution(_n);
                }        
            }
        }

        /// <summary>
        /// Подстановка данных из перебора и вычисление решения
        /// </summary>
        /// <param name="_n">Массив индексов решений из А2</param>
        /// <param name="f">Файл для записей логов</param>
        private void GetSolution(List<int> _n)
        {
            var tempA = ListUtils.MatrixDeepCopy(_a);
            for (var dataType = 0; dataType < dataTypesCount; dataType++)
            {
                if (_n[dataType] >= 0)
                {
                    tempA[dataType] = ListUtils.VectorDeepCopy(SetTempAFromA2(dataType, _n[dataType])[dataType]);
                }
            }
            var shedule = new Shedule(tempA);
            //shedule.ConstructShedule();
            shedule.ConstructSheduleWithBuffer(3, dataTypesCount);
            var r = shedule.RetyrnR();
            var sets = new Sets(Form1.compositionSets, Form1.timeSets);
            sets.GetSolution(r);
            var time = sets.GetNewCriterion(Form1.direct);
            var s = PrintA(tempA);
            //f.Write(s + " - " + fBuf);
            //MessageBox.Show(s + " Время обработки " + fBuf);
            if (time < _f1Buf)
            {
                _abuf = ListUtils.MatrixDeepCopy(tempA);
                _typeSolutionFlag = true;
                _f1Buf = time;
                //file.Write(" +");
            }
            //f.WriteLine();
        }

        /// <summary>
        /// Алгоритм формирования решения по составам паритй всех типов данных
        /// </summary>
        public int[] GenetateSolutionForAllTypesSecondAlgorithm()
        {
            var sets = new Sets(Form1.compositionSets, Form1.timeSets);
            var result = new[] { 0, 0 };
            //using (var f = new StreamWriter("standartOutData.txt", true))
            {
                GenerateStartSolution();
                var shedule = new Shedule(_a);
                shedule.ConstructShedule();
                var r = shedule.RetyrnR();
                sets.GetSolution(r);
                var time = sets.GetNewCriterion(Form1.direct);
                var _f1 = time;
                _f1Buf = _f1;
                result[0] = _f1Buf;
                var maxA = ListUtils.MatrixDeepCopy(_a);
                _typeSolutionFlag = true;
                if (!isFixedBatches)
                {
                    while (CheckType(_i))
                    {
                        // Буферезируем текущее решение для построение нового на его основе
                        _ai = ListUtils.MatrixDeepCopy(_a);
                        if (_typeSolutionFlag)
                        {
                            _a1 = new List<List<List<int>>>();
                            for (var dataType = 0; dataType < dataTypesCount; dataType++)
                            {
                                _a1.Add(new List<List<int>>());
                                _a1[dataType].Add(new List<int>());
                                _a1[dataType][0] = ListUtils.VectorDeepCopy(_a[dataType]);
                            }
                            _typeSolutionFlag = false;
                        }

                        var tempA = ListUtils.MatrixDeepCopy(_ai);
                        _abuf = ListUtils.MatrixDeepCopy(_ai);
                        _f1Buf = _f1;

                        // Для каждого типа и каждого решения в типе строим новое решение и проверяем его на критерий
                        _a2 = new List<List<List<int>>>();
                        string s;
                        for (var dataType = 0; dataType < dataTypesCount; dataType++)
                        {
                            _a2.Add(new List<List<int>>());
                            if (_i[dataType] <= 0) continue;
                            _a2[dataType] = NewData(dataType);
                            for (var j = 0; j < _a2[dataType].Count; j++)
                            {
                                tempA = SetTempAFromA2(dataType, j);
                                shedule = new Shedule(tempA);
                                shedule.ConstructShedule();
                                r = shedule.RetyrnR();
                                sets = new Sets(Form1.compositionSets, Form1.timeSets);
                                sets.GetSolution(r);
                                time = sets.GetNewCriterion(Form1.direct);
                                s = PrintA(tempA);
                                //f.Write(s + " - " + time);
                                if (time < _f1Buf)
                                {
                                    _abuf = ListUtils.MatrixDeepCopy(tempA);
                                    _typeSolutionFlag = true;
                                    _f1Buf = time;
                                }
                                //f.WriteLine();
                            }
                        }
                        if (!_typeSolutionFlag)
                        {
                            List<int> _n = new List<int>();
                            _nTemp = new List<int>();
                            for (int dataType = 0; dataType < dataTypesCount; dataType++)
                            {
                                _nTemp.Add(0);
                                _n.Add(_a2[dataType].Count);
                                if (_n[dataType] == 0) _n[dataType] = -1;
                            }
                            GenerateCombination(0, _nTemp);
                        }
                        if (_typeSolutionFlag)
                        {
                            _a = ListUtils.MatrixDeepCopy(_abuf);
                            _f1 = _f1Buf;
                        }
                        else
                        {
                            for (int dataType = 0; dataType < dataTypesCount; dataType++)
                            {
                                _a1[dataType] = ListUtils.MatrixDeepCopy(_a2[dataType]);
                                if (!_a1[dataType].Any() || !_a1[dataType][0].Any())
                                {
                                    _i[dataType] = 0;
                                }
                            }
                        }
                        //f.WriteLine("------------------");
                    }
                }
                result[1] = _f1;
                //f.Close();
            }
            return result;
        }

        #region Неиспользуемые функции

        /// <summary>
        /// Нужна для отладки вывода массива 
        /// </summary>
        /// <param name="m">входной лист</param>
        /// <returns>лист в виде строки</returns>
        private static string PrintList(List<int> m)
        {
            var result = "";
            foreach (var t in m)
            {
                result += t + ", ";
            }
            return result;
        }

        /// <summary>
        /// Проверка на достижение максимально возможного решения по составам типов
        /// </summary>
        /// <param name="inMatrix">Матрица текущих составов</param>
        private void CheckSolution(IReadOnlyList<List<int>> inMatrix)
        {
            for (var i = 0; i < inMatrix.Count; i++)
            {
                var elem = inMatrix[i][0];


                if (elem != 2)
                    continue;

                var count = 1;
                for (var j = 1; j < inMatrix[i].Count; j++)
                {
                    if (inMatrix[i][j] == elem)
                    {
                        count++;
                    }
                }
                if (count == inMatrix[i].Count)
                {
                    _i[i] = 0;
                }
            }
        }

        /// <summary>
        /// Функция вычисления f1 критерия
        /// </summary>
        /// <param name="inMatrix">Матрица А на текущем шаге</param>
        /// <returns>Значение критериия</returns>
        public int GetCriterion(List<List<int>> inMatrix)
        {
            return inMatrix.SelectMany(t => t).Sum();
        }

        #endregion
    }
}
