using magisterDiplom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

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
        /// Данная переменная представляет из себя максимальное время переналадки приборов
        /// </summary>
        int maxChangeoverTime;

        /// <summary>
        /// Данная переменная представляет из себя максимальное время выполнения задания
        /// </summary>
        int maxProccessingTime;

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
        int generation_count;

        public static int buff;
        List<List<List<int>>> changeoverTime = new List<List<List<int>>>();
        List<List<int>> proccessingTime = new List<List<int>>();

        public static bool direct;

        public static List<List<int>> compositionSets;//майкрософт кодстайл говорит юзать верблюжий стиль https://msdn.microsoft.com/en-us/library/ms229043%28v=vs.100%29.aspx
        public static List<List<int>> timeSets;

        #endregion

        public Form1()
        {
            InitializeComponent();
            InitializeForm();

            /*
            var test = new List<List<int>>();
            test.Add(new List<int>());
            test.Add(new List<int>(){2});
            test.Add(new List<int>(){10, 2});
            var t = new Shedule(test);
            //Shedule.L = 5;
            //t.GetTime();
            //var a = t.ReturnRIndex(0);
            //var b = t.ReturnRIndex(1);
             */

        }

        /// <summary>
        /// Данная функция инициализирует все необходимые, для работы приложения, переменные
        /// </summary>
        private void InitializeForm()
        {
            // Инициализируем переменные с внешних компонентов
            maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
            maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
            dataTypesCount = Convert.ToInt32(numeric_data_types_count.Value);
            deviceCount = Convert.ToInt32(numeric_device_count.Value);
            batchCount = Convert.ToInt32(numeric_batch_count.Value);
            randomValue = Convert.ToInt32(numeric_random.Value);
            buffer = Convert.ToInt32(numeric_buffer.Value);
            isFixedBatches = checkBox_fixed_batches.Checked;
            isOptimization = checkBox_optimization.Checked;
            selectoinType = SelectoinType.Undefined;
            isValueChagedToUpdate = false;

            // Инициализируем перменные для данных таблиц
            changeoverTime = new List<List<List<int>>>();
            proccessingTime = new List<List<int>>();

            // Выполняем инициализацию графических таблиц
            InitDataGridView();

            // Выполняем инициализацию внутренних таблиц
            InitTables();

            // Выполяем рандомизацию для таблицы времени обработки
            randomizeProcessingTime_Click();

            // Выполняем рандомизацию для таблицы времени переналадки приборов
            randomizeChangeoverTime_Click();

            // Выполняем считывание данных во внутренние таблицы
            GetTime();
        }

        private List<int> Copy(IEnumerable<int> _in)
        {
            return _in.ToList();
        }

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

        /// <summary>
        /// Данная функция выполняет инициализацию внутренних таблиц
        /// </summary>
        private void InitTables()
        {
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

            // Определяем новую структуру
            dataGridView_proccessing_time.RowCount = deviceCount;
            dataGridView_proccessing_time.ColumnCount = dataTypesCount;
            dataGridView_changeover_time.RowCount = deviceCount * dataTypesCount;
            dataGridView_changeover_time.ColumnCount = dataTypesCount;

        }

        /// <summary>
        /// Данная функция выполняет считывание данных и графических таблиц во внутреннии
        /// </summary>
        private void GetTime()
        {

            // Для каждого прибора выполняем считывание
            for (int device = 0; device < deviceCount; device++)

                // Для каждого типа данных выполняем считывание
                for (int row_dataType = 0; row_dataType < dataTypesCount; row_dataType++) { 

                    // Считываем данные во внутрнние таблицы
                    proccessingTime[device][row_dataType] = Convert.ToInt32(dataGridView_proccessing_time.Rows[device].Cells[row_dataType].Value);

                    // Для каждого типа данных выполняем считывание
                    for (var col_dataType = 0; col_dataType < dataTypesCount; col_dataType++)

                        // Считываем данные во внутрнние таблицы
                        changeoverTime[device][row_dataType][col_dataType] = Convert.ToInt32(dataGridView_changeover_time.Rows[row_dataType + device * dataTypesCount].Cells[0].Value);
                }
        }

        /// <summary>
        /// Получить решение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
            List<int> batchCountList = CreateBatchCountList();

            GetTime();
            Shedule.conveyorLenght = deviceCount;
            Shedule.Switching = changeoverTime;
            Shedule.Treatment = proccessingTime;
            var firstLevel = new FirstLevel(dataTypesCount, batchCountList, isFixedBatches);
            firstLevel.GenetateSolutionForAllTypes("outputFirstAlgorithm.txt");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] N_komplect_type = {2};
            int[] N_komplect_for_type = {2, 4};
            int[] N_komplect_sostav = {2, 4};
            int[] n = {5, 10};
            int[] l = {5, 10};
            int[] time = {2, 4, 8, 16, 32};
            using (var file = new StreamWriter("11 - " + checkBox2.Checked + ".txt", false))
            {
                foreach (var n_kom in N_komplect_type)
                {
                    compositionSets = new List<List<int>>();
                    timeSets = new List<List<int>>();
                    foreach (var n_kom_q in N_komplect_for_type)
                    {
                        for (int i = 0; i < n_kom; i++)
                        {
                            var rand = new Random();
                            var temp = rand.Next(10);
                            if (temp > 5)
                            {
                                temp = 150;
                            }
                            else
                            {
                                temp = 100;
                            }
                            compositionSets.Add(new List<int>());
                            timeSets.Add(new List<int>());
                            for (int j = 0; j < n_kom_q; j++)
                            {
                                timeSets[i].Add((j + 1) * temp);
                            }
                        }
                        foreach (var n_kom_s in N_komplect_sostav)
                        {
                            foreach (var t in n)
                            {
                                dataTypesCount = t;
                                for (int i = 0; i < n_kom; i++)
                                {
                                    for (var ind = 0; ind < dataTypesCount; ind++)
                                    {
                                        var rand = new Random();
                                        var temp = rand.Next(10);
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

                                foreach (var _countLine in l)
                                {
                                    file.WriteLine("Kn=" + n_kom);
                                    file.WriteLine("Kq=" + n_kom_q);
                                    file.WriteLine("Kqs=" + n_kom_s);
                                    file.WriteLine("N=" + t + "L=" + _countLine);
                                    foreach (var t2 in time)
                                    {
                                        foreach (var t3 in time)
                                        {
                                            changeoverTime = new List<List<List<int>>>();
                                            proccessingTime = new List<List<int>>();
                                            deviceCount = _countLine;
                                            maxChangeoverTime = t3;
                                            maxProccessingTime = t2;
                                            InitTables();
                                            InitDataGridView();
                                            GetTime();
                                            Shedule.conveyorLenght = _countLine;
                                            Shedule.Switching = changeoverTime;
                                            Shedule.Treatment = proccessingTime;

                                            // Создаём вектор длиной dataTypesCount из 0
                                            var batchCountList = new List<int>();
                                            for (var dataType = 0; dataType < dataTypesCount; dataType++)
                                                batchCountList.Add(0);
                                            
                                            for (var dataType = 0; dataType < dataTypesCount; dataType++)
                                            {
                                                batchCount = 0;
                                                for (int i = 0; i < n_kom; i++)
                                                {
                                                    batchCount += compositionSets[i][dataType] * n_kom_q;
                                                }

                                                batchCountList[dataType] = batchCount;
                                            }

                                            var gaa = new GAA(dataTypesCount, batchCountList, isFixedBatches, batchCount);

                                            
                                            var result = gaa.calcSetsFitnessList(checkBox2.Checked, numeric_generation_count.Value, (int)numericUpDown2.Value);

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

            MessageBox.Show("Данные успешно записаны", "Учпешное завершение", MessageBoxButtons.OK);
        }
            
        private void Change()
        {
            try
            {
                dataTypesCount = (int)numeric_data_types_count.Value;
                deviceCount = Convert.ToInt32(numeric_device_count.Value);
                maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
                maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
                changeoverTime = new List<List<List<int>>>();
                proccessingTime = new List<List<int>>();
                InitTables();
                InitDataGridView();
            }
            catch 
            {
                return;
            }
        }

        /// <summary>
        /// Второй метод
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
            List<int> batchCountList = CreateBatchCountList();

            deviceCount = Convert.ToInt32(numeric_device_count.Value);
            GetTime();
            Shedule.conveyorLenght = deviceCount;
            Shedule.Switching = changeoverTime;
            Shedule.Treatment = proccessingTime;
            var firstLevel = new FirstLevel(dataTypesCount, batchCountList, isFixedBatches);
            firstLevel.GenetateSolutionForAllTypesSecondAlgorithm();
        }
        
        private void tabControl_time_setup_Selecting(object sender = null, TabControlCancelEventArgs e = null)
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
            randomizeProcessingTime_Click();

            // Выполняем рандомизацию для таблицы времени переналадки приборов
            randomizeChangeoverTime_Click();

            // Выполняем считывание данных во внутренние таблицы
            GetTime();
        }

        #region Выбор способа обработки данных

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.TournamentSelection (Турнирная селекция)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_TournamentSelection_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.TournamentSelection;
        }

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.RouletteMethod (Метод рулетки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_RouletteMethod_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.RouletteMethod;
        }

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.UniformRanking (Равномерное ранжирование)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_UniformRanking_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.UniformRanking;
        }

        /// <summary>
        /// Данная функция обрабатывает выбор обработки данных и отмечает выбора, как SelectoinType.SigmaClipping (Сигма отсечение)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_SigmaClipping_change(object sender, EventArgs e)
        {
            selectoinType = SelectoinType.SigmaClipping;
        }

        #endregion

        #region Определяем входные параметры

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_batch_count и записывает их в batchCount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_batch_count_ValueChanged(object sender, EventArgs e)
        {
            batchCount = Convert.ToInt32(numeric_batch_count.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_max_changeover_time и записывает их в maxChangeoverTime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_max_changeover_time_ValueChanged(object sender, EventArgs e)
        {
            maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_max_proccessing_time и записывает их в maxProccessingTime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_max_proccessing_time_ValueChanged(object sender, EventArgs e)
        {
            maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_data_types_count и записывает их в dataTypesCount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_data_types_count_ValueChanged(object sender, EventArgs e)
        {
            dataTypesCount = Convert.ToInt32(numeric_data_types_count.Value);
            isValueChagedToUpdate = true;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_buffer и записывает их в buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_buffer_ValueChanged(object sender, EventArgs e)
        {
            buffer = Convert.ToInt32(numeric_buffer.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_device_count и записывает их в deviceCount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_device_count_ValueChanged(object sender, EventArgs e)
        {
            deviceCount = Convert.ToInt32(numeric_device_count.Value);
            isValueChagedToUpdate = true;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента checkBox_fixed_batches и записывает их в isFixedBatches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_fixed_batches_CheckedChanged(object sender, EventArgs e)
        {
            isFixedBatches = checkBox_fixed_batches.Checked;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента checkBox_optimization и записывает их в isOptimization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_optimization_CheckedChanged(object sender, EventArgs e)
        {
            isOptimization = checkBox_optimization.Checked;
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_random и записывает их в randomValue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_random_ValueChanged(object sender, EventArgs e)
        {
            randomValue = Convert.ToInt32(numeric_random.Value);
        }

        /// <summary>
        /// Данная функция определяет входные параметры с графического компонента numeric_generation_count и записывает их в generation_count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numeric_generation_count_ValueChanged(object sender, EventArgs e)
        {
            generation_count = Convert.ToInt32(numeric_generation_count.Value);
        }
        #endregion


        private void OldSecondLevelButton_Click(object sender, EventArgs e)
        {
            var massi = new[] { 2, 4, 8, 16, 32 };
            using (var file = new StreamWriter("Фиксированные партии - " + isFixedBatches + ", оптимизации групп " + isOptimization + " N=" + numeric_data_types_count.Value.ToString() + " L=" + numeric_device_count.Value.ToString() + ".txt", false))
            {
                foreach (var intt in massi)
                {
                    numeric_max_changeover_time.Value = intt;
                    foreach (var item in massi)
                    {
                        for (var tz = 50; tz <= 200; tz = tz + 50)
                        {
                            for (var countGroup = 2; countGroup <= 8; countGroup += 2)
                            {


                                numeric_max_proccessing_time.Value = item;

                                // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
                                List<int> batchCountList = CreateBatchCountList();

                                deviceCount = Convert.ToInt32(numeric_device_count.Value);
                                maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
                                maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
                                GetTime();
                                Shedule.conveyorLenght = deviceCount;
                                Shedule.Switching = changeoverTime;
                                Shedule.Treatment = proccessingTime;
                                var firstLevel = new FirstLevel(dataTypesCount, batchCountList, isFixedBatches);
                                firstLevel.GenetateSolutionForAllTypes("outputFirstAlgorithm.txt");
                                var oldSecondLevel = new OldSecondLevel(tz, countGroup, deviceCount);

                                int criteria;
                                int flCrit;
                                var listInt = !isOptimization
                                    ? oldSecondLevel.CalcFitnessList(firstLevel._a, out criteria, out flCrit)
                                    : oldSecondLevel.CalcOptimalFitnessList(firstLevel._a, out criteria, out flCrit);
                                var stringTime = listInt.Select(i => i.ToString());
                                var join = string.Join(" ", stringTime);
                                file.WriteLine("Tz = " + tz.ToString() + Environment.NewLine +
                                               "Tp = " + intt + Environment.NewLine +
                                               "Z = " + countGroup + Environment.NewLine +
                                               "Crit = " + criteria + Environment.NewLine +
                                               "fLCrit = " + flCrit +  Environment.NewLine +
                                               "To = " + item  + Environment.NewLine +
                                               listInt.Sum().ToString() + join  + Environment.NewLine + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            MessageBox.Show(@"Решения найдены");
        }


        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void OldSecondLevelAll_Click(object sender, EventArgs e)
        {
            var massi = new[] { 2, 4, 8, 16, 32 };
            using (var file = new StreamWriter("Га, оптимизации групп " + isOptimization + " N=" + numeric_data_types_count.Value + "L=" + numeric_device_count.Value.ToString() + ".txt", false))
            {
                foreach (var intt in massi)
                {
                    numeric_max_changeover_time.Value = intt;
                    foreach (var item in massi)
                    {
                        for (var tz = 50; tz <= 200; tz = tz + 50)
                        {
                            for (var countGroup = 2; countGroup <= 8; countGroup += 2)
                            {


                                numeric_max_proccessing_time.Value = item;

                                // Инициализируем вектор длиной dataTypesCount, каждый элемент которого будет равен batchCount
                                List<int> batchCountList = CreateBatchCountList();

                                deviceCount = Convert.ToInt32(numeric_device_count.Value);
                                maxChangeoverTime = Convert.ToInt32(numeric_max_changeover_time.Value);
                                maxProccessingTime = Convert.ToInt32(numeric_max_proccessing_time.Value);
                                GetTime();
                                Shedule.conveyorLenght = deviceCount;
                                Shedule.Switching = changeoverTime;
                                Shedule.Treatment = proccessingTime;

                                var gaa = new GAA(dataTypesCount, batchCountList, isFixedBatches, batchCount);
                                gaa.SetXrom((int)numericUpDown2.Value);
                                var sostav = gaa.calcFitnessList();

                                //var firstLevel = new FirstLevel(_countType, batchCountList, checkBox1.Checked);
                                //firstLevel.GenetateSolutionForAllTypes("outputFirstAlgorithm.txt");
                                var oldSecondLevel = new OldSecondLevel(tz, countGroup, deviceCount);

                                int criteria;
                                int flCrit;
                                var listInt = !isOptimization
                                    ? oldSecondLevel.CalcFitnessList(sostav, out criteria, out flCrit)
                                    : oldSecondLevel.CalcOptimalFitnessList(sostav, out criteria, out flCrit);
                                var stringTime = listInt.Select(i => i.ToString());
                                var join = string.Join(" ", stringTime);
                                file.WriteLine("Tz = " + tz.ToString() + Environment.NewLine +
                                               "Tp = " + intt + Environment.NewLine +
                                               "Z = " + countGroup + Environment.NewLine +
                                               "Crit = " + criteria + Environment.NewLine +
                                               "fLCrit = " + flCrit +  Environment.NewLine +
                                               "To = " + item  + Environment.NewLine +
                                               listInt.Sum().ToString() + join  + Environment.NewLine + Environment.NewLine);
                            }
                        }
                    }
                }
            }
            MessageBox.Show(@"Решения найдены");
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Тестовый прогон
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Form1.direct = checkBox2.Checked;
            int[] N_komplect_sostav = { 2, 4 };
            int[] n = { 5, 10 }; // количество данных
            int[] l = { 5, 10 }; 
            int[] time = { 2, 4, 8, 16, 32 }; // время (чего?)

            var file = "test/testFile_";
            var str = "direct";
            if (!Form1.direct)
            {
                str = "first_task";
            }
            var n_kom = Convert.ToInt32(textBox1.Text);
            var rand = new Random((int)DateTime.Now.ToBinary());
            var temp = 0;
            var n_kom_q = Convert.ToInt32(textBox2.Text);
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
                    foreach (var t in n)
                    {
                        dataTypesCount = t;
                        compositionSets = new List<List<int>>();
                        for (int i = 0; i < n_kom; i++)
                        {
                            compositionSets.Add(new List<int>());
                            for (var ind = 0; ind < dataTypesCount; ind++)
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

                        foreach (var _countLine in l)
                        {
                            fileOut.WriteLine("Kq=" + n_kom_q);
                            fileOut.WriteLine("Kqs=" + n_kom_s);
                            fileOut.WriteLine("N=" + t + "L=" + _countLine);
                            fileOut.WriteLine("Times");
                            fileOut.WriteLine(printArray(timeSets));
                            fileOut.WriteLine("Compositions");
                            fileOut.WriteLine(printArray(compositionSets));
                            foreach (var t2 in time)
                            {
                                foreach (var t3 in time)
                                {
                                    changeoverTime = new List<List<List<int>>>(); // переналадка
                                    proccessingTime = new List<List<int>>(); // время обработки данных i-го типа
                                    deviceCount = _countLine; // количество сегментов
                                    maxChangeoverTime = t3;
                                    maxProccessingTime = t2;
                                    InitTables();
                                    InitDataGridView();
                                    GetTime();
                                    Shedule.conveyorLenght = _countLine;
                                    Shedule.Switching = changeoverTime;
                                    Shedule.Treatment = proccessingTime;

                                    var _batchCountList = new List<int>(); // Количество пакетов
                                    for (var ii = 0; ii < dataTypesCount; ii++)
                                    {
                                        _batchCountList.Add(0);
                                    }
                                    for (var ii = 0; ii < dataTypesCount; ii++)
                                    {
                                        batchCount = 0;
                                        for (int i = 0; i < n_kom; i++)
                                        {
                                            batchCount += compositionSets[i][ii];
                                        }
                                        _batchCountList[ii] = batchCount * n_kom_q;
                                    }
                                    var firstLevel = new FirstLevel(dataTypesCount, _batchCountList, isFixedBatches);
                                    var result = firstLevel.GenetateSolutionForAllTypesSecondAlgorithm();

                                    //var gaa = new GAA(_countType, listCountButches, checkBox1.Checked, batchCount);
                                    //var resultGaa = gaa.calcSetsFitnessList(checkBox2.Checked, GenerationCounter.Value, (int)numericUpDown2.Value);

                                    var first = result[0];
                                    var top = result[1];
                                    //fileOut.WriteLine(first + "\t" + top + "\t" + resultGaa);
                                    fileOut.WriteLine(first + "\t" + top);
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
        /// Данная функция выполняет рандомизацию таблицы слева (таблица времени обработки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void randomizeProcessingTime_Click(object sender = null, EventArgs e = null)
        {
            // Формируем рандомайзер на основе введённого значение
            // Random rand = new Random(randomValue);

            // Формируем второе рандомное значение, как половина от введённого
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
        /// Данная функция выполняет рандомизацию таблицы справа (таблица времени переналадки)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void randomizeChangeoverTime_Click(object sender = null, EventArgs e = null)
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

        private void copyPreprocessingTime_Click(object sender, EventArgs e)
        {
            int countType = (int)numeric_data_types_count.Value;
            int countDevices = Convert.ToInt32(numeric_device_count.Value);

            for (int rowIndex = countType; rowIndex < countType * countDevices; rowIndex++)
            {
                var row = dataGridView_changeover_time.Rows[rowIndex];
                for (int columnIndex = 0; columnIndex < row.Cells.Count; columnIndex++)
                {
                    dataGridView_changeover_time.Rows[rowIndex].Cells[columnIndex].Value = dataGridView_changeover_time.Rows[rowIndex % countType].Cells[columnIndex].Value;
                }
            }
        }

        private string printArray(List<List<int>> arr)
        {
            var str = "";
            foreach (var row in arr)
            {
                foreach (var column in row)
                {
                    str += column + "\t";
                }
                str += "\r\n";
            }
            return str;
        }

        /// <summary>
        /// Прогон
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            // Инициализируем выходной файл на добавление
            StreamWriter file_output_method_GAA = new StreamWriter("outputGAASimpleResult.txt", true);

            // Фиксируем начало обработки
            file_output_method_GAA.WriteLine($"Дата и время запуска: {DateTime.Now.ToString("O")}\n{{");

            // Формируем массив данных [8, 12, 16, 24, 32]
            int[] array = { 8, 12, 16, 24, 32 };

            // Для каждого элемента из массива array выполняем обработку
            foreach (int _batchCount in array)
            {

                // Для следующих количеств типов данных выполняем обработку
                for (int _dataTypesCount = 5; _dataTypesCount <= 10; _dataTypesCount += 5)
                {

                    // Для следующих количеств приборов (сегментов) выполняем обработку
                    for (int _conveyorLenght = 5; _conveyorLenght <= 10; _conveyorLenght += 5)
                    {
                        
                        // Выводим информацию в выходной файл
                        file_output_method_GAA.WriteLine("\tbatchCount(CB)   \t= {0}", _batchCount);
                        file_output_method_GAA.WriteLine("\tconveyorLength(L)\t= {0}", _conveyorLenght);
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
                                Shedule.conveyorLenght = _conveyorLenght;
                                Shedule.Switching = changeoverTime;
                                Shedule.Treatment = proccessingTime;

                                // Инициализируем вектор длиной _dataTypesCount, каждый элемент которого будет равен _batchCount
                                List<int> batchCountList = CreateBatchCountList(_batchCount, _dataTypesCount);

                                var gaa = new GAA(_dataTypesCount, batchCountList, isFixedBatches, _batchCount);
                                gaa.SetXrom((int)numericUpDown2.Value);
                                var countSourceKit = gaa.calcFitnessList();
                                int s;
                                var result = gaa.getSelectionPopulation(selectoinType, out s);

                                using (var file_outputGAA = new StreamWriter("outputGAA.txt", true))
                                {
                                    foreach (var elem in gaa.nabor)
                                    {

                                        foreach (var elem2 in elem.GenList)
                                        {
                                            foreach (var elem3 in elem2)
                                            {
                                                file_outputGAA.Write(elem3 + " ");
                                            }
                                            file_outputGAA.WriteLine();
                                        }
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
            file_output_method_GAA.WriteLine($"}}\nДата и время завершения: {DateTime.Now.ToString("O")}");
            file_output_method_GAA.Close();

            MessageBox.Show("Данные успешно записаны", "Успешное завершение", MessageBoxButtons.OK);
        }

        private void setsBtn_Click(object sender, EventArgs e)
        {
            var firstType = new List<int>();
            var secondType = new List<int>();
            var testTime = new List<int>();
            testTime.Add(15);
            testTime.Add(18);
            firstType.Add(3);
            firstType.Add(3);
            firstType.Add(3);
            secondType.Add(2);
            secondType.Add(3);
            secondType.Add(4);
            var testType = new List<List<int>>();
            testType.Add(firstType);
            testType.Add(secondType);
        }

        private void group_box_data_proccessing_Enter(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Данная функция возвращает вектор длиной в количество типов данных состоящий из максимального размера каждого типа данных.
        /// Например для _dataTypesCount = 3 и _batchCount = 2 вернёт [2,2,2].
        /// Если вызвать функцию с аргументами по умолчанию, то в качестве значений _batchCount и _dataTypesCount будут 
        /// взяты локальные значения batchCount и dataTypesCount.
        /// </summary>
        /// <param name="_batchCount">Максимальное количество элементов для любого типа данных</param>
        /// <param name="_dataTypesCount">Максимальное количество типов данных</param>
        /// <returns>Вектор длиной _dataTypesCount, каждый элемент которого равен _batchCount</returns>
        private List<int> CreateBatchCountList(int _batchCount = 0, int _dataTypesCount = 0)
        {
            // Проверяем аргументы на корректные параметры
            _batchCount = _batchCount == 0 ? batchCount : _batchCount;
            _dataTypesCount = _dataTypesCount == 0 ? dataTypesCount : _dataTypesCount;

            // Объявляем переменную списка содержащую количество элементов каждого типа
            List<int> batchCountList = new List<int>();
            for (int dataType = 0; dataType < _dataTypesCount; dataType++)
                batchCountList.Add(_batchCount);

            // Возвращаем список количества элементов каждого типа
            return batchCountList;
        }

    }

}