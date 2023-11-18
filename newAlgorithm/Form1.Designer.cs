namespace newAlgorithm
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.numeric_data_types_count = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox_system_setup = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.numeric_generation_count = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.group_box_data_proccessing = new System.Windows.Forms.GroupBox();
            this.radioButton_TournamentSelection = new System.Windows.Forms.RadioButton();
            this.radioButton_RouletteMethod = new System.Windows.Forms.RadioButton();
            this.radioButton_SigmaClipping = new System.Windows.Forms.RadioButton();
            this.radioButton_UniformRanking = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.numeric_device_count = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.numeric_max_proccessing_time = new System.Windows.Forms.NumericUpDown();
            this.checkBox_optimization = new System.Windows.Forms.CheckBox();
            this.numeric_batch_count = new System.Windows.Forms.NumericUpDown();
            this.OldSecondLevelAll = new System.Windows.Forms.Button();
            this.OldSecondLevelButton = new System.Windows.Forms.Button();
            this.numeric_max_changeover_time = new System.Windows.Forms.NumericUpDown();
            this.button5 = new System.Windows.Forms.Button();
            this.numeric_xromossomi_size = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numeric_buffer = new System.Windows.Forms.NumericUpDown();
            this.checkBox_fixed_batches = new System.Windows.Forms.CheckBox();
            this.setsBtn = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.randomizeProcessingTime = new System.Windows.Forms.Button();
            this.numeric_random = new System.Windows.Forms.NumericUpDown();
            this.dataGridView_proccessing_time = new System.Windows.Forms.DataGridView();
            this.copyPreprocessingTime = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.dataGridView_changeover_time = new System.Windows.Forms.DataGridView();
            this.randomizePreprocessingTime = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_data_types_count)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox_system_setup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_generation_count)).BeginInit();
            this.group_box_data_proccessing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_device_count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_max_proccessing_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_batch_count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_max_changeover_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_xromossomi_size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_buffer)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_random)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_proccessing_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_changeover_time)).BeginInit();
            this.SuspendLayout();
            // 
            // numeric_data_types_count
            // 
            this.numeric_data_types_count.Location = new System.Drawing.Point(197, 57);
            this.numeric_data_types_count.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numeric_data_types_count.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numeric_data_types_count.Name = "numeric_data_types_count";
            this.numeric_data_types_count.Size = new System.Drawing.Size(45, 20);
            this.numeric_data_types_count.TabIndex = 0;
            this.numeric_data_types_count.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numeric_data_types_count.ValueChanged += new System.EventHandler(this.numeric_data_types_count_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(542, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 38);
            this.button1.TabIndex = 2;
            this.button1.Text = "Получить решение";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Количество типов данных";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Количество данных каждого типа";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(188, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Максимальное время переналадки";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Максимальное время обработки";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(189, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Количество приборов на конвейере";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(268, 155);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Формирование Гаа";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(771, 500);
            this.tabControl.TabIndex = 12;
            this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_time_setup_Selecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox_system_setup);
            this.tabPage1.Controls.Add(this.setsBtn);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(763, 474);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Установка параметров";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // groupBox_system_setup
            // 
            this.groupBox_system_setup.Controls.Add(this.numeric_data_types_count);
            this.groupBox_system_setup.Controls.Add(this.textBox2);
            this.groupBox_system_setup.Controls.Add(this.numeric_generation_count);
            this.groupBox_system_setup.Controls.Add(this.label10);
            this.groupBox_system_setup.Controls.Add(this.group_box_data_proccessing);
            this.groupBox_system_setup.Controls.Add(this.textBox1);
            this.groupBox_system_setup.Controls.Add(this.numeric_device_count);
            this.groupBox_system_setup.Controls.Add(this.label9);
            this.groupBox_system_setup.Controls.Add(this.checkBox2);
            this.groupBox_system_setup.Controls.Add(this.label1);
            this.groupBox_system_setup.Controls.Add(this.numeric_max_proccessing_time);
            this.groupBox_system_setup.Controls.Add(this.checkBox_optimization);
            this.groupBox_system_setup.Controls.Add(this.numeric_batch_count);
            this.groupBox_system_setup.Controls.Add(this.OldSecondLevelAll);
            this.groupBox_system_setup.Controls.Add(this.OldSecondLevelButton);
            this.groupBox_system_setup.Controls.Add(this.numeric_max_changeover_time);
            this.groupBox_system_setup.Controls.Add(this.label2);
            this.groupBox_system_setup.Controls.Add(this.button5);
            this.groupBox_system_setup.Controls.Add(this.label3);
            this.groupBox_system_setup.Controls.Add(this.numeric_xromossomi_size);
            this.groupBox_system_setup.Controls.Add(this.label4);
            this.groupBox_system_setup.Controls.Add(this.label11);
            this.groupBox_system_setup.Controls.Add(this.numeric_buffer);
            this.groupBox_system_setup.Controls.Add(this.button2);
            this.groupBox_system_setup.Controls.Add(this.label5);
            this.groupBox_system_setup.Controls.Add(this.checkBox_fixed_batches);
            this.groupBox_system_setup.Location = new System.Drawing.Point(8, 24);
            this.groupBox_system_setup.Name = "groupBox_system_setup";
            this.groupBox_system_setup.Size = new System.Drawing.Size(506, 401);
            this.groupBox_system_setup.TabIndex = 41;
            this.groupBox_system_setup.TabStop = false;
            this.groupBox_system_setup.Text = "Настройки системы";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(364, 104);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 32;
            // 
            // numeric_generation_count
            // 
            this.numeric_generation_count.Location = new System.Drawing.Point(394, 181);
            this.numeric_generation_count.Name = "numeric_generation_count";
            this.numeric_generation_count.Size = new System.Drawing.Size(103, 20);
            this.numeric_generation_count.TabIndex = 33;
            this.numeric_generation_count.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numeric_generation_count.ValueChanged += new System.EventHandler(this.numeric_generation_count_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(361, 55);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Кол-во типов комплектов";
            // 
            // group_box_data_proccessing
            // 
            this.group_box_data_proccessing.Controls.Add(this.radioButton_TournamentSelection);
            this.group_box_data_proccessing.Controls.Add(this.radioButton_RouletteMethod);
            this.group_box_data_proccessing.Controls.Add(this.radioButton_SigmaClipping);
            this.group_box_data_proccessing.Controls.Add(this.radioButton_UniformRanking);
            this.group_box_data_proccessing.Location = new System.Drawing.Point(6, 210);
            this.group_box_data_proccessing.Name = "group_box_data_proccessing";
            this.group_box_data_proccessing.Size = new System.Drawing.Size(246, 108);
            this.group_box_data_proccessing.TabIndex = 36;
            this.group_box_data_proccessing.TabStop = false;
            this.group_box_data_proccessing.Text = "Способ обработки данных";
            this.group_box_data_proccessing.Enter += new System.EventHandler(this.group_box_data_proccessing_Enter);
            // 
            // radioButton_TournamentSelection
            // 
            this.radioButton_TournamentSelection.AutoSize = true;
            this.radioButton_TournamentSelection.Location = new System.Drawing.Point(6, 19);
            this.radioButton_TournamentSelection.Name = "radioButton_TournamentSelection";
            this.radioButton_TournamentSelection.Size = new System.Drawing.Size(133, 17);
            this.radioButton_TournamentSelection.TabIndex = 15;
            this.radioButton_TournamentSelection.TabStop = true;
            this.radioButton_TournamentSelection.Text = "Турнирная селекция ";
            this.radioButton_TournamentSelection.UseVisualStyleBackColor = true;
            this.radioButton_TournamentSelection.CheckedChanged += new System.EventHandler(this.radioButton_TournamentSelection_change);
            // 
            // radioButton_RouletteMethod
            // 
            this.radioButton_RouletteMethod.AutoSize = true;
            this.radioButton_RouletteMethod.Location = new System.Drawing.Point(6, 42);
            this.radioButton_RouletteMethod.Name = "radioButton_RouletteMethod";
            this.radioButton_RouletteMethod.Size = new System.Drawing.Size(100, 17);
            this.radioButton_RouletteMethod.TabIndex = 16;
            this.radioButton_RouletteMethod.TabStop = true;
            this.radioButton_RouletteMethod.Text = "Метод рулетки";
            this.radioButton_RouletteMethod.UseVisualStyleBackColor = true;
            this.radioButton_RouletteMethod.CheckedChanged += new System.EventHandler(this.radioButton_RouletteMethod_change);
            // 
            // radioButton_SigmaClipping
            // 
            this.radioButton_SigmaClipping.AutoSize = true;
            this.radioButton_SigmaClipping.Location = new System.Drawing.Point(6, 88);
            this.radioButton_SigmaClipping.Name = "radioButton_SigmaClipping";
            this.radioButton_SigmaClipping.Size = new System.Drawing.Size(234, 17);
            this.radioButton_SigmaClipping.TabIndex = 17;
            this.radioButton_SigmaClipping.TabStop = true;
            this.radioButton_SigmaClipping.Text = "Сигма отсечение(Пока не реализованно)";
            this.radioButton_SigmaClipping.UseVisualStyleBackColor = true;
            this.radioButton_SigmaClipping.CheckedChanged += new System.EventHandler(this.radioButton_SigmaClipping_change);
            // 
            // radioButton_UniformRanking
            // 
            this.radioButton_UniformRanking.AutoSize = true;
            this.radioButton_UniformRanking.Location = new System.Drawing.Point(6, 65);
            this.radioButton_UniformRanking.Name = "radioButton_UniformRanking";
            this.radioButton_UniformRanking.Size = new System.Drawing.Size(171, 17);
            this.radioButton_UniformRanking.TabIndex = 18;
            this.radioButton_UniformRanking.TabStop = true;
            this.radioButton_UniformRanking.Text = "Равномерное ранжирование";
            this.radioButton_UniformRanking.UseVisualStyleBackColor = true;
            this.radioButton_UniformRanking.CheckedChanged += new System.EventHandler(this.radioButton_UniformRanking_change);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(364, 77);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 29;
            // 
            // numeric_device_count
            // 
            this.numeric_device_count.Location = new System.Drawing.Point(197, 160);
            this.numeric_device_count.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numeric_device_count.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numeric_device_count.Name = "numeric_device_count";
            this.numeric_device_count.Size = new System.Drawing.Size(45, 20);
            this.numeric_device_count.TabIndex = 40;
            this.numeric_device_count.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numeric_device_count.ValueChanged += new System.EventHandler(this.numeric_device_count_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(361, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Диррективные сроки";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(364, 35);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(122, 17);
            this.checkBox2.TabIndex = 27;
            this.checkBox2.Text = "Да/Первая задача";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged_1);
            // 
            // numeric_max_proccessing_time
            // 
            this.numeric_max_proccessing_time.Location = new System.Drawing.Point(197, 108);
            this.numeric_max_proccessing_time.Name = "numeric_max_proccessing_time";
            this.numeric_max_proccessing_time.Size = new System.Drawing.Size(45, 20);
            this.numeric_max_proccessing_time.TabIndex = 39;
            this.numeric_max_proccessing_time.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numeric_max_proccessing_time.ValueChanged += new System.EventHandler(this.numeric_max_proccessing_time_ValueChanged);
            // 
            // checkBox_optimization
            // 
            this.checkBox_optimization.AutoSize = true;
            this.checkBox_optimization.Location = new System.Drawing.Point(6, 371);
            this.checkBox_optimization.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_optimization.Name = "checkBox_optimization";
            this.checkBox_optimization.Size = new System.Drawing.Size(95, 17);
            this.checkBox_optimization.TabIndex = 31;
            this.checkBox_optimization.Text = "Оптимизация";
            this.checkBox_optimization.UseVisualStyleBackColor = true;
            this.checkBox_optimization.CheckedChanged += new System.EventHandler(this.checkBox_optimization_CheckedChanged);
            // 
            // numeric_batch_count
            // 
            this.numeric_batch_count.Location = new System.Drawing.Point(197, 32);
            this.numeric_batch_count.Name = "numeric_batch_count";
            this.numeric_batch_count.Size = new System.Drawing.Size(45, 20);
            this.numeric_batch_count.TabIndex = 37;
            this.numeric_batch_count.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numeric_batch_count.ValueChanged += new System.EventHandler(this.numeric_batch_count_ValueChanged);
            // 
            // OldSecondLevelAll
            // 
            this.OldSecondLevelAll.Location = new System.Drawing.Point(392, 131);
            this.OldSecondLevelAll.Margin = new System.Windows.Forms.Padding(2);
            this.OldSecondLevelAll.Name = "OldSecondLevelAll";
            this.OldSecondLevelAll.Size = new System.Drawing.Size(105, 22);
            this.OldSecondLevelAll.TabIndex = 26;
            this.OldSecondLevelAll.Text = "Га + Группы";
            this.OldSecondLevelAll.UseVisualStyleBackColor = true;
            this.OldSecondLevelAll.Click += new System.EventHandler(this.OldSecondLevelAll_Click);
            // 
            // OldSecondLevelButton
            // 
            this.OldSecondLevelButton.Location = new System.Drawing.Point(391, 155);
            this.OldSecondLevelButton.Margin = new System.Windows.Forms.Padding(2);
            this.OldSecondLevelButton.Name = "OldSecondLevelButton";
            this.OldSecondLevelButton.Size = new System.Drawing.Size(106, 23);
            this.OldSecondLevelButton.TabIndex = 25;
            this.OldSecondLevelButton.Text = "Трехуровневая задача";
            this.OldSecondLevelButton.UseVisualStyleBackColor = true;
            this.OldSecondLevelButton.Click += new System.EventHandler(this.OldSecondLevelButton_Click);
            // 
            // numeric_max_changeover_time
            // 
            this.numeric_max_changeover_time.Location = new System.Drawing.Point(197, 83);
            this.numeric_max_changeover_time.Name = "numeric_max_changeover_time";
            this.numeric_max_changeover_time.Size = new System.Drawing.Size(45, 20);
            this.numeric_max_changeover_time.TabIndex = 38;
            this.numeric_max_changeover_time.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numeric_max_changeover_time.ValueChanged += new System.EventHandler(this.numeric_max_changeover_time_ValueChanged);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(267, 130);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(118, 23);
            this.button5.TabIndex = 21;
            this.button5.Text = "прогон";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // numeric_xromossomi_size
            // 
            this.numeric_xromossomi_size.Location = new System.Drawing.Point(268, 181);
            this.numeric_xromossomi_size.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.numeric_xromossomi_size.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numeric_xromossomi_size.Name = "numeric_xromossomi_size";
            this.numeric_xromossomi_size.Size = new System.Drawing.Size(120, 20);
            this.numeric_xromossomi_size.TabIndex = 20;
            this.numeric_xromossomi_size.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numeric_xromossomi_size.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(167, 13);
            this.label11.TabIndex = 35;
            this.label11.Text = "Максимальный размер буфера";
            // 
            // numeric_buffer
            // 
            this.numeric_buffer.Location = new System.Drawing.Point(197, 134);
            this.numeric_buffer.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numeric_buffer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numeric_buffer.Name = "numeric_buffer";
            this.numeric_buffer.Size = new System.Drawing.Size(45, 20);
            this.numeric_buffer.TabIndex = 34;
            this.numeric_buffer.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numeric_buffer.ValueChanged += new System.EventHandler(this.numeric_buffer_ValueChanged);
            // 
            // checkBox_fixed_batches
            // 
            this.checkBox_fixed_batches.AutoSize = true;
            this.checkBox_fixed_batches.Location = new System.Drawing.Point(6, 349);
            this.checkBox_fixed_batches.Name = "checkBox_fixed_batches";
            this.checkBox_fixed_batches.Size = new System.Drawing.Size(149, 17);
            this.checkBox_fixed_batches.TabIndex = 12;
            this.checkBox_fixed_batches.Text = "Фиксированные партии";
            this.checkBox_fixed_batches.UseVisualStyleBackColor = true;
            this.checkBox_fixed_batches.CheckedChanged += new System.EventHandler(this.checkBox_fixed_batches_CheckedChanged);
            // 
            // setsBtn
            // 
            this.setsBtn.Location = new System.Drawing.Point(638, 348);
            this.setsBtn.Name = "setsBtn";
            this.setsBtn.Size = new System.Drawing.Size(90, 38);
            this.setsBtn.TabIndex = 22;
            this.setsBtn.Text = "Тест комплектов";
            this.setsBtn.UseVisualStyleBackColor = true;
            this.setsBtn.Click += new System.EventHandler(this.setsBtn_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(638, 304);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(90, 38);
            this.button4.TabIndex = 19;
            this.button4.Text = "Тестовый прогон";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(638, 275);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 23);
            this.button3.TabIndex = 14;
            this.button3.Text = "Второй метод";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(763, 474);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Установка времени";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 4;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.randomizeProcessingTime, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.numeric_random, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.dataGridView_proccessing_time, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.copyPreprocessingTime, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.dataGridView_changeover_time, 2, 1);
            this.tableLayoutPanel.Controls.Add(this.randomizePreprocessingTime, 2, 2);
            this.tableLayoutPanel.Controls.Add(this.label6, 2, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.277405F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.7226F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(757, 468);
            this.tableLayoutPanel.TabIndex = 23;
            // 
            // randomizeProcessingTime
            // 
            this.randomizeProcessingTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.randomizeProcessingTime.Location = new System.Drawing.Point(192, 416);
            this.randomizeProcessingTime.Name = "randomizeProcessingTime";
            this.randomizeProcessingTime.Size = new System.Drawing.Size(183, 20);
            this.randomizeProcessingTime.TabIndex = 13;
            this.randomizeProcessingTime.Text = "Рандом времени обработки";
            this.randomizeProcessingTime.UseVisualStyleBackColor = true;
            this.randomizeProcessingTime.Click += new System.EventHandler(this.randomizeProcessingTime_Click);
            // 
            // numeric_random
            // 
            this.numeric_random.Dock = System.Windows.Forms.DockStyle.Top;
            this.numeric_random.Location = new System.Drawing.Point(3, 416);
            this.numeric_random.Name = "numeric_random";
            this.numeric_random.Size = new System.Drawing.Size(183, 20);
            this.numeric_random.TabIndex = 22;
            this.numeric_random.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.numeric_random.ValueChanged += new System.EventHandler(this.numeric_random_ValueChanged);
            // 
            // dataGridView_proccessing_time
            // 
            this.dataGridView_proccessing_time.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_proccessing_time.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel.SetColumnSpan(this.dataGridView_proccessing_time, 2);
            this.dataGridView_proccessing_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_proccessing_time.Location = new System.Drawing.Point(3, 37);
            this.dataGridView_proccessing_time.Name = "dataGridView_proccessing_time";
            this.dataGridView_proccessing_time.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView_proccessing_time.Size = new System.Drawing.Size(372, 373);
            this.dataGridView_proccessing_time.TabIndex = 17;
            // 
            // copyPreprocessingTime
            // 
            this.copyPreprocessingTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.copyPreprocessingTime.Location = new System.Drawing.Point(570, 416);
            this.copyPreprocessingTime.Name = "copyPreprocessingTime";
            this.copyPreprocessingTime.Size = new System.Drawing.Size(184, 20);
            this.copyPreprocessingTime.TabIndex = 21;
            this.copyPreprocessingTime.Text = "Копировать с первого";
            this.copyPreprocessingTime.UseVisualStyleBackColor = true;
            this.copyPreprocessingTime.Click += new System.EventHandler(this.copyPreprocessingTime_Click);
            // 
            // label7
            // 
            this.tableLayoutPanel.SetColumnSpan(this.label7, 2);
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(372, 34);
            this.label7.TabIndex = 18;
            this.label7.Text = "Время обработки требований";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGridView_changeover_time
            // 
            this.dataGridView_changeover_time.AllowDrop = true;
            this.dataGridView_changeover_time.AllowUserToAddRows = false;
            this.dataGridView_changeover_time.AllowUserToDeleteRows = false;
            this.dataGridView_changeover_time.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_changeover_time.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tableLayoutPanel.SetColumnSpan(this.dataGridView_changeover_time, 2);
            this.dataGridView_changeover_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_changeover_time.Location = new System.Drawing.Point(381, 37);
            this.dataGridView_changeover_time.Name = "dataGridView_changeover_time";
            this.dataGridView_changeover_time.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView_changeover_time.Size = new System.Drawing.Size(373, 373);
            this.dataGridView_changeover_time.TabIndex = 19;
            // 
            // randomizePreprocessingTime
            // 
            this.randomizePreprocessingTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.randomizePreprocessingTime.Location = new System.Drawing.Point(381, 416);
            this.randomizePreprocessingTime.Name = "randomizePreprocessingTime";
            this.randomizePreprocessingTime.Size = new System.Drawing.Size(183, 20);
            this.randomizePreprocessingTime.TabIndex = 21;
            this.randomizePreprocessingTime.Text = "Рандом времени";
            this.randomizePreprocessingTime.UseVisualStyleBackColor = true;
            this.randomizePreprocessingTime.Click += new System.EventHandler(this.randomizeChangeoverTime_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.label6, 2);
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(381, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(373, 34);
            this.label6.TabIndex = 20;
            this.label6.Text = "Время переналадки приборов";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 500);
            this.Controls.Add(this.tabControl);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numeric_data_types_count)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox_system_setup.ResumeLayout(false);
            this.groupBox_system_setup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_generation_count)).EndInit();
            this.group_box_data_proccessing.ResumeLayout(false);
            this.group_box_data_proccessing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_device_count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_max_proccessing_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_batch_count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_max_changeover_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_xromossomi_size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_buffer)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_random)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_proccessing_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_changeover_time)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numeric_data_types_count;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView_proccessing_time;
        private System.Windows.Forms.CheckBox checkBox_fixed_batches;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RadioButton radioButton_UniformRanking;
        private System.Windows.Forms.RadioButton radioButton_SigmaClipping;
        private System.Windows.Forms.RadioButton radioButton_RouletteMethod;
        private System.Windows.Forms.RadioButton radioButton_TournamentSelection;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown numeric_xromossomi_size;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button setsBtn;
        private System.Windows.Forms.Button OldSecondLevelButton;
        private System.Windows.Forms.Button OldSecondLevelAll;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox_optimization;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown numeric_generation_count;
        public System.Windows.Forms.DataGridView dataGridView_changeover_time;
        private System.Windows.Forms.Button copyPreprocessingTime;
        private System.Windows.Forms.Button randomizePreprocessingTime;
        private System.Windows.Forms.Button randomizeProcessingTime;
        private System.Windows.Forms.NumericUpDown numeric_random;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numeric_buffer;
        private System.Windows.Forms.GroupBox group_box_data_proccessing;
        private System.Windows.Forms.NumericUpDown numeric_max_proccessing_time;
        private System.Windows.Forms.NumericUpDown numeric_max_changeover_time;
        private System.Windows.Forms.NumericUpDown numeric_batch_count;
        private System.Windows.Forms.NumericUpDown numeric_device_count;
        private System.Windows.Forms.GroupBox groupBox_system_setup;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}

