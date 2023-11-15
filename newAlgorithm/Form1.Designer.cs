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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.countBatchesTB = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timeSwitchingTB = new System.Windows.Forms.TextBox();
            this.timeTreatmentingTB = new System.Windows.Forms.TextBox();
            this.LTB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.buffer = new System.Windows.Forms.NumericUpDown();
            this.GenerationCounter = new System.Windows.Forms.NumericUpDown();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.OptimizationSecondLevel = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.OldSecondLevelAll = new System.Windows.Forms.Button();
            this.OldSecondLevelButton = new System.Windows.Forms.Button();
            this.setsBtn = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.button4 = new System.Windows.Forms.Button();
            this.radioButton_UniformRanking = new System.Windows.Forms.RadioButton();
            this.radioButton_SigmaClipping = new System.Windows.Forms.RadioButton();
            this.radioButton_RouletteMethod = new System.Windows.Forms.RadioButton();
            this.radioButton_TournamentSelection = new System.Windows.Forms.RadioButton();
            this.button3 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.valueRandomize = new System.Windows.Forms.NumericUpDown();
            this.copyPreprocessingTime = new System.Windows.Forms.Button();
            this.randomizePreprocessingTime = new System.Windows.Forms.Button();
            this.randomizeProcessingTime = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buffer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GenerationCounter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueRandomize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(15, 33);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 0;
            this.numericUpDown1.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // countBatchesTB
            // 
            this.countBatchesTB.Location = new System.Drawing.Point(15, 78);
            this.countBatchesTB.Name = "countBatchesTB";
            this.countBatchesTB.Size = new System.Drawing.Size(100, 20);
            this.countBatchesTB.TabIndex = 1;
            this.countBatchesTB.Text = "12";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 238);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Получить решение";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Количество типов данных";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(178, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Количество данных каждого типа";
            // 
            // timeSwitchingTB
            // 
            this.timeSwitchingTB.Location = new System.Drawing.Point(15, 119);
            this.timeSwitchingTB.Name = "timeSwitchingTB";
            this.timeSwitchingTB.Size = new System.Drawing.Size(100, 20);
            this.timeSwitchingTB.TabIndex = 5;
            this.timeSwitchingTB.Text = "2";
            this.timeSwitchingTB.TextChanged += new System.EventHandler(this.timeSwitchingTB_TextChanged);
            // 
            // timeTreatmentingTB
            // 
            this.timeTreatmentingTB.Location = new System.Drawing.Point(15, 162);
            this.timeTreatmentingTB.Name = "timeTreatmentingTB";
            this.timeTreatmentingTB.Size = new System.Drawing.Size(100, 20);
            this.timeTreatmentingTB.TabIndex = 6;
            this.timeTreatmentingTB.Text = "2";
            this.timeTreatmentingTB.TextChanged += new System.EventHandler(this.timeTreatmentingTB_TextChanged);
            // 
            // LTB
            // 
            this.LTB.Location = new System.Drawing.Point(159, 111);
            this.LTB.Name = "LTB";
            this.LTB.Size = new System.Drawing.Size(119, 20);
            this.LTB.TabIndex = 7;
            this.LTB.Text = "4";
            this.LTB.TextChanged += new System.EventHandler(this.LTB_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Макс время переналадки";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 146);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Макс время обработки";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(166, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Длина конвейера";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(161, 162);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Формирование Гаа";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(664, 396);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.buffer);
            this.tabPage1.Controls.Add(this.GenerationCounter);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.OptimizationSecondLevel);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.checkBox2);
            this.tabPage1.Controls.Add(this.OldSecondLevelAll);
            this.tabPage1.Controls.Add(this.OldSecondLevelButton);
            this.tabPage1.Controls.Add(this.setsBtn);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.numericUpDown2);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.radioButton_UniformRanking);
            this.tabPage1.Controls.Add(this.radioButton_SigmaClipping);
            this.tabPage1.Controls.Add(this.radioButton_RouletteMethod);
            this.tabPage1.Controls.Add(this.radioButton_TournamentSelection);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.checkBox1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.numericUpDown1);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.countBatchesTB);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.LTB);
            this.tabPage1.Controls.Add(this.timeSwitchingTB);
            this.tabPage1.Controls.Add(this.timeTreatmentingTB);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(656, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Установка параметров";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 196);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 13);
            this.label11.TabIndex = 35;
            this.label11.Text = "Буфер";
            // 
            // buffer
            // 
            this.buffer.Location = new System.Drawing.Point(15, 212);
            this.buffer.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.buffer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.buffer.Name = "buffer";
            this.buffer.Size = new System.Drawing.Size(120, 20);
            this.buffer.TabIndex = 34;
            this.buffer.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // GenerationCounter
            // 
            this.GenerationCounter.Location = new System.Drawing.Point(287, 188);
            this.GenerationCounter.Name = "GenerationCounter";
            this.GenerationCounter.Size = new System.Drawing.Size(103, 20);
            this.GenerationCounter.TabIndex = 33;
            this.GenerationCounter.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(317, 105);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 32;
            // 
            // OptimizationSecondLevel
            // 
            this.OptimizationSecondLevel.AutoSize = true;
            this.OptimizationSecondLevel.Location = new System.Drawing.Point(487, 194);
            this.OptimizationSecondLevel.Margin = new System.Windows.Forms.Padding(2);
            this.OptimizationSecondLevel.Name = "OptimizationSecondLevel";
            this.OptimizationSecondLevel.Size = new System.Drawing.Size(95, 17);
            this.OptimizationSecondLevel.TabIndex = 31;
            this.OptimizationSecondLevel.Text = "Оптимизация";
            this.OptimizationSecondLevel.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(314, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Кол-во типов комплектов";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(317, 78);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 29;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(314, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Диррективные сроки";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(317, 36);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(122, 17);
            this.checkBox2.TabIndex = 27;
            this.checkBox2.Text = "Да/Первая задача";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged_1);
            // 
            // OldSecondLevelAll
            // 
            this.OldSecondLevelAll.Location = new System.Drawing.Point(285, 138);
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
            this.OldSecondLevelButton.Location = new System.Drawing.Point(284, 162);
            this.OldSecondLevelButton.Margin = new System.Windows.Forms.Padding(2);
            this.OldSecondLevelButton.Name = "OldSecondLevelButton";
            this.OldSecondLevelButton.Size = new System.Drawing.Size(106, 23);
            this.OldSecondLevelButton.TabIndex = 25;
            this.OldSecondLevelButton.Text = "Трехуровневая задача";
            this.OldSecondLevelButton.UseVisualStyleBackColor = true;
            this.OldSecondLevelButton.Click += new System.EventHandler(this.OldSecondLevelButton_Click);
            // 
            // setsBtn
            // 
            this.setsBtn.Location = new System.Drawing.Point(441, 250);
            this.setsBtn.Name = "setsBtn";
            this.setsBtn.Size = new System.Drawing.Size(81, 49);
            this.setsBtn.TabIndex = 22;
            this.setsBtn.Text = "Тест комплектов";
            this.setsBtn.UseVisualStyleBackColor = true;
            this.setsBtn.Click += new System.EventHandler(this.setsBtn_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(160, 137);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(118, 23);
            this.button5.TabIndex = 21;
            this.button5.Text = "прогон";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(161, 188);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown2.TabIndex = 20;
            this.numericUpDown2.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(505, 14);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(98, 50);
            this.button4.TabIndex = 19;
            this.button4.Text = "Тестовый прогон";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // radioButton_UniformRanking
            // 
            this.radioButton_UniformRanking.AutoSize = true;
            this.radioButton_UniformRanking.Location = new System.Drawing.Point(163, 273);
            this.radioButton_UniformRanking.Name = "radioButton_UniformRanking";
            this.radioButton_UniformRanking.Size = new System.Drawing.Size(171, 17);
            this.radioButton_UniformRanking.TabIndex = 18;
            this.radioButton_UniformRanking.TabStop = true;
            this.radioButton_UniformRanking.Text = "Равномерное ранжирование";
            this.radioButton_UniformRanking.UseVisualStyleBackColor = true;
            this.radioButton_UniformRanking.CheckedChanged += new System.EventHandler(this.radioButton_UniformRanking_change);
            // 
            // radioButton_SigmaClipping
            // 
            this.radioButton_SigmaClipping.AutoSize = true;
            this.radioButton_SigmaClipping.Location = new System.Drawing.Point(163, 296);
            this.radioButton_SigmaClipping.Name = "radioButton_SigmaClipping";
            this.radioButton_SigmaClipping.Size = new System.Drawing.Size(234, 17);
            this.radioButton_SigmaClipping.TabIndex = 17;
            this.radioButton_SigmaClipping.TabStop = true;
            this.radioButton_SigmaClipping.Text = "Сигма отсечение(Пока не реализованно)";
            this.radioButton_SigmaClipping.UseVisualStyleBackColor = true;
            this.radioButton_SigmaClipping.CheckedChanged += new System.EventHandler(this.radioButton_SigmaClipping_change);
            // 
            // radioButton_RouletteMethod
            // 
            this.radioButton_RouletteMethod.AutoSize = true;
            this.radioButton_RouletteMethod.Location = new System.Drawing.Point(163, 250);
            this.radioButton_RouletteMethod.Name = "radioButton_RouletteMethod";
            this.radioButton_RouletteMethod.Size = new System.Drawing.Size(100, 17);
            this.radioButton_RouletteMethod.TabIndex = 16;
            this.radioButton_RouletteMethod.TabStop = true;
            this.radioButton_RouletteMethod.Text = "Метод рулетки";
            this.radioButton_RouletteMethod.UseVisualStyleBackColor = true;
            this.radioButton_RouletteMethod.CheckedChanged += new System.EventHandler(this.radioButton_RouletteMethod_change);
            // 
            // radioButton_TournamentSelection
            // 
            this.radioButton_TournamentSelection.AutoSize = true;
            this.radioButton_TournamentSelection.Location = new System.Drawing.Point(163, 227);
            this.radioButton_TournamentSelection.Name = "radioButton_TournamentSelection";
            this.radioButton_TournamentSelection.Size = new System.Drawing.Size(133, 17);
            this.radioButton_TournamentSelection.TabIndex = 15;
            this.radioButton_TournamentSelection.TabStop = true;
            this.radioButton_TournamentSelection.Text = "Турнирная селекция ";
            this.radioButton_TournamentSelection.UseVisualStyleBackColor = true;
            this.radioButton_TournamentSelection.CheckedChanged += new System.EventHandler(this.radioButton_TournamentSelection_change);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(15, 267);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(136, 23);
            this.button3.TabIndex = 14;
            this.button3.Text = "Второй метод";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(158, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(130, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Фиксированные партии";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(161, 33);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(41, 17);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.Text = "Да";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.valueRandomize);
            this.tabPage2.Controls.Add(this.copyPreprocessingTime);
            this.tabPage2.Controls.Add(this.randomizePreprocessingTime);
            this.tabPage2.Controls.Add(this.randomizeProcessingTime);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(656, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Установка времени";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // valueRandomize
            // 
            this.valueRandomize.Location = new System.Drawing.Point(4, 337);
            this.valueRandomize.Name = "valueRandomize";
            this.valueRandomize.Size = new System.Drawing.Size(62, 20);
            this.valueRandomize.TabIndex = 22;
            // 
            // copyPreprocessingTime
            // 
            this.copyPreprocessingTime.Location = new System.Drawing.Point(466, 337);
            this.copyPreprocessingTime.Name = "copyPreprocessingTime";
            this.copyPreprocessingTime.Size = new System.Drawing.Size(139, 20);
            this.copyPreprocessingTime.TabIndex = 21;
            this.copyPreprocessingTime.Text = "Копировать с первого";
            this.copyPreprocessingTime.UseVisualStyleBackColor = true;
            this.copyPreprocessingTime.Click += new System.EventHandler(this.copyPreprocessingTime_Click);
            // 
            // randomizePreprocessingTime
            // 
            this.randomizePreprocessingTime.Location = new System.Drawing.Point(321, 337);
            this.randomizePreprocessingTime.Name = "randomizePreprocessingTime";
            this.randomizePreprocessingTime.Size = new System.Drawing.Size(139, 20);
            this.randomizePreprocessingTime.TabIndex = 21;
            this.randomizePreprocessingTime.Text = "Рандом времени";
            this.randomizePreprocessingTime.UseVisualStyleBackColor = true;
            this.randomizePreprocessingTime.Click += new System.EventHandler(this.randomizePreprocessingTime_Click);
            // 
            // randomizeProcessingTime
            // 
            this.randomizeProcessingTime.Location = new System.Drawing.Point(72, 337);
            this.randomizeProcessingTime.Name = "randomizeProcessingTime";
            this.randomizeProcessingTime.Size = new System.Drawing.Size(212, 20);
            this.randomizeProcessingTime.TabIndex = 13;
            this.randomizeProcessingTime.Text = "Рандом времени обработки";
            this.randomizeProcessingTime.UseVisualStyleBackColor = true;
            this.randomizeProcessingTime.Click += new System.EventHandler(this.randomizeProcessingTime_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(317, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(282, 24);
            this.label6.TabIndex = 20;
            this.label6.Text = "Время переналадки приборов";
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowDrop = true;
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(321, 43);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(283, 288);
            this.dataGridView2.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(4, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(280, 24);
            this.label7.TabIndex = 18;
            this.label7.Text = "Время обработки требований";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(4, 43);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(280, 288);
            this.dataGridView1.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 396);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.buffer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GenerationCounter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.valueRandomize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox countBatchesTB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox timeSwitchingTB;
        private System.Windows.Forms.TextBox timeTreatmentingTB;
        private System.Windows.Forms.TextBox LTB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RadioButton radioButton_UniformRanking;
        private System.Windows.Forms.RadioButton radioButton_SigmaClipping;
        private System.Windows.Forms.RadioButton radioButton_RouletteMethod;
        private System.Windows.Forms.RadioButton radioButton_TournamentSelection;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button setsBtn;
        private System.Windows.Forms.Button OldSecondLevelButton;
        private System.Windows.Forms.Button OldSecondLevelAll;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox OptimizationSecondLevel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown GenerationCounter;
        public System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Button copyPreprocessingTime;
        private System.Windows.Forms.Button randomizePreprocessingTime;
        private System.Windows.Forms.Button randomizeProcessingTime;
        private System.Windows.Forms.NumericUpDown valueRandomize;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown buffer;
    }
}

