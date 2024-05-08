using magisterDiplom;
using magisterDiplom.Model;
using magisterDiplom.Utils;
using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace newAlgorithm
{
    public partial class Form1 : Form
    {

        #region Поля данных

        /// <summary>
        /// Данная переменная отвечает за выбор способа обработки данных и определено перечислением SelectionType
        /// </summary>
        private SelectoinType selectoinType;

        /// <summary>
        /// Данная переменная определяет количество данных каждого типа
        /// </summary>
        int batchCount;

        /// <summary>
        /// Данная переменная определяет количество типов данных
        /// </summary>
        int dataTypesCount;

        /// <summary>
        /// Данная переменная определяет размер буфера перед приборами
        /// </summary>
        int buffer;

        /// <summary>
        /// Данная переменная определяет длину конвейера, как количество приборов
        /// </summary>
        int deviceCount;

        /// <summary>
        /// Данная переменная определяет значения для генерации рандомных данных
        /// </summary>
        int randomValue;

        /// <summary>
        /// Данная переменная определяет фиксированные ли партии
        /// </summary>
        bool isFixedBatches;

        /// <summary>
        /// Данная переменная определяет использование оптимизации
        /// </summary>
        bool isOptimization;

        /// <summary>
        /// Данная переменная определяет необходимость обновления данных при переходах на вкладку "Установка времени"
        /// </summary>
        private bool isValueChagedToUpdate;

        /// <summary>
        /// TODO: Написать описание
        /// </summary>
        int generationCount;

        /// <summary>
        /// Данная переменная определяет размер списка из хромосом
        /// </summary>
        int xromossomiSize;

        /// <summary>
        /// Данная переменная представляет из 3-мерную матрицу и используется, как матрица переналадки приборов с одного
        /// типа задания на другой. Первое измерение определяется, как количество приборов на конвейере. Второе и третье
        /// измерения это количество типов данных. Таким образом changeoverTime = [deviceCount x dataTypesCount x dataTypesCount]
        /// </summary>
        List<List<List<int>>> changeoverTime = new List<List<List<int>>>();

        /// <summary>
        /// Данная переменная представляет из себя двумрную матрицу и используется, как матрица времени выполнения заданий.
        /// Первое измерение представляет определяется, как количество приборов на конвейере. Второе измерения это количество
        /// типов данных. Таким образом proccessingTime = [deviceCount x dataTypesCount]
        /// </summary>ы
        List<List<int>> proccessingTime = new List<List<int>>();

        /// <summary>
        /// Данная переменная содержим в себе список из времён вермени выполнения ПТО для соответсвующего прибора.
        /// preMaintenanceTimes = [deviceCount]
        /// </summary>
        List<int> preMaintenanceTimes;

        /// <summary>
        /// Данный список определяет интенсивность отказов для приборов соответсвенно: [deviceCount]
        /// </summary>
        List<double> failureRates;

        /// <summary>
        /// Данный список определяет востановление прибора соответсвенно: [deviceCount]
        /// </summary>
        List<double> restoringDevice;

        // TODO: разобраться со статическими типами данных
        public static int buff;
        public static bool direct;
        public static bool vizualizationOn;
        public static List<List<int>> compositionSets;
        public static List<List<int>> timeSets;

        #endregion

        /// <summary>
        /// Данный конструктор выполняет инициализацию данных для работы формы
        /// </summary>
        public Form1()
        {

            // Вызываем сгенерированную функцию инициализации графических компонентов
            InitializeComponent();

            // Вызываем самописную функцию инициализации данных и графических компонентов
            InitializeForm();
        }

        #region Обработка событий разного рода

        #region Обработка кнопок

        /// <summary>
        /// Данная функция вызывается при нажание на кнопку "Второй метод"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();

            // TODO: Перед вызовом данной функции необходимо выполнить присваивание для Form1.compositionSets и Form1.timeSets

            // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
            List<int> batchCountList = CreateBatchCountList();

            // Инициализируем расписание
            Shedule.deviceCount = deviceCount;
            Shedule.changeoverTime = changeoverTime;
            Shedule.proccessingTime = proccessingTime;

            // Создаём экземпляр конфигурационной структуры
            Config config = new Config(
                dataTypesCount,
                deviceCount,
                buffer,
                (Matrix)(new Matrix(proccessingTime)),
                Config.ChangeoverTimeConverter(changeoverTime),
                isFixedBatches
            );

            // Формируем первый уровень
            var firstLevel = new FirstLevel(config, batchCountList);

            // Выполняем генерацию данных для ввсех типов вторым алгоритмом
            firstLevel.GenetateSolutionForAllTypesSecondAlgorithm();
        }

        /// <summary>
        ///  Данная функция вызывается при нажание на кнопку "Получить решение"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();

            // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
            List<int> batchCountList = CreateBatchCountList();
            
            // Инициализируем расписание
            Shedule.deviceCount = deviceCount;
            Shedule.changeoverTime = changeoverTime;
            Shedule.proccessingTime = proccessingTime;

            // Создаём экземпляр конфигурационной структуры
            // TODO: Создать отдельный конфиг для задачи с ПТО
            Config config = new Config(
                dataTypesCount,
                deviceCount,
                buffer,
                (Matrix) (new Matrix(proccessingTime)),
                Config.ChangeoverTimeConverter(changeoverTime),
                isFixedBatches
            );

            // Формируем первый уровень
            var firstLevel = new FirstLevel(config, batchCountList);

            // Генерируем решение и записываем его в файл
            firstLevel.GenetateSolutionForAllTypes("outputFirstAlgorithm.txt");
        }

        /// <summary>
        /// Данная функция вызывается при нажатии кнопки "Га + Группы"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OldSecondLevelAll_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();

            // Формируем вектор из значений { 2, 4, 8, 16, 32 }, для генерации матриц выполнения и переналадки
            int[] array = { 2, 4, 8, 16, 32 };

            using (var file = new StreamWriter("Га, оптимизации групп " + isOptimization + " N=" + dataTypesCount + "L=" + deviceCount + ".txt", false))
            {

                // Перебираем каждой элемент вектора, как время переналадки приборов
                foreach (int _maxChangeoverTime in array)
                {

                    // Перебираем каждой элемент вектора, как время выполнения задания
                    foreach (int _maxProccessingTime in array)
                    {
                        for (var tz = 50; tz <= 200; tz += 50)
                        {
                            for (var countGroup = 2; countGroup <= 8; countGroup += 2)
                            {

                                // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
                                List<int> batchCountList = CreateBatchCountList();

                                // Выполняем настройку расписания
                                Shedule.deviceCount = deviceCount;
                                Shedule.changeoverTime = OldChangeoverTimeGenerator(_maxChangeoverTime, deviceCount, dataTypesCount);
                                Shedule.proccessingTime = OldProccessingTimeGenerator(_maxProccessingTime, deviceCount, dataTypesCount);

                                // Создаём экземпляр конфигурационной структуры
                                Config config = new Config(
                                    dataTypesCount,
                                    deviceCount,
                                    buffer,
                                    (Matrix)(new Matrix(OldProccessingTimeGenerator(_maxProccessingTime, deviceCount, dataTypesCount))),
                                    Config.ChangeoverTimeConverter(OldChangeoverTimeGenerator(_maxChangeoverTime, deviceCount, dataTypesCount)),
                                    isFixedBatches
                                );

                                // Создаём экземпляр класса GAA
                                var gaa = new GAA(dataTypesCount, batchCountList, isFixedBatches, batchCount);

                                // Определяем количество хромосом в списке из хромосом
                                gaa.SetXrom(xromossomiSize);
                                var sostav = gaa.calcFitnessList();

                                //var firstLevel = new FirstLevel(_countType, batchCountList, checkBox1.Checked);
                                //firstLevel.GenetateSolutionForAllTypes("outputFirstAlgorithm.txt");
                                var oldSecondLevel = new OldSecondLevel(tz, countGroup);

                                var listInt = !isOptimization
                                    ? oldSecondLevel.CalcFitnessList(sostav, out int criteria, out int flCrit)
                                    : oldSecondLevel.CalcOptimalFitnessList(sostav, out criteria, out flCrit);

                                // Выводим информацию в файл
                                file.WriteLine($"Tz = {tz}");
                                file.WriteLine($"Tp = {_maxChangeoverTime}");
                                file.WriteLine($"Z = {countGroup}");
                                file.WriteLine($"Crit = {criteria}");
                                file.WriteLine($"fLCrit = {flCrit}");
                                file.WriteLine($"To = {_maxProccessingTime}");
                                file.WriteLine($"{listInt.Sum()} = {listInt.Select(i => i.ToString())}");
                            }
                        }
                    }
                }
            }
            MessageBox.Show(@"Решения найдены");
        }

        /// <summary>
        /// Данная функция вызывается при нажатии кнопки "Трёхуровневая"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OldSecondLevelButton_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();

            // Формируем вектор из значений { 2, 4, 8, 16, 32 }, для генерации матриц выполнения и переналадки
            var array = new[] { 2, 4, 8, 16, 32 };

            using (var file = new StreamWriter("Фиксированные партии - " + isFixedBatches + ", оптимизации групп " + isOptimization + " N=" + dataTypesCount + " L=" + deviceCount + ".txt", false))
            {

                // Перебираем каждой элемент вектора, как время переналадки приборов
                foreach (var _maxChangeoverTime in array)
                {

                    // Перебираем каждой элемент вектора, как время выполнения задания
                    foreach (var _maxProccessingTime in array)
                    {
                        for (var tz = 50; tz <= 200; tz += 50)
                        {
                            for (var countGroup = 2; countGroup <= 8; countGroup += 2)
                            {

                                // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
                                List<int> batchCountList = CreateBatchCountList();

                                // Формируем расписание
                                Shedule.deviceCount = deviceCount;
                                Shedule.changeoverTime = OldChangeoverTimeGenerator(_maxChangeoverTime, deviceCount, dataTypesCount);
                                Shedule.proccessingTime = OldProccessingTimeGenerator(_maxProccessingTime, deviceCount, dataTypesCount);

                                // Создаём экземпляр конфигурационной структуры
                                Config config = new Config(
                                    dataTypesCount,
                                    deviceCount,
                                    buffer,
                                    (Matrix)(new Matrix(OldProccessingTimeGenerator(_maxProccessingTime, deviceCount, dataTypesCount))),
                                    Config.ChangeoverTimeConverter(OldChangeoverTimeGenerator(_maxChangeoverTime, deviceCount, dataTypesCount)),
                                    isFixedBatches
                                );

                                // Формируем первый уровень
                                var firstLevel = new FirstLevel(config, batchCountList);

                                firstLevel.GenetateSolutionForAllTypes("outputFirstAlgorithm.txt");
                                var oldSecondLevel = new OldSecondLevel(tz, countGroup);

                                var listInt = !isOptimization
                                    ? oldSecondLevel.CalcFitnessList(firstLevel.PrimeMatrixA, out int criteria, out int flCrit)
                                    : oldSecondLevel.CalcOptimalFitnessList(firstLevel.PrimeMatrixA, out criteria, out flCrit);

                                // Выводим информацию в файл
                                file.WriteLine($"Tz = {tz}");
                                file.WriteLine($"Tp = {_maxChangeoverTime}");
                                file.WriteLine($"Z = {countGroup}");
                                file.WriteLine($"Crit = {criteria}");
                                file.WriteLine($"fLCrit = {flCrit}");
                                file.WriteLine($"To = {_maxProccessingTime}");
                                file.WriteLine($"{listInt.Sum()} = {listInt.Select(i => i.ToString())}");
                            }
                        }
                    }
                }
            }
            MessageBox.Show(@"Решения найдены");
        }

        /// <summary>
        /// Данная фукнция вызывается при нажатии кнопки "Тестовый прогон"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();

            Form1.direct = checkBox_deadline_on.Checked;
            int[] N_komplect_sostav = { 2, 4 };

            // Формируем вектор из возможных количеств типов данных
            int[] _dataTypesCountArray = { 5, 10 };

            // Формируем вектор из возможных количеств приборов в конвейерной системе
            int[] _deviceCountArray = { 5, 10 };

            // Формируем вектор из значений { 2, 4, 8, 16, 32 }, для генерации матриц выполнения и переналадки
            int[] time = { 2, 4, 8, 16, 32 };

            var file = "testFile_";
            var str = "direct";
            if (!Form1.direct)
            {
                str = "first_task";
            }

            int n_kom;
            int n_kom_q;
            try
            {
                n_kom = Convert.ToInt32(textBox1.Text);
                n_kom_q = Convert.ToInt32(textBox2.Text);
            }
            catch (Exception)
            {
                n_kom = 2;
                n_kom_q = 2;
            }
            var rand = new Random((int)DateTime.Now.ToBinary());
            int temp;
            var fileOut = new StreamWriter(file + "All_" + str + "_" + n_kom + "_" + n_kom_q + "_new.txt", true);

            timeSets = new List<List<int>>();
            //foreach(var n_kom_q in N_komplect_for_type)
            {
                for (int i = 0; i < n_kom; i++)
                {
                    temp = (rand.Next(10) > 5) ? 150 : 100;

                    timeSets.Add(new List<int>());
                    for (int j = 0; j < n_kom_q; j++)
                    {
                        timeSets[i].Add((j + 1) * temp);
                    }
                }
                foreach (var n_kom_s in N_komplect_sostav)
                {
                    foreach (int _dataTypesCount in _dataTypesCountArray)
                    {
                        compositionSets = new List<List<int>>();
                        for (int i = 0; i < n_kom; i++)
                        {
                            compositionSets.Add(new List<int>());
                            for (var dataType = 0; dataType < _dataTypesCount; dataType++)
                            {
                                temp = rand.Next(10);
                                if (temp > 5)
                                {
                                    compositionSets[i].Add(n_kom_s);
                                }
                                else
                                {
                                    compositionSets[i].Add(2);
                                }
                            }
                        }

                        foreach (var _deviceCount in _deviceCountArray)
                        {

                            // Выводим информацию в файл
                            fileOut.WriteLine($"Kq = {n_kom_q}");
                            fileOut.WriteLine($"Kqs= {n_kom_s}");
                            fileOut.WriteLine($"N  = {_dataTypesCount};\tL = {_deviceCount}");
                            fileOut.WriteLine($"Times");
                            fileOut.WriteLine(ListUtils.MatrixIntToString(timeSets, "\t"));
                            fileOut.WriteLine($"Compositions");
                            fileOut.WriteLine(ListUtils.MatrixIntToString(compositionSets, "\t"));

                            // Перебираем каждой элемент вектора, как время выполнения задания
                            foreach (var _maxProccessingTime in time)
                            {

                                // Перебираем каждой элемент вектора, как время переналадки приборов
                                foreach (var _maxChangeoverTime in time)
                                {

                                    // Формируем расписание
                                    Shedule.deviceCount = _deviceCount;
                                    Shedule.changeoverTime = OldChangeoverTimeGenerator(_maxChangeoverTime, _deviceCount, _dataTypesCount);
                                    Shedule.proccessingTime = OldProccessingTimeGenerator(_maxProccessingTime, _deviceCount, _dataTypesCount);
                                    
                                    // Создаём экземпляр конфигурационной структуры
                                    Config config = new Config(
                                        _dataTypesCount,
                                        _deviceCount,
                                        buffer,
                                        (Matrix)(new Matrix(OldProccessingTimeGenerator(_maxProccessingTime, _deviceCount, _dataTypesCount))),
                                        Config.ChangeoverTimeConverter(OldChangeoverTimeGenerator(_maxChangeoverTime, _deviceCount, _dataTypesCount)),
                                        isFixedBatches
                                    );
                                    
                                    // Создаём вектор длиной в количество типов данных наполненный нулями
                                    List<int> _batchCountList = CreateBatchCountList(0, _dataTypesCount);

                                    for (var dataType = 0; dataType < _dataTypesCount; dataType++)
                                    {
                                        batchCount = 0;
                                        for (int i = 0; i < n_kom; i++)
                                        {
                                            batchCount += compositionSets[i][dataType];
                                        }
                                        _batchCountList[dataType] = batchCount * n_kom_q;
                                    }

                                    var firstLevel = new FirstLevel(config, _batchCountList);
                                    var result = firstLevel.GenetateSolutionForAllTypesSecondAlgorithm();

                                    //var gaa = new GAA(_countType, listCountButches, checkBox1.Checked, batchCount);
                                    //var resultGaa = gaa.calcSetsFitnessList(checkBox2.Checked, GenerationCounter.Value, (int)numericUpDown2.Value);

                                    //fileOut.WriteLine(first + "\t" + top + "\t" + resultGaa);
                                    fileOut.WriteLine($"first:{result[0]};\ttop:{result[1]}.\t");
                                }
                            }
                        }
                    }
                }
            }
            fileOut.Close();

            MessageBox.Show("Тестовый прогон завершен");
        }

        /// <summary>
        /// Данная функция вызывается при нажатии кнопки "Прогон"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button5Click(object sender, EventArgs e)
        {


            // Выполняем перестроение матриц
            TablesRebuild();

            // Инициализируем выходной файл на добавление
            StreamWriter file_output_method_GAA = new StreamWriter("outputGAASimpleResult.txt", true);

            // Фиксируем начало обработки
            file_output_method_GAA.WriteLine($"Дата и время запуска: {DateTime.Now:O}\n{{");

            // Формируем массив данных [8, 12, 16, 24, 32]
            int[] array = { 8, 12, 16, 24, 32 };

            // Для каждого элемента из массива array выполняем обработку
            foreach (int _batchCount in array)
            {

                // Для следующих количеств типов данных выполняем обработку
                for (int _dataTypesCount = 5; _dataTypesCount <= 10; _dataTypesCount += 5)
                {

                    // Для следующих количеств приборов (сегментов) выполняем обработку
                    for (int _deviceCount = 5; _deviceCount <= 10; _deviceCount += 5)
                    {

                        // Выводим информацию в выходной файл
                        file_output_method_GAA.WriteLine("\tbatchCount(CB)   \t= {0}", _batchCount);
                        file_output_method_GAA.WriteLine("\tconveyorLength(L)\t= {0}", _deviceCount);
                        file_output_method_GAA.WriteLine("\tdataTypesCount(N)\t= {0}", _dataTypesCount);

                        // Для следующих максимальных времён выполнения обработки выполняем обработку
                        for (int _maxProccessingTime = 2; _maxProccessingTime <= 32; _maxProccessingTime *= 2)
                        {
                            // Выводим информацию в выходной файл
                            file_output_method_GAA.WriteLine($"\t{{\n\t\tmaxProccessingTime\t= {_maxProccessingTime}");

                            // Для следующих максимальных времён переналадки приборов выполняем обработку
                            for (int _maxChangeoverTime = 2; _maxChangeoverTime <= 32; _maxChangeoverTime *= 2)
                            {

                                // Выводим информацию в выходной файл
                                file_output_method_GAA.WriteLine($"\t\t{{\n\t\t\tmaxChangeoverTime\t= {_maxChangeoverTime}");

                                // Формируем расписание
                                Shedule.deviceCount = _deviceCount;
                                Shedule.changeoverTime = OldChangeoverTimeGenerator(_maxChangeoverTime, _deviceCount, _dataTypesCount);
                                Shedule.proccessingTime = OldProccessingTimeGenerator(_maxProccessingTime, _deviceCount, _dataTypesCount);

                                // Создаём экземпляр конфигурационной структуры
                                Config config = new Config(
                                    _dataTypesCount,
                                    _deviceCount,
                                    buffer,
                                    (Matrix)(new Matrix(OldProccessingTimeGenerator(_maxProccessingTime, _deviceCount, _dataTypesCount))),
                                    Config.ChangeoverTimeConverter(OldChangeoverTimeGenerator(_maxChangeoverTime, _deviceCount, _dataTypesCount)),
                                    isFixedBatches
                                );

                                // Инициализируем вектор длиной _dataTypesCount, каждый элемент которого будет равен _batchCount
                                List<int> batchCountList = CreateBatchCountList(_batchCount, _dataTypesCount);

                                var gaa = new GAA(_dataTypesCount, batchCountList, isFixedBatches, _batchCount);
                                gaa.SetXrom((int)numeric_xromossomi_size.Value);
                                var countSourceKit = gaa.calcFitnessList();
                                var result = gaa.getSelectionPopulation(selectoinType, out int s);

                                using (var file_outputGAA = new StreamWriter("outputGAA.txt", true))
                                {
                                    foreach (var elem in gaa.nabor)
                                    {

                                        file_outputGAA.WriteLine(ListUtils.MatrixIntToString(elem.GenList, " "));
                                        file_outputGAA.WriteLine("_________________________");
                                        file_outputGAA.WriteLine(gaa._fitnesslist[_maxChangeoverTime]);
                                        file_outputGAA.WriteLine("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
                                        file_outputGAA.WriteLine("Kit" + countSourceKit[_maxChangeoverTime]);
                                        file_outputGAA.WriteLine("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
                                        file_outputGAA.WriteLine("_________________________");

                                    }

                                    file_outputGAA.WriteLine("***************************");
                                    file_outputGAA.WriteLine(result);
                                    file_outputGAA.WriteLine("***************************");

                                }

                                // Выводим информацию в выходной файл
                                file_output_method_GAA.WriteLine("\t\t\tresult\t= {0}", result);
                                file_output_method_GAA.WriteLine($"\t\t}}");
                            }

                            // Выводим информацию в выходной файл
                            file_output_method_GAA.WriteLine($"\t}}");
                        }
                    }
                }
            }

            // Фиксируем конец обработки
            file_output_method_GAA.WriteLine($"}}\nДата и время завершения: {DateTime.Now:O}");
            file_output_method_GAA.Close();

            MessageBox.Show("Данные успешно записаны", "Успешное завершение", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Данная функция вызывается при нажатии кнопки "Формирование Гаа"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();

            // Создаём экземпляр класса Random для генерации рандомных данных
            var rand = new Random(0);

            // Формируем вектор из возможных количеств типов данных
            int[] _dataTypesCountArray = { 5, 10 };

            // Формируем вектор из возможных количеств приборов в конвейерной системе
            int[] _deviceCountArray = { 5, 10 };

            // Формируем вектор из значений { 2, 4, 8, 16, 32 }, для генерации матриц выполнения и переналадки
            int[] time = { 2, 4, 8, 16, 32 };

            int[] N_komplect_type = { 2 };
            int[] N_komplect_for_type = { 2, 4 };
            int[] N_komplect_sostav = { 2, 4 };

            using (var file = new StreamWriter("11 - " + checkBox_deadline_on.Checked + ".txt", false))
            {
                foreach (var n_kom in N_komplect_type)
                {
                    compositionSets = new List<List<int>>();
                    timeSets = new List<List<int>>();
                    foreach (var n_kom_q in N_komplect_for_type)
                    {

                        for (int i = 0; i < n_kom; i++)
                        {
                            var temp = rand.Next(10);
                            temp = (temp > 5) ? 150 : 100;

                            compositionSets.Add(new List<int>());
                            timeSets.Add(new List<int>());
                            for (int j = 0; j < n_kom_q; j++)
                                timeSets[i].Add((j + 1) * temp);

                        }

                        foreach (var n_kom_s in N_komplect_sostav)
                        {
                            foreach (var _dataTypesCount in _dataTypesCountArray)
                            {

                                for (int i = 0; i < n_kom; i++)
                                    for (var dataType = 0; dataType < _dataTypesCount; dataType++)
                                        compositionSets[i].Add((rand.Next(10) > 5) ? n_kom_s : 2);

                                foreach (var _deviceCount in _deviceCountArray)
                                {
                                    file.WriteLine($"Kn =\t{n_kom}");
                                    file.WriteLine($"Kq =\t{n_kom_q}");
                                    file.WriteLine($"Kqs=\t{n_kom_s}");
                                    file.WriteLine($"N  =\t{_dataTypesCount};\tL = {_deviceCount}");

                                    // Перебираем каждой элемент вектора, как время выполнения задания
                                    foreach (var _maxProccessingTime in time)
                                    {
                                        
                                        // Перебираем каждой элемент вектора, как время переналадки приборов
                                        foreach (var _maxChangeoverTime in time)
                                        {

                                            // Формируем расписание
                                            Shedule.deviceCount = _deviceCount;
                                            Shedule.changeoverTime = OldChangeoverTimeGenerator(_maxChangeoverTime, _deviceCount, _dataTypesCount);
                                            Shedule.proccessingTime = OldProccessingTimeGenerator(_maxProccessingTime, _deviceCount, _dataTypesCount);

                                            // Создаём экземпляр конфигурационной структуры
                                            Config config = new Config(
                                                _dataTypesCount,
                                                _deviceCount,
                                                buffer,
                                                (Matrix)(new Matrix(OldProccessingTimeGenerator(_maxProccessingTime, _deviceCount, _dataTypesCount))),
                                                Config.ChangeoverTimeConverter(OldChangeoverTimeGenerator(_maxChangeoverTime, _deviceCount, _dataTypesCount)),
                                                isFixedBatches
                                            );

                                            // Создаём вектор длиной dataTypesCount из 0
                                            var batchCountList = CreateBatchCountList(0, _dataTypesCount);

                                            for (var dataType = 0; dataType < _dataTypesCount; dataType++)
                                            {
                                                batchCount = 0;
                                                for (int i = 0; i < n_kom; i++)
                                                    batchCount += compositionSets[i][dataType] * n_kom_q;

                                                batchCountList[dataType] = batchCount;
                                            }

                                            var gaa = new GAA(_dataTypesCount, batchCountList, isFixedBatches, batchCount);
                                            
                                            var result = gaa.calcSetsFitnessList(checkBox_deadline_on.Checked, generationCount, xromossomiSize);

                                            file.WriteLine(result);
                                        }
                                    }
                                    file.WriteLine();
                                }
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Данные успешно записаны", "Успешное завершение", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Данная функция вызывается при нажатии кнопки "Тест комплектов"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetsBtn_Click(object sender, EventArgs e)
        {

            // Выполняем перестроение матриц
            TablesRebuild();
        }

        #endregion

        #region Выбор способа обработки данных

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.TournamentSelection (Турнирная селекция)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_TournamentSelection_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.TournamentSelection;
        }

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.RouletteMethod (Метод рулетки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_RouletteMethod_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.RouletteMethod;
        }

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.UniformRanking (Равномерное ранжирование)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_UniformRanking_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.UniformRanking;
        }

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.SigmaClipping (Сигма отсечение)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_SigmaClipping_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.SigmaClipping;
        }

        #endregion

        #region Обработка рандомизаций и выбора вкладки

        /// <summary>
        /// Данная функция выполняет перестроение таблиц
        /// </summary>
        private void TablesRebuild()
        {

            // Определяем необходимость переопределения данных таблиц
            if (!isValueChagedToUpdate)
                return;

            // Отчищаем внутреннии таблицы
            changeoverTime.Clear();
            proccessingTime.Clear();

            // Выполняем инициализацию графических таблиц
            InitDataGridView();

            // Выполняем инициализацию внутренних таблиц
            InitTables();

            // Выполяем рандомизацию для таблицы времени обработки
            RandomizeProcessingTime_Click();

            // Выполняем рандомизацию для таблицы времени переналадки приборов
            RandomizeChangeoverTime_Click();

            // Выполняем рандомизацию для таблицы времени ПТО
            RandomizePreMaintenanceTime_Click();

            // Сбрасываем необходимость переопределения данных таблиц
            isValueChagedToUpdate = false;
        }

        /// <summary>
        /// Данная функция обрабатываем переключение между вкладками "Установка параметров" и "Установка времени"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_time_setup_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // TODO: БАГ. Проблема вызова. При переопределении переменных не переопределяются матрицы
            // Вычисляем выбранную вкладку
            switch (e.TabPageIndex)
            {

                // Вкладка "Установка времени"
                case 0:

                    // При переключении на данную вкладку считываем новые данные с таблиц
                    GetTime();
                    break;

                // Вкладка "Установка параметров"
                case 1:

                    // При переключении на данную вкладку в случае изменений параметров таблицы выполняем обработку
                    TablesRebuild();

                    break;

                // Невозможный случай
                default:
                    MessageBox.Show("Шойтан, как ты суда попал?");
                    break;
            }
        }

        /// <summary>
        /// Данная функция выполняет рандомизацию таблицы слева (таблица времени обработки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomizeProcessingTime_Click(object sender = null, EventArgs e = null)
        {
            // Формируем рандомайзер на основе введённого значение
            // Random rand = new Random(randomValue);

            // Формируем второе рандомное значение, как половина от введённого
            // TODO: Выполнить инициализацию второй переменной с введённого числа с View.
            int _randomValue = randomValue / 2;

            // Для каждого прибора (проходимся по всем строкам таблицы dataGridView_proccessing_time)
            for (int device = 0; device < dataGridView_proccessing_time.RowCount; device++)
            {

                // Выполняем наименование строки
                dataGridView_proccessing_time.Rows[device].HeaderCell.Value = string.Format("Device {0}", device);

                // Чередуем два рандомных значения _randomValue и randomValue с каждой новой строкой
                int value = (device % 2 == 0) ? _randomValue : randomValue;

                // Для каждого типа данных (проходися по всем столбцам таблица dataGridView_proccessing_time)
                for (int dataType = 0; dataType < dataGridView_proccessing_time.ColumnCount; dataType++)
                {

                    // Выполняем наименование строки
                    dataGridView_proccessing_time.Columns[dataType].HeaderCell.Value = string.Format("Type {0}", dataType);


                    // Чередуем два рандомных значения _randomValue и randomValue с каждой новой колонкой
                    value = (value == _randomValue) ? randomValue : _randomValue;

                    // Присваиваем новое рандомное значение в таблице dataGridView_proccessing_time
                    dataGridView_proccessing_time.Rows[device].Cells[dataType].Value = value;
                }
            }

            // Сохраняем сгенерированные данные
            GetTime();
        }

        /// <summary>
        /// Данная функция выполняет рандомизацию таблицы посередине (таблица времени переналадки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomizeChangeoverTime_Click(object sender = null, EventArgs e = null)
        {
            // Формируем рандомайзер на основе введённого значение
            // Random rand = new Random(randomValue);

            // Формируем второе рандомное значение, как половина от введённого
            int _randomValue = randomValue / 2;

            // Для каждого устройства
            for (int device = 0; device < deviceCount; device++)
            {

                // Для каждого типа данных (проходимся по всем строкам таблицы dataGridView_changeover_time)
                for (int row_dataType = 0; row_dataType < dataTypesCount; row_dataType++)
                {

                    // Выполняем наименование строки
                    dataGridView_changeover_time.Rows[row_dataType + device * dataTypesCount].HeaderCell.Value = string.Format("Type {0}", row_dataType);

                    // Чередуем два рандомных значения _randomValue и randomValue с каждой новой строкой
                    int value = (row_dataType % 2 == 0) ? _randomValue : randomValue;

                    // Для каждого типа данных (проходися по всем столбцам таблица dataGridView_changeover_time)
                    for (int col_dataType = 0; col_dataType < dataTypesCount; col_dataType++)
                    {

                        // Выполняем наименование столбцов
                        if (device == 0)
                            dataGridView_changeover_time.Columns[col_dataType].HeaderCell.Value = string.Format("Type {0}", col_dataType);

                        // Когда индекс по строке равен индексу по столбцу
                        if (row_dataType == col_dataType)
                        {

                            // Присваиваем значение 0 таблице dataGridView_changeover_time
                            dataGridView_changeover_time.Rows[row_dataType + device * dataTypesCount].Cells[col_dataType].Value = 0;

                            // Продолжаем обработку
                            continue;
                        }

                        // Чередуем два рандомных значения _randomValue и randomValue с каждой новой колонкой
                        value = (value == _randomValue) ? randomValue : _randomValue;

                        // Присваиваем новое рандомное значение в таблице dataGridView_changeover_time
                        dataGridView_changeover_time.Rows[row_dataType + device * dataTypesCount].Cells[col_dataType].Value = value;
                    }
                }
            }

            // Сохраняем сгенерированные данные
            GetTime();
        }

        /// <summary>
        /// Данная функция выполняем рандомизацию таблицы справа (таблица времени ПТО)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomizePreMaintenanceTime_Click(object sender = null, EventArgs e = null)
        {

            // Создаём объект для генерации рандомного значения
            var random = new Random(randomValue);

            // Формируем второе рандомное значение, как половина от введённого
            int _randomValue = randomValue / 2;

            // Устанавливаем заголовок столбца
            dataGridView_pre_maintenance.Columns[0].HeaderCell.Value = "Time";
            dataGridView_preM_failureRates.Columns[0].HeaderCell.Value = "Time";
            dataGridView_preM_restoringDevice.Columns[0].HeaderCell.Value = "Time";

            // Для каждого устройства
            for (int device = 0; device < deviceCount; device++)
            {

                // Устанавливаем заголовок строки
                dataGridView_pre_maintenance.Rows[device].HeaderCell.Value = string.Format("Device {0}", device);
                dataGridView_preM_failureRates.Rows[device].HeaderCell.Value = string.Format("Device {0}", device);
                dataGridView_preM_restoringDevice.Rows[device].HeaderCell.Value = string.Format("Device {0}", device);

                dataGridView_pre_maintenance.Rows[device].Cells[0].Value = (device % 2 == 0) ? randomValue : _randomValue;
                dataGridView_preM_failureRates.Rows[device].Cells[0].Value = random.NextDouble() % 0.01;
                dataGridView_preM_restoringDevice.Rows[device].Cells[0].Value = random.NextDouble();
            }

            GetTime();
        }
        #endregion

        #region Определяем входные параметры

        /// <summary>
        /// Данная функция обрабатываем копирования данных в таблицы dataGridView_changeover_time с первого устройства на все остальные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyPreprocessingTime_Click(object sender, EventArgs e)
        {

            // Для всех строк после первого устройства выполняем обработку
            for (int rowIndex = dataTypesCount; rowIndex < dataTypesCount * deviceCount; rowIndex++)
            {

                // Определяем обрабатываемую строку
                var row = dataGridView_changeover_time.Rows[rowIndex];

                // Для всех столбцов выполняем выполняем обработку
                for (int columnIndex = 0; columnIndex < row.Cells.Count; columnIndex++)

                    // Определяем новое значение в ячейке, как скопированное с первого устройства
                    dataGridView_changeover_time.Rows[rowIndex].Cells[columnIndex].Value = dataGridView_changeover_time.Rows[rowIndex % dataTypesCount].Cells[columnIndex].Value;
            }
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_batch_count и записывает их в batchCount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_batch_count_ValueChanged(object sender, EventArgs e)
        {
            batchCount = Convert.ToInt32(numeric_batch_count.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_max_changeover_time и записывает их в maxChangeoverTime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_max_changeover_time_ValueChanged(object sender, EventArgs e)
        {
            // maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_max_proccessing_time и записывает их в maxProccessingTime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_max_proccessing_time_ValueChanged(object sender, EventArgs e)
        {
            // maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_data_types_count и записывает их в dataTypesCount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_data_types_count_ValueChanged(object sender, EventArgs e)
        {
            dataTypesCount = Convert.ToInt32(numeric_data_types_count.Value);
            isValueChagedToUpdate = true;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_buffer и записывает их в buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_buffer_ValueChanged(object sender, EventArgs e)
        {
            buffer = Convert.ToInt32(numeric_buffer.Value);
            Form1.buff = buffer;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_device_count и записывает их в deviceCount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_device_count_ValueChanged(object sender, EventArgs e)
        {
            deviceCount = Convert.ToInt32(numeric_device_count.Value);
            isValueChagedToUpdate = true;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента checkBox_fixed_batches и записывает их в isFixedBatches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_fixed_batches_CheckedChanged(object sender, EventArgs e)
        {
            isFixedBatches = checkBox_fixed_batches.Checked;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента checkBox_optimization и записывает их в isOptimization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_optimization_CheckedChanged(object sender, EventArgs e)
        {
            isOptimization = checkBox_optimization.Checked;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента checkBox_visualization и записывает их в vizualizationOn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_visualization_CheckedChanged(object sender, EventArgs e)
        {
            Form1.vizualizationOn = checkBox_visualization.Checked;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_random и записывает их в randomValue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_random_ValueChanged(object sender, EventArgs e)
        {
            randomValue = Convert.ToInt32(numeric_random.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_generation_count и записывает их в generation_count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_generation_count_ValueChanged(object sender, EventArgs e)
        {
            generationCount = Convert.ToInt32(numeric_generation_count.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_xromossomi_size и записывает их в xromossomiSize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            xromossomiSize = Convert.ToInt32(numeric_xromossomi_size.Value);
        }

        #endregion

        #endregion

        #region Вспомогательные функции

        /// <summary>
        /// Данная функция возвращает вектор длиной в количество типов данных состоящий из максимального размера каждого типа данных.
        /// Например для _dataTypesCount = 3 и _batchCount = 2 вернёт [2,2,2].
        /// Если вызвать функцию с аргументами по умолчанию, то в качестве значений _batchCount и _dataTypesCount будут 
        /// взяты локальные значения batchCount и dataTypesCount.
        /// </summary>
        /// <param name="_batchCount">Максимальное количество элементов для любого типа данных</param>
        /// <param name="_dataTypesCount">Максимальное количество типов данных</param>
        /// <returns>Вектор длиной _dataTypesCount, каждый элемент которого равен _batchCount</returns>
        private List<int> CreateBatchCountList(int _batchCount = -1, int _dataTypesCount = 0)
        {
            // Проверяем аргументы на корректные параметры
            _batchCount = _batchCount == -1 ? batchCount : _batchCount;
            _dataTypesCount = _dataTypesCount == 0 ? dataTypesCount : _dataTypesCount;

            // Объявляем переменную списка содержащую количество элементов каждого типа
            List<int> batchCountList = new List<int>(_dataTypesCount);
            batchCountList.AddRange(Enumerable.Repeat(_batchCount, _dataTypesCount));

            // Возвращаем список количества элементов каждого типа
            return batchCountList;
        }

        /// <summary>
        /// Данная функция генерирует матрицу времён выполнения заданий. Данный участок кода использовался в более ранней версии программы и назывался RandomTime
        /// <param name="_maxChangeoverTime">Устанавливаем ограничения для генерции данных</param>
        /// <param name="_deviceCount">Устанавливаем количество приборов для размерности матрицы</param>
        /// <param name="_dataTypesCount">Устанавливаем количество типов данных для размерности матрицы</param>
        /// <returns>Возвращаем матрицу времён выполнения заданий</returns>
        private List<List<int>> OldProccessingTimeGenerator(int _maxChangeoverTime, int _deviceCount, int _dataTypesCount)
        {
            // Объявляем переменную для хранения матрицы времени выполнения заданий
            List<List<int>> _proccessingTime = new List<List<int>>();

            // Инициализируем переменную для генерации данных
            int count = 0;

            // Для каждого устройства выполняем генерацию
            for (var device = 0; device < _deviceCount; device++)
            {

                // Выполняем инициализацию матрицы
                _proccessingTime.Add(new List<int>());

                // Для каждого типа данных выполняем генерацию
                for (var dataType = 0; dataType < _dataTypesCount; dataType++)
                    _proccessingTime[device].Add((count++ % 2 == 0) ? 2 : _maxChangeoverTime);
            }

            // Возвращаем матрицу времён выполнения
            return _proccessingTime;
        }

        /// <summary>
        /// Данная функция генерирует матрицу времён переналадки приборов. Данный участок кода использовался в более ранней версии программы и назывался RandomTime
        /// <param name="_maxChangeoverTime">Устанавливаем ограничения для генерции данных</param>
        /// <param name="_deviceCount">Устанавливаем количество приборов для размерности матрицы</param>
        /// <param name="_dataTypesCount">Устанавливаем количество типов данных для размерности матрицы</param>
        /// <returns>Возвращаем матрицу времён переналадки приборов</returns>
        private List<List<List<int>>> OldChangeoverTimeGenerator(int _maxChangeoverTime, int _deviceCount, int _dataTypesCount)
        {
            // Объявляем переменную для хранения матрицы времени переналадки приборов
            List<List<List<int>>> _changeoverTime = new List<List<List<int>>>();

            // Инициализируем переменную для генерации данных
            int count = 0;

            // Для каждого устройства выполняем генерацию
            for (var device = 0; device < _deviceCount; device++)
            {

                // Выполняем инициализацию матрицы
                _changeoverTime.Add(new List<List<int>>());

                // Для каждого типа данных выполняем генерацию
                for (var row_dataType = 0; row_dataType < _dataTypesCount; row_dataType++)
                {

                    // Выполняем инициализацию матрицы
                    _changeoverTime[device].Add(new List<int>());

                    // Для каждого типа данных выполняем генерацию
                    for (var col_dataType = 0; col_dataType < _dataTypesCount; col_dataType++)
                        _changeoverTime[device][row_dataType].Add((count % 2 == 0) ? 2 : _maxChangeoverTime);

                    // Увеличиваем счётчик
                    count++;
                }
            }

            // Возвращаем матрицу времён переналадки приборов
            return _changeoverTime;
        }

        /// <summary>
        /// Данная функция инициализирует все необходимые, для работы приложения, переменные
        /// </summary>
        private void InitializeForm()
        {
            // Инициализируем переменные с внешних компонентов
            // maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
            // maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
            dataTypesCount = Convert.ToInt32(numeric_data_types_count.Value);
            xromossomiSize = Convert.ToInt32(numeric_xromossomi_size.Value);
            deviceCount = Convert.ToInt32(numeric_device_count.Value);
            batchCount = Convert.ToInt32(numeric_batch_count.Value);
            randomValue = Convert.ToInt32(numeric_random.Value);
            buffer = Convert.ToInt32(numeric_buffer.Value);
            isFixedBatches = checkBox_fixed_batches.Checked;
            isOptimization = checkBox_optimization.Checked;
            selectoinType = SelectoinType.Undefined;
            Form1.vizualizationOn = false;
            isValueChagedToUpdate = false;
            Form1.buff = buffer;

            // Выполняем проверку на наличие Excel на компьютере
            if (Type.GetTypeFromProgID("Excel.Application") == null)

                // Выключаем графический компонент checkBox_visualization, если Excel нет на компьютере
                checkBox_visualization.Enabled = false;

            // Инициализируем перменные для данных таблиц
            changeoverTime = new List<List<List<int>>>();
            proccessingTime = new List<List<int>>();
            preMaintenanceTimes = new List<int>();
            
            // Отчищаем список
            failureRates?.Clear();
            failureRates = new List<double>(deviceCount);
            failureRates.AddRange(Enumerable.Repeat(0.0, deviceCount));

            // Отчищаем список
            restoringDevice?.Clear();
            restoringDevice = new List<double>(deviceCount);
            restoringDevice.AddRange(Enumerable.Repeat(0.0, deviceCount));

            // Выполняем инициализацию графических таблиц
            InitDataGridView();

            // Выполняем инициализацию внутренних таблиц
            InitTables();

            // Выполяем рандомизацию для таблицы времени обработки
            RandomizeProcessingTime_Click();

            // Выполняем рандомизацию для таблицы времени переналадки приборов
            RandomizeChangeoverTime_Click();

            // Выполняем рандомизацию для таблиц ПТО
            RandomizePreMaintenanceTime_Click();
        }

        /// <summary>
        /// Данная функция выполняет инициализацию внутренних таблиц
        /// </summary>
        private void InitTables()
        {
            // Инициализируем вектор времени ПТО
            preMaintenanceTimes  = ListUtils.InitVectorInt(deviceCount, 0);
            failureRates = new List<double>(deviceCount);// ListUtils.InitVectorInt(deviceCount, 0);
            restoringDevice = new List<double>(deviceCount);// ListUtils.InitVectorInt(deviceCount, 0);

            // Для каждого прибора выполняем обработку
            for (var device = 0; device < deviceCount; device++)
            {

                // Инициализируем новые элементы в таблицы
                proccessingTime.Add(new List<int>());
                changeoverTime.Add(new List<List<int>>());

                // Для каждого типа данных выполняем обработку
                for (var row_dataType = 0; row_dataType < dataTypesCount; row_dataType++)
                {

                    // Инициализируем новые элементы в таблицы
                    proccessingTime[device].Add(0);
                    changeoverTime[device].Add(new List<int>());

                    // Для каждого типа данных выполняем обработку
                    for (var col_dataType = 0; col_dataType < dataTypesCount; col_dataType++)
                        changeoverTime[device][row_dataType].Add(0);
                }
            }
        }

        /// <summary>
        /// Данная функция переопределяет графические таблицы в зависимости от структуры сети
        /// </summary>
        private void InitDataGridView()
        {

            // Удаляем структуру таблиц
            dataGridView_proccessing_time.Rows.Clear();
            dataGridView_proccessing_time.Columns.Clear();
            dataGridView_changeover_time.Rows.Clear();
            dataGridView_changeover_time.Columns.Clear();
            dataGridView_pre_maintenance.Rows.Clear();
            dataGridView_pre_maintenance.Columns.Clear();
            dataGridView_preM_failureRates.Rows.Clear();
            dataGridView_preM_failureRates.Columns.Clear();
            dataGridView_preM_restoringDevice.Rows.Clear();
            dataGridView_preM_restoringDevice.Columns.Clear();

            // Определяем новую структуру
            dataGridView_proccessing_time.RowCount = deviceCount;
            dataGridView_proccessing_time.ColumnCount = dataTypesCount;
            dataGridView_changeover_time.RowCount = deviceCount * dataTypesCount;
            dataGridView_changeover_time.ColumnCount = dataTypesCount;

            // Определяем новую таблицу времени ПТО
            dataGridView_pre_maintenance.RowCount = deviceCount;
            dataGridView_pre_maintenance.ColumnCount = 1;
            dataGridView_preM_failureRates.RowCount = deviceCount;
            dataGridView_preM_failureRates.ColumnCount = 1;
            dataGridView_preM_restoringDevice.RowCount = deviceCount;
            dataGridView_preM_restoringDevice.ColumnCount = 1;
        }

        /// <summary>
        /// Данная функция выполняет считывание данных из графических таблиц во внутреннии
        /// </summary>
        private bool GetTime()
        {

            // Отчищаем список
            failureRates?.Clear();
            failureRates = new List<double>(deviceCount);
            failureRates.AddRange(Enumerable.Repeat(0.0, deviceCount));

            // Отчищаем список
            restoringDevice?.Clear();
            restoringDevice = new List<double>(deviceCount);
            restoringDevice.AddRange(Enumerable.Repeat(0.0, deviceCount));

            // Для каждого прибора выполняем считывание
            for (int device = 0; device < deviceCount; device++)
            {

                // Считываем данные во внутрнние таблицы
                preMaintenanceTimes[device] = Convert.ToInt32(dataGridView_pre_maintenance.Rows[device].Cells[0].Value);

                // Считываем элемент 
                failureRates[device] = Convert.ToDouble(dataGridView_preM_failureRates.Rows[device].Cells[0].Value);
                if (failureRates[device] > 1) {
                    MessageBox.Show("Ошибка, значение не может быть больше 1");
                    return false;
                } else if (failureRates[device] < 0)
                {
                    MessageBox.Show("Ошибка, значение не может быть отрицательным");
                    return false;
                }

                restoringDevice[device] = Convert.ToDouble(dataGridView_preM_restoringDevice.Rows[device].Cells[0].Value);
                if (restoringDevice[device] > 1) {
                    MessageBox.Show("Ошибка, значение не может быть больше 1");
                    return false;
                } else if (restoringDevice[device] < 0) {
                    MessageBox.Show("Ошибка, значение не может быть отрицательным");
                    return false;
                }

                // Для каждого типа данных выполняем считывание
                for (int row_dataType = 0; row_dataType < dataTypesCount; row_dataType++)
                {

                    // Считываем данные во внутрнние таблицы
                    proccessingTime[device][row_dataType] = Convert.ToInt32(dataGridView_proccessing_time.Rows[device].Cells[row_dataType].Value);

                    // Для каждого типа данных выполняем считывание
                    for (var col_dataType = 0; col_dataType < dataTypesCount; col_dataType++)

                        // Считываем данные во внутрнние таблицы
                        changeoverTime[device][row_dataType][col_dataType] = Convert.ToInt32(dataGridView_changeover_time.Rows[row_dataType + device * dataTypesCount].Cells[col_dataType].Value);
                }
            }

            // Возвращяем флаг успешного считываения данных
            return true;
        }

        #endregion

        #region Страшно это трогать
        /*
        /// <summary>
        /// Неизвестный кусок кода, который страшно удалять, даже если он нигде не используется
        /// </summary>
        /// <param name="_in">Неясно почему и зачем мы это делаем, но так делали наши предшественники, давайте будет уважать их труд и оставим данный кусок кода. Или нет.</param>
        /// <returns></returns>
        private List<List<int>> СоставыПартий(IReadOnlyList<List<int>> _in)
        {
            var a = new List<List<int>>();
            var count = 0;
            a.Add(new List<int>());
            a[count] = Copy(_in[0]);
            count++;
            foreach (var t in _in)
            {
                for (var j = 1; j < t.Count; j++)
                {
                    do
                    {
                        a.Add(new List<int>());
                        a[count] = Copy(a[count - 1]);
                        a[count][0]--;
                        a[count][j]++;
                        count++;
                    }
                    while (a[count - 1][0] > a[count - 1][j]);
                }
            }
            return a;
        }
        */
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void GetPreMSolution_Click(object sender, EventArgs e)
        {
            // Преобразуем входные данные в число типа Double
            // double beta = Convert.ToDouble(betaValue.Text);
            double.TryParse(betaValue.Text, out double beta);

            // Если beta будет больше 1
            if (beta > (double)1.0)
            {
                MessageBox.Show("Ошибка. Значение beta не может быть больше 1");
                return;
            }

            // Если beta будет меньше 0
            if (beta < (double)0.0)
            {
                MessageBox.Show("Ошибка. Значение beta не может быть меньше 0");
                return;
            }

            // Инициализируем расписание
            Shedule.deviceCount = deviceCount;
            Shedule.changeoverTime = changeoverTime;
            Shedule.proccessingTime = proccessingTime;

            // Создаём экземпляр конфигурационной структуры
            Config config = new Config(
                dataTypesCount,
                deviceCount,
                buffer,
                (Matrix)(new Matrix(proccessingTime)),
                Config.ChangeoverTimeConverter(changeoverTime),
                isFixedBatches
            );
            
            // Создаём экземпляр конфигурационной структуры для ПТО
            PreMConfig preMConfig = new PreMConfig(
                config,
                preMaintenanceTimes,
                failureRates,
                restoringDevice,
                beta
            );

            // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
            List<int> batchCountList = CreateBatchCountList();

            // Формируем первый уровень
            var firstLevel = new FirstLevel(config, batchCountList);

            // Выполняем генерацию данных для ввсех типов вторым алгоритмом
            firstLevel.GenetateSolutionWithPremaintenance("Premintenance", preMConfig);
        }
    }
}