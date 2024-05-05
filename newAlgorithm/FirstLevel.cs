﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using magisterDiplom.Utils;
using magisterDiplom.Model;
using magisterDiplom.Fabric;

namespace newAlgorithm
{
    public class FirstLevel
    {
        /// <summary>
        /// Данная структура данных содержит информацию о конфигурации конвейерной системы
        /// </summary>
        private readonly Config config;

        /// <summary>
        /// Данная переменная определяет вектор данных для интерпритации типов данных
        /// </summary>
        private readonly List<int> _i;
        
        private List<List<int>> _ai;                    // Буферизированная матрица составов партий требований на k+1 шаге 
        
        /// <summary>
        /// Лучшая матрица составов партий
        /// </summary>
        private List<List<int>> bestMatrixA;

        
        private List<List<List<int>>> _a1;              // Матрица составов партий требований на k+1 шаге 

        // Матрица составов партий требований фиксированного типа
        // [dataTypesCount x ??? x ???] 
        private List<List<List<int>>> _a2;              

        /// <summary>
        /// Аналог матрицы A - A'
        /// Матрица составов партий требований на k шаге.
        /// matrixA_Prime[i][h], где i - это тип данных. h - это индекс партии, а значение по индексам это количество партий
        /// </summary>
        public List<List<int>> PrimeMatrixA { get; private set; }
        
        /// <summary>
        /// Данная переменная определяет вектор количества требований для каждого типа данных
        /// </summary>
        private readonly List<int> batchCountList;        // Начальное количество требований для каждого типа данных
        
        /// <summary>
        /// Текущий критерий f1
        /// </summary>
        private int f1Current;
        
        /// <summary>
        /// Лучший критерий f1
        /// </summary>
        private int f1Optimal;
        private List<int> _nTemp;
        private bool isBestSolution;

        /// <summary>
        /// Конструктор с параметрами принимающий структуру конфигурации
        /// </summary>
        /// <param name="config">Структура конифгурации содержащая в себе информацию о конвейерной системе</param>
        /// <param name="batchCountList">Вектор длиной config.dataTypesCount, каждый элемент которого - это количество элементов в партии</param>
        public FirstLevel(Config config, List<int> batchCountList)
        {
            this.config = config;
            this.batchCountList = batchCountList;
            _i = new List<int>(config.dataTypesCount);
        }

        /// <summary>
        /// Алгоритм формирования фиксированных партий
        /// Данная функция выполняет инициализацию матрицы _a и вектора _i.
        /// Вектор _i инициализируется, как вектор из 1 длиной dataTypesCount.
        /// Матрица matrixA_Prime инициализируется, как матрица [dataTypesCount x 1] = [n x ].
        /// </summary>
        public void GenerateFixedBatchesSolution()
        {
            // Инициализируем строки матрицы A
            PrimeMatrixA = new List<List<int>>();

            // Для каждого типа данных выполняем обработку
            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
            {

                // Добавляем в вектор _i элемент 1 в конец списка
                _i.Add(1);

                // Инициализируем столбцы матрицы A
                PrimeMatrixA.Add(new List<int>());

                // Для каждой строки матрицы A добавляем вектор количеств элементов в партии
                PrimeMatrixA[dataType].Add(batchCountList[dataType]);
            }
        }

        /// <summary>
        /// Алгоритм формирования начальных решений по составам партий всех типов
        /// Правило 1: Количество данных 𝑖-го типа в партиях не может быть менее 2
        /// </summary>
        public void GenerateStartSolution()
        {
            // Минимальное количество данных в партии
            const int minBatchSize = 2;

            // Выполяем отчистку вектора _i и матрицы _a
            _i.Clear();
            PrimeMatrixA?.Clear();

            // Инициализируем матрицу A
            PrimeMatrixA = new List<List<int>>();
            
            // Для каждого типа данных выполняем обработку
            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
            {

                // Для каждого типа указываем, что они находятся на расмотрении
                _i.Add(1);

                // Для каждого типа создаём вектор с составом партий и формируем его, как [n_p - 2, 2]
                PrimeMatrixA.Add(new List<int>());
                PrimeMatrixA[dataType].Add(batchCountList[dataType] - minBatchSize);
                PrimeMatrixA[dataType].Add(minBatchSize);
            }

            // Для каждого типа данных выполняем проверку
            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
            
                // Выполяем проверку на отсутсвие единичных партий
                if (PrimeMatrixA[dataType][0] < 2 || PrimeMatrixA[dataType][0] < PrimeMatrixA[dataType][1])
                {
                    PrimeMatrixA[dataType].Clear();
                    PrimeMatrixA[dataType].Add(batchCountList[dataType]);
                    _i[dataType] = 0;
                }
        }

        /// <summary>
        /// Функция проверки наличия оставшихся в расмотрении типов
        /// </summary>
        /// <returns>True, если в наличие еще есть рассматриваемые типы, иначе False</returns>
        private bool CheckType()
        {
            // Для каждого типа данных выполняем проверку на не нулевое количество типов
            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)

                // Проверяем количество типов
                if (_i[dataType] > 0)

                    // Если в списке количество больше 0, значит типы ещё есть в расмотрении
                    return true;

            // Все типы были расмотрены
            return false;
        }

        /// <summary>
        /// Построчное формирование матрицы промежуточного решени
        /// </summary>
        /// <param name="dataType">тип рассматриваемого решения</param>
        /// <param name="batchIndex">индекс подставляемого решения</param>
        /// <returns>матрица А с подставленным новым решением в соответствующий тип</returns>
        private List<List<int>> SetTempAFromA2(int dataType, int batchIndex)
        {
            var result = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);
            if (batchIndex < _a2[dataType].Count)
                result[dataType] = ListUtils.VectorIntDeepCopy(_a2[dataType][batchIndex]);
            return result;
        }

        /// <summary>
        /// Функция получения неповторяющихся решений в матрице А2 на шаге 9
        /// </summary>
        /// <param name="inMatrix">входная матрица сформированных решений</param>
        /// <returns>Новые решения без повторений</returns>
        public List<List<int>> SortedMatrix(List<List<int>> inMatrix)
        {
            var temp = ListUtils.MatrixIntDeepCopy(inMatrix);
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
                        return countFind == inList.Count;
                    });
                    if (lastIndexForDelete == i) continue;
                    temp.RemoveAt(lastIndexForDelete);
                    inMatrix.RemoveAt(lastIndexForDelete);
                }

                if (++countLoops > 100)
                    break;
            }
            return inMatrix;
        }
        
        /// <summary>
        /// Удаление повторений новых решений совпадающих с A1
        /// </summary>
        /// <param name="inMatrix">матрица новых решений</param>
        /// <param name="dataType">рассматриваемый тип</param>
        /// <returns>Полученные новые решения</returns>
        private List<List<int>> CheckMatrix(List<List<int>> inMatrix, int dataType)
        {

            foreach (var row2 in _a1[dataType])
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
        /// <param name="dataType">рассматриваемый тип</param>
        /// <returns>новые решения для этого типа</returns>
        private List<List<int>> NewData(int dataType)
        {
            var result = new List<List<int>>();
            foreach(var row in _a1[dataType])
            {
                for (var j = 1; j < row.Count; j++)
                {
                    result.Add(ListUtils.VectorIntDeepCopy(row));
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
            result = CheckMatrix(result,dataType);
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
            if (type < config.dataTypesCount)
            {
                for (var variantOfSplitIndex = 0; variantOfSplitIndex < _a2[type].Count; variantOfSplitIndex++)
                {
                    List<List<int>> tempB = (tempM != null) ? ListUtils.MatrixIntDeepCopy(tempM) : new List<List<int>>();

                    tempB.Add(tempMatrix[type][variantOfSplitIndex]);
                    CombinationType(file, tempMatrix, type + 1, tempB, ref solutionFlag);
                }
            } else
            {
                var shedule = new Shedule(tempM);
                //shedule.ConstructShedule();
                shedule.ConstructSheduleWithBuffer(config.buffer, config.dataTypesCount);
                var fBuf = shedule.GetTime();
                string s = ListUtils.MatrixIntToString(tempM, ", ", "", ";");
                file.Write(s + " " + fBuf);
                MessageBox.Show(s + " Время обработки " + fBuf);
                if (fBuf < f1Optimal)
                {
                    bestMatrixA = ListUtils.MatrixIntDeepCopy(tempM);
                    solutionFlag = true;
                    f1Optimal = fBuf;
                    file.Write(" +");
                    return;
                }
                file.WriteLine();
            }
        }


        /// <summary>
        /// Рекурсивная комбинация всех типов _a2 с фиксированным решением _a
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tempA"></param>
        /// <param name="type"></param>
        public void CombinationTypeWithPremaintences(StreamWriter file, List<List<List<int>>> tempMatrix, int type, List<List<int>> tempM, ref bool solutionFlag, ref SimplePreMSchedule schedule)
        {
            if (type < config.dataTypesCount)
            {
                for (var variantOfSplitIndex = 0; variantOfSplitIndex < _a2[type].Count; variantOfSplitIndex++)
                {
                    List<List<int>> tempB = (tempM == null) ? new List<List<int>>() : ListUtils.MatrixIntDeepCopy(tempM);
                    tempB.Add(tempMatrix[type][variantOfSplitIndex]);
                    CombinationTypeWithPremaintences(file, tempMatrix, type + 1, tempB, ref solutionFlag, ref schedule);
                }
            }
            else
            {
                if (schedule.Build(tempM))
                {

                    var fBuf = schedule.GetMakespan();
                    string s = ListUtils.MatrixIntToString(tempM, ", ", "", ";");
                    file.Write(s + " " + fBuf);
                    MessageBox.Show(s + " Время обработки " + fBuf);
                    if (fBuf < f1Optimal)
                    {
                        bestMatrixA = ListUtils.MatrixIntDeepCopy(tempM);
                        solutionFlag = true;
                        f1Optimal = fBuf;
                        file.Write(" +");
                        return;
                    }
                    file.WriteLine();
                }
            }
        }

        /// <summary>
        /// Алгоритм формирования решения по составам паритй всех типов данных
        /// </summary>
        public void GenetateSolutionForAllTypes(string fileName)
        {
            using (var file = new StreamWriter(fileName))
            {
                GenerateFixedBatchesSolution();
                var shedule = new Shedule(PrimeMatrixA);
                //shedule.ConstructShedule();
                shedule.ConstructSheduleWithBuffer(config.buffer, config.dataTypesCount);
                f1Current = shedule.GetTime();

                MessageBox.Show(ListUtils.MatrixIntToString(PrimeMatrixA, ", ", "", ";") + "Время обработки " + f1Current);
                f1Optimal = f1Current;
                file.WriteLine(f1Optimal);
                var maxA = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);
                isBestSolution = true;

                // Генерируем начальное решение
                GenerateStartSolution();

                shedule = new Shedule(PrimeMatrixA);
                //shedule.ConstructShedule();
                shedule.ConstructSheduleWithBuffer(config.buffer, config.dataTypesCount);
                f1Current = shedule.GetTime();
                MessageBox.Show(ListUtils.MatrixIntToString(PrimeMatrixA, ", ", "", ";") + " Время обработки " + f1Current);
                if (f1Current < f1Optimal)
                {
                    bestMatrixA = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);
                    isBestSolution = true;
                    f1Optimal = f1Current;
                    file.Write(" +");
                }
                if (!config.isFixedBatches)
                {

                    // До тех пор, поа не расмотрели все типы выполняем обработку
                    while (CheckType())
                    {
                        // Буферезируем текущее решение для построение нового на его основе
                        _ai = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);
                        if (isBestSolution)
                        {
                            _a1 = new List<List<List<int>>>();

                            // Для каждого типа данных выполняем обработку
                            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                            {
                                _a1.Add(new List<List<int>>());
                                _a1[dataType].Add(new List<int>());
                                _a1[dataType][0] = ListUtils.VectorIntDeepCopy(PrimeMatrixA[dataType]);
                            }
                            isBestSolution = false;
                        }

                        List<List<int>> tempA; // var tempA = ListUtils.MatrixIntDeepCopy(_ai);
                        bestMatrixA = ListUtils.MatrixIntDeepCopy(_ai);
                        f1Optimal = f1Current;

                        // Для каждого типа и каждого решения в типе строим новое решение и проверяем его на критерий
                        // Строим A2 и параллельно проверяем критерий
                        _a2 = new List<List<List<int>>>(config.dataTypesCount);

                        // Выполяем инициализацию
                        _a2.AddRange(Enumerable.Repeat(new List<List<int>>(), config.dataTypesCount));
                        
                        string s;
                        file.WriteLine("окрестность 1 вида");

                        // Для каждого типа данных в рассмотрении (_i[dataType] != 0) выполняем обработку
                        for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                        {

                            // Если данный тип данных не находится в рассмотрении
                            if (_i[dataType] <= 0)

                                // Пропускаем итерацию
                                continue;

                            // Формируем новый состав партий для типа dataType
                            _a2[dataType] = NewData(dataType);

                            // Для каждого пакета в новом составе партий выполняем обработку
                            for (var batchIndex = 0; batchIndex < _a2[dataType].Count; batchIndex++)
                            {
                                tempA = SetTempAFromA2(dataType, batchIndex);
                                shedule = new Shedule(tempA);
                                //shedule.ConstructShedule();
                                shedule.ConstructSheduleWithBuffer(config.buffer, config.dataTypesCount);
                                var fBuf = shedule.GetTime();
                                s = ListUtils.MatrixIntToString(tempA, ", ", "", ";");
                                file.Write(s + " " + fBuf);
                                MessageBox.Show(s + " Время обработки " + fBuf);                                    
                                if (fBuf < f1Optimal)
                                {
                                    bestMatrixA = ListUtils.MatrixIntDeepCopy(tempA);
                                    isBestSolution = true;
                                    f1Optimal = fBuf;
                                    file.Write(" +");
                                }
                                file.WriteLine();
                            }
                        }
                        if (!isBestSolution)
                        {
                            file.WriteLine("комбинации типов");
                            CombinationType(file, _a2, 0, null, ref isBestSolution);
                        }

                        if (isBestSolution)
                        {
                            MessageBox.Show("Лучшее решение " + ListUtils.MatrixIntToString(bestMatrixA, ", ", "", ";") + " Время обработки " + f1Optimal);
                            PrimeMatrixA = ListUtils.MatrixIntDeepCopy(bestMatrixA);
                            f1Current = f1Optimal;

                            continue;
                        }

                        // Для каждого типа данных выполняем обработку
                        for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
                        {
                            _a1[dataType] = ListUtils.MatrixIntDeepCopy(_a2[dataType]);
                            if (!_a1[dataType].Any() || !_a1[dataType][0].Any())
                                _i[dataType] = 0;
                        }
                    }
                }
                file.WriteLine(f1Current);
                file.Close();
                MessageBox.Show("Решения найдены");
            }
        }


        /// <summary>
        /// Алгоритм формирования решения по составам паритй всех типов данных
        /// </summary>
        public void GenetateSolutionWithPremaintenance(string fileName)
        {

            // Переопределяем значение оптимального критерий f1
            f1Optimal = int.MaxValue;
            f1Current = int.MaxValue;

            using (var file = new StreamWriter(fileName))
            {

                // Создаём экземпляр класса для работы с нижним уровнем
                SimplePreMSchedule schedule = new SimplePreMSchedule(config);

                // TODO: Костыль для фиксированных пакетов
                {
                    GenerateFixedBatchesSolution();

                    // TODO: Использовать объёкт своего класса
                    
                    if (schedule.Build(PrimeMatrixA))
                    {

                        // Получаем f1 критерий
                        f1Current = schedule.GetMakespan();

                        MessageBox.Show(ListUtils.MatrixIntToString(PrimeMatrixA, ", ", "", ";") + "Время обработки " + f1Current);
                        f1Optimal = f1Current;
                        file.WriteLine(f1Optimal);
                        // TODO: ненужное присваивание
                        // var maxA = ListUtils.MatrixIntDeepCopy(primeMatrixA);
                        isBestSolution = true;
                    }
                }
                
                // Генерируем начальное решение
                GenerateStartSolution();

                // Вызываем расчёты
                if (schedule.Build(PrimeMatrixA)) { 
                    
                    // Получаем f1
                    f1Current = schedule.GetMakespan();
                    MessageBox.Show(ListUtils.MatrixIntToString(PrimeMatrixA, ", ", "", ";") + " Время обработки " + f1Current);
                }

                // Если текущей критерий лучше оптимального
                if (f1Current < f1Optimal)
                {

                    // Копируем матрицу с лучшим решением
                    bestMatrixA = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);

                    // Устанавливаем флаг лучшего решения
                    isBestSolution = true;

                    // Переопределяем критерий f1 лучшего решения
                    f1Optimal = f1Current;

                    // Логируем нахождение лучшего решения
                    file.Write(" +");
                }

                // Инициализируем матрицу _a1
                {
                    _a1 = new List<List<List<int>>>();

                    // Для каждого типа данных выполняем обработку
                    for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                    {
                        _a1.Add(new List<List<int>>());
                        _a1[dataType].Add(new List<int>());
                        _a1[dataType][0] = ListUtils.VectorIntDeepCopy(PrimeMatrixA[dataType]);
                    }
                }

                // Если пакеты не фиксированные
                if (!config.isFixedBatches)
                {

                    // До тех пор, поа не расмотрели все типы выполняем обработку
                    while (CheckType())
                    {
                        // Буферезируем текущее решение для построение нового на его основе
                        _ai = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);

                        // Если текущее решение лучше
                        if (isBestSolution)
                        {
                            // Копируем текущее решение во временный массив _a1
                            
                            _a1 = new List<List<List<int>>>();

                            // Для каждого типа данных выполняем обработку
                            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                            {
                                _a1.Add(new List<List<int>>());
                                _a1[dataType].Add(new List<int>());
                                _a1[dataType][0] = ListUtils.VectorIntDeepCopy(PrimeMatrixA[dataType]);
                            }

                            // Сбрасываем флаг лучшего решения
                            isBestSolution = false;
                        }

                        // TODO: Ненужное присваивание
                        List<List<int>> tempA; // var tempA = ListUtils.MatrixIntDeepCopy(_ai);
                        bestMatrixA = ListUtils.MatrixIntDeepCopy(_ai);
                        
                        // TODO: Спорный случай, всегда перезатираем лучшее решение из костыля
                        f1Optimal = f1Current;

                        // Для каждого типа и каждого решения в типе строим новое решение и проверяем его на критерий
                        // Строим A2 и параллельно проверяем критерий
                        _a2 = new List<List<List<int>>>(config.dataTypesCount);

                        // Выполяем инициализацию
                        _a2.AddRange(Enumerable.Repeat(new List<List<int>>(), config.dataTypesCount));

                        string s;
                        file.WriteLine("окрестность 1 вида");

                        // Для каждого типа данных в рассмотрении (_i[dataType] != 0) выполняем обработку
                        for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                        {

                            // Если данный тип данных не находится в рассмотрении
                            if (_i[dataType] <= 0)

                                // Пропускаем итерацию
                                continue;

                            // Формируем новый состав партий для типа dataType
                            _a2[dataType] = NewData(dataType);

                            // Для каждого пакета в новом составе партий выполняем обработку
                            for (var batchIndex = 0; batchIndex < _a2[dataType].Count; batchIndex++)
                            {
                                tempA = SetTempAFromA2(dataType, batchIndex);

                                // TODO: Вызывать построение нижнего уровня
                                // TODO:

                                // Если расписание построилось не успешно
                                if (!schedule.Build(tempA))

                                    // Пропускаем обработку
                                    continue;

                                // Получаем критерий f1
                                var fBuf = schedule.GetMakespan();
                                s = ListUtils.MatrixIntToString(tempA, ", ", "", ";");
                                file.Write(s + " " + fBuf);
                                MessageBox.Show(s + " Время обработки " + fBuf);

                                // Если текущее решение лучше
                                if (fBuf < f1Optimal)
                                {

                                    // Копируем матрицу с лучшим решением
                                    bestMatrixA = ListUtils.MatrixIntDeepCopy(tempA);

                                    // Устанавливаем флаг лучшего решения
                                    isBestSolution = true;

                                    // Переопределяем критерий f1 лучшего решения
                                    f1Optimal = fBuf;

                                    // Логируем нахождение лучшего решения
                                    file.Write(" +");
                                }

                                // Логируем
                                file.WriteLine();
                            }
                        }
                        
                        // Если лучшее решения не было найдено
                        if (!isBestSolution)
                        {

                            // Логируем вызов комбинаций типов
                            file.WriteLine("комбинации типов");

                            // Формируем следующий состав пакетов заданий
                            CombinationTypeWithPremaintences(file, _a2, 0, null, ref isBestSolution, ref schedule);
                        }

                        // Если лучшее решения было найдено
                        if (isBestSolution)
                        {
                            MessageBox.Show("Лучшее решение " + ListUtils.MatrixIntToString(bestMatrixA, ", ", "", ";") + " Время обработки " + f1Optimal);

                            // Запоминаем лучшее решение
                            PrimeMatrixA = ListUtils.MatrixIntDeepCopy(bestMatrixA);

                            // TODO: Проверить излишнее переопределение f1Current в f1Optimal
                            f1Current = f1Optimal;

                            continue;
                        }

                        // Для каждого типа данных выполняем обработку
                        for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
                        {
                            _a1[dataType] = ListUtils.MatrixIntDeepCopy(_a2[dataType]);
                            if (!_a1[dataType].Any() || !_a1[dataType][0].Any())
                                _i[dataType] = 0;
                        }
                    }
                }

                // Проверяем успешность работы программы
                if (f1Optimal == int.MaxValue)
                {
                    MessageBox.Show("Решения не было найдено");
                    return;
                }

                // Логируем лучший критерий f1
                file.WriteLine(f1Optimal);
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
        private void GenerateCombination(List<int> _n)
        {

            // Для каждого типа данных выполняем обработку с конца
            for (int dataType = config.dataTypesCount - 1; dataType >= 0; dataType--)
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
            var tempA = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);

            // Для каждого типа данных выполняем обработку
            for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
            {
                if (_n[dataType] >= 0)
                {
                    tempA[dataType] = ListUtils.VectorIntDeepCopy(SetTempAFromA2(dataType, _n[dataType])[dataType]);
                }
            }
            var shedule = new Shedule(tempA);
            //shedule.ConstructShedule();
            shedule.ConstructSheduleWithBuffer(3, config.dataTypesCount);

            shedule.BuildMatrixRWithTime();
            var matrixRWithTime = shedule.ReturnMatrixRWithTime();

            var sets = new Sets(Form1.compositionSets, Form1.timeSets);
            sets.GetSolution(matrixRWithTime);
            var time = sets.GetNewCriterion(Form1.direct);
            //var s = ListUtils.MatrixIntToString(tempA, ", ", "", ";");
            //f.Write(s + " - " + fBuf);
            //MessageBox.Show(s + " Время обработки " + fBuf);
            if (time < f1Optimal)
            {
                bestMatrixA = ListUtils.MatrixIntDeepCopy(tempA);
                isBestSolution = true;
                f1Optimal = time;
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

                // Генерируем начальное решение
                GenerateStartSolution();

                // Создаём экземпляр класса расписания с помощью матрицы Aprime
                var shedule = new Shedule(PrimeMatrixA);

                // Выполяем построение расписания
                shedule.ConstructShedule();

                shedule.BuildMatrixRWithTime();
                var matrixRWithTime = shedule.ReturnMatrixRWithTime();
                sets.GetSolution(matrixRWithTime);
                var time = sets.GetNewCriterion(Form1.direct);
                var _f1 = time;
                f1Optimal = _f1;
                result[0] = f1Optimal;
                isBestSolution = true;

                // До тех пор пока в наличие есть оставшиеся типы и партии не фиксированные выполняем обработку
                while (CheckType() && !config.isFixedBatches)
                {
                    // Буферезируем текущее решение для построение нового на его основе
                    _ai = ListUtils.MatrixIntDeepCopy(PrimeMatrixA);
                    if (isBestSolution)
                    {
                        _a1 = new List<List<List<int>>>();

                        // Для каждого типа данных выполняем обработку
                        for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                        {
                            _a1.Add(new List<List<int>>());
                            _a1[dataType].Add(new List<int>());
                            _a1[dataType][0] = ListUtils.VectorIntDeepCopy(PrimeMatrixA[dataType]);
                        }
                        isBestSolution = false;
                    }

                    List<List<int>> tempA; // var tempA = ListUtils.MatrixIntDeepCopy(_ai);
                    bestMatrixA = ListUtils.MatrixIntDeepCopy(_ai);
                    f1Optimal = _f1;

                    // Для каждого типа и каждого решения в типе строим новое решение и проверяем его на критерий
                    _a2 = new List<List<List<int>>>();

                    // Для кадого типа данных выполняем обработку
                    for (var dataType = 0; dataType < config.dataTypesCount; dataType++)
                    {
                        _a2.Add(new List<List<int>>());
                        if (_i[dataType] <= 0) continue;
                        _a2[dataType] = NewData(dataType);
                        for (var j = 0; j < _a2[dataType].Count; j++)
                        {
                            tempA = SetTempAFromA2(dataType, j);
                            shedule = new Shedule(tempA);
                            shedule.ConstructShedule();

                            shedule.BuildMatrixRWithTime();
                            matrixRWithTime = shedule.ReturnMatrixRWithTime();

                            sets = new Sets(Form1.compositionSets, Form1.timeSets);
                            sets.GetSolution(matrixRWithTime);
                            time = sets.GetNewCriterion(Form1.direct);
                            //s = ListUtils.MatrixIntToString(tempA, ", ", "", ";");
                            //f.Write(s + " - " + time);
                            if (time < f1Optimal)
                            {
                                bestMatrixA = ListUtils.MatrixIntDeepCopy(tempA);
                                isBestSolution = true;
                                f1Optimal = time;
                            }
                            //f.WriteLine();
                        }
                    }
                    if (!isBestSolution)
                    {
                        List<int> _n = new List<int>();
                        _nTemp = new List<int>();

                        // Для каждого типа данных выполняем обработку
                        for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
                        {
                            _nTemp.Add(0);
                            _n.Add(_a2[dataType].Count);
                            if (_n[dataType] == 0) _n[dataType] = -1;
                        }
                        GenerateCombination(_nTemp);
                    }
                    if (isBestSolution)
                    {
                        PrimeMatrixA = ListUtils.MatrixIntDeepCopy(bestMatrixA);
                        _f1 = f1Optimal;

                        // Продолжаем цикл
                        continue;
                    }
                    
                    // Для каждого типа данных выполняем обработку
                    for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
                    {
                        _a1[dataType] = ListUtils.MatrixIntDeepCopy(_a2[dataType]);
                        if (!_a1[dataType].Any() || !_a1[dataType][0].Any())
                            _i[dataType] = 0;
                    }
                    
                    //f.WriteLine("------------------");
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
        // private static string PrintList(List<int> m)
        // {
        //     var result = "";
        //     foreach (var t in m)
        //     {
        //         result += t + ", ";
        //     }
        //     return result;
        // }

        /// <summary>
        /// Проверка на достижение максимально возможного решения по составам типов
        /// </summary>
        /// <param name="inMatrix">Матрица текущих составов</param>
        // private void CheckSolution(IReadOnlyList<List<int>> inMatrix)
        // {
        //     for (var i = 0; i < inMatrix.Count; i++)
        //     {
        //         var elem = inMatrix[i][0];
        // 
        // 
        //         if (elem != 2)
        //             continue;
        // 
        //         var count = 1;
        //         for (var j = 1; j < inMatrix[i].Count; j++)
        //         {
        //             if (inMatrix[i][j] == elem)
        //             {
        //                 count++;
        //             }
        //         }
        //         if (count == inMatrix[i].Count)
        //         {
        //             _i[i] = 0;
        //         }
        //     }
        // }

        /// <summary>
        /// Функция вычисления f1 критерия
        /// </summary>
        /// <param name="inMatrix">Матрица А на текущем шаге</param>
        /// <returns>Значение критериия</returns>
        // public int GetCriterion(List<List<int>> inMatrix)
        // {
        //     return inMatrix.SelectMany(t => t).Sum();
        // }

        #endregion
    }
}
