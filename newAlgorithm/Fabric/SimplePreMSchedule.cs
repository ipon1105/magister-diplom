﻿using magisterDiplom.Model;
using magisterDiplom.Utils;
using newAlgorithm;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using Batch = magisterDiplom.Model.Batch;

namespace magisterDiplom.Fabric
{

    /// <summary>
    /// Класс простого расписания с ПТО, наследуется от PreMSchedule
    /// </summary>
    public class SimplePreMSchedule : PreMSchedule
    {
        const bool IsDebug_ShiftMatrixY = true;
        const bool IsDebug_SearchByPosition = true;
        const bool IsDebug_CalcSysReliability = true;
        const bool IsDebug_GetPreMUtility = true;

        /// <summary>
        /// Поток записи в файл
        /// </summary>
        private FileStream fstream = null;

        /// <summary>
        /// Установит файл записи
        /// </summary>
        /// <param name="filename">Имя файла для записи</param>
        public void SetLogFile(string filename)
        {

            // Создаём объект для записи в файл
            fstream = new FileStream(filename, FileMode.Create);
        }

        /// <summary>
        /// Закроет объект для записи в файла
        /// </summary>
        private void UnsetLogFile()
        {

            // Закрываем объект
            fstream?.Close();
            fstream = null;
        }

        /// <summary>
        /// Запишит переданный текст в конец файла
        /// </summary>
        /// <param name="text">Текст для записи в конец файла</param>
        private void WriteToLogFile(string text)
        {

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Преобразуем текстовые данные в множество байт
            byte[] buffer = Encoding.Default.GetBytes($"{text}");

            // Записываем данные в файл
            fstream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Выводим матриц порядка ПТО
        /// </summary>
        private void WriteToLogFileMatrixY(String prefix = "")
        {

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Создаём строку с 10000 символов вместимостью
            StringBuilder str = new StringBuilder(10000);

            // Выводим информационное сообщение
            str.AppendLine($"{prefix}Матрица порядка ПТО [Y].");

            // Объявляем количество пакетов
            int batchCount;

            // Если расписание пустое
            if ((batchCount = this.schedule.Count()) == 0)
            {

                // Выводим информационное сообщение
                str.AppendLine($"{prefix}Не существует.");

                // Прекращяем вывод
                return;
            }

            // Объявляем индекс пакета
            int batchIndex;

            // Объявляем индекс прибора
            int device;

            // Для каждого прибора
            for (device = 0; device < this.config.deviceCount; device++) {

                // Выводим префикс
                str.Append($"{prefix}");

                // Выводим разделитель
                for (batchIndex = 0; batchIndex < batchCount; batchIndex++)
                    str.Append($"+-");
                str.AppendLine("+");

                // Выводим префикс
                str.Append($"{prefix}");

                // Выводим данные
                for (batchIndex = 0; batchIndex < this.matrixY[device].Count(); batchIndex++)
                    str.Append($"|{matrixY[device][batchIndex]}");
                str.AppendLine("|");
            }

            // Выводим префикс
            str.Append($"{prefix}");

            // Выводим разделитель
            for (batchIndex = 0; batchIndex < batchCount; batchIndex++)
                str.Append($"+-");
            str.AppendLine("+");

            // Преобразуем текстовые данные в множество байт
            byte[] buffer = Encoding.Default.GetBytes(str.ToString());

            // Записываем данные в файл
            fstream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Функция выполняет вывод матриц порядка и количества пакетов заданий
        /// </summary>
        /// <param name="prefix">Префикс перед выводом матриц</param>
        private void WriteToLogFileSchedule(String prefix = "") {

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Создаём строку с 10000 символов вместимостью
            StringBuilder str = new StringBuilder(10000);

            // Объявляем количество пакетов
            int batchCount;

            // Объявляем тип данных
            int dataType;

            // Объявляем индекс пакета заданий
            int batchIndex;

            // Объявляем количество цифр для максимального размера пакета заданий
            int symbols = 0;

            // Выводим информационное сообщение
            str.AppendLine($"{prefix}Матрица порядка и количества пакетов заданий [P,R].");
            
            // Если расписание пустое
            if ((batchCount = this.schedule.Count()) == 0)
            {

                // Выводим информационное сообщение
                str.AppendLine($"{prefix}Не существует.");

                // Прекращяем вывод
                return;
            }

            // Вычисляем размер максимального пакет
            for (batchIndex = 0; batchIndex < batchCount; batchIndex++)
                symbols = Math.Max(symbols, this.schedule[batchIndex].Size);

            // Вычисляем количество цифр для максимального размера пакета заданий
            symbols = (int)Math.Log10(symbols) + 1;

            // Получаем матрицы P и R
            List<List<int>> mP = GetMatrixP();
            List<List<int>> mR = GetMatrixR();

            // Для каждой строки
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++) {

                // Выводим префикс
                str.Append($"{prefix}");

                // Выводим разделитель
                for (batchIndex = 0; batchIndex < batchCount; batchIndex++)
                    str.Append($"+-");
                str.Append("+   ");

                // Выводим разделитель
                for (batchIndex = 0; batchIndex < batchCount; batchIndex++) {
                    str.Append($"+");
                    str.Append(new String('-', symbols));
                }
                str.AppendLine("+");

                // Выводим префикс
                str.Append($"{prefix}");

                // Выводим элементы матрицы P
                for (batchIndex = 0; batchIndex < batchCount; batchIndex++) {
                    str.Append($"|{mP[dataType][batchIndex]}");
                }
                str.Append("|   ");

                // Выводим элементы матрицы R
                for (batchIndex = 0; batchIndex < batchCount; batchIndex++)
                {

                    str.Append($"|{mR[dataType][batchIndex]}");
                    if (mR[dataType][batchIndex] == 0)
                        str.Append(new String(' ', symbols - 1));
                    else
                        str.Append(new String(' ', (symbols - (int) Math.Log10(mR[dataType][batchIndex]) - 1 ) ));
                }
                str.AppendLine("|");
            }

            // Выводим префикс
            str.Append($"{prefix}");

            // Выводим разделитель
            for (batchIndex = 0; batchIndex < batchCount; batchIndex++)
                str.Append($"+-");
            str.Append("+   ");

            // Выводим разделитель
            for (batchIndex = 0; batchIndex < batchCount; batchIndex++) {
                str.Append($"+");
                str.Append(new String('-', symbols));
            }
            str.AppendLine("+");

            // Преобразуем текстовые данные в множество байт
            byte[] buffer = Encoding.Default.GetBytes(str.ToString());

            // Записываем данные в файл
            fstream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Функция выполняет вывод матрицы моментов начала времени выполнения заданий и ПТО
        /// </summary>
        /// <param name="prefix">Префикс перед выводом матрицы</param>
        private void WriteToLogFileStartProcessing(String prefix = "")
        {

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Создаём строку с 10000 символов вместимостью
            StringBuilder str = new StringBuilder(10000);

            // Выводим информационное сообщение
            str.AppendLine($"{prefix}Матрица моментов начала времени выполнения заданий и ПТО.");

            // Если матрица начала моментов времени выполнения заданий пуста
            if (this.startProcessing.Count == 0)
            {

                // Выводим информационное сообщение
                str.AppendLine($"{prefix}Не существует.");

                // Заканчиваем вывод
                return;
            }

            // Объявляем максимальное время
            int times;

            // Объявляем количество цифр для числа под типы
            int typeSize;

            // Объявляем количество цифр для числа время выполнения
            int procSize;

            // Объявляем общее количество символов для вывода отрезка равной единице времени
            int genSize;
            
            // Объявляем индекс прибора
            int device;

            // Объявляем индекс типа данных
            int dataType;

            // Объявляем размер наибольшего ПТО
            int premTime = 0;

            // Расчитываем количество цифр для числа под типы
            typeSize = (int)Math.Log10(this.config.dataTypesCount) + 1;

            // Расчитываем количество цифр для числа время выполнения
            procSize = 0;
            for (device = 0; device < this.config.deviceCount; device++)
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                    procSize = Math.Max(procSize, this.config.proccessingTime[device][dataType]);
            procSize = (int)Math.Log10(procSize) + 1;

            // Расчитываем общее количество символов для вывода отрезка равной единице времени
            genSize = typeSize + 1 + procSize;

            // Вычисляем максимальное время
            {

                // Вычисляем время для выполнения всех заданий
                times = this.GetMakespan();

                // Вычисляем максимальный разделитель
                genSize = Math.Max(genSize, (int)Math.Log10(times) + 1);

                // Вычисляем размер наибольшего ПТО
                for (device = 0; device < this.config.deviceCount; device++)
                    premTime = Math.Max(premTime, this.preMConfig.preMaintenanceTimes[device]);
                times += premTime;
            }

            // Выводим разделитель
            {
                str.Append($"{prefix}{0}");
                str.Append(new String(' ', genSize));
                for (int j = 1; j < times + 1; j++)
                {
                    str.Append($"{j}");
                    str.Append(new String(' ', (genSize - ((int)Math.Log10(j)))));
                }
                str.AppendLine();
            }

            // Для каждого прибора
            for (device = 0; device < this.config.deviceCount; device++)
            {

                // Выводим разделитель
                {
                    str.Append($"{prefix}+");
                    for (int i = 0; i < times; i++)
                    {
                        str.Append(new String('-', genSize));
                        str.Append("+");
                    }
                    str.AppendLine();
                }

                str.Append($"{prefix}");

                // Для каждого временного отделителя
                for (int time = 0; time < times; time++)
                {

                    // Для каждого пакета заданий
                    for (int batchIndex = 0; batchIndex < this.startProcessing[device].Count(); batchIndex++) { 

                        // Для каждого задания
                        for (int job = 0; job < this.startProcessing[device][batchIndex].Count(); job++)

                            // Если момент начала времени выполнения равен текущему
                            if (this.startProcessing[device][batchIndex][job] == time)
                            {

                                // Вычисляем время выполнения пакета задания с типом this.schedule[batchIndex].Type на приборе device
                                int proc_time = this.config.proccessingTime[device][this.schedule[batchIndex].Type];

                                // Выводим t заданий
                                for (int t = 0; t < proc_time; t++) {

                                    // Объявляем число непробельных символов для отображения типа
                                    int unspaces;

                                    // Вычисляем количество символов для отображения типа
                                    if (this.schedule[batchIndex].Type == 0)
                                        unspaces = 1;
                                    else
                                        unspaces = (int)Math.Log10(this.schedule[batchIndex].Type) + 1;

                                    // Вычисляем количество символов для отображения времени выполнения
                                    unspaces += (int)Math.Log10(proc_time) + 1;

                                    // Выводим пакет в формате тип:время выполнения
                                    str.Append($"|{this.schedule[batchIndex].Type}:{proc_time}");
                                    str.Append(new String(' ', genSize - (unspaces + 1)));
                                }

                                // Если текущий момент времени последний и в данной позиции есть ПТО
                                if (this.startProcessing[device][batchIndex][job] == this.startProcessing[device][batchIndex].Last() && this.matrixY[device][batchIndex] == 1)
                                {

                                    // Выводим t раз ПТО
                                    for (int t = 0; t < this.preMConfig.preMaintenanceTimes[device]; t++) { 
                                        str.Append($"|");
                                        str.Append(new String('*', genSize));
                                    }

                                    // Увеличиваем время
                                    time += this.preMConfig.preMaintenanceTimes[device];
                                }

                                // Увеличиваем время
                                time += this.config.proccessingTime[device][this.schedule[batchIndex].Type];
                            }
                    }
                    
                    // Если время возможно
                    if (time < times) {

                        // Отрисовывем пробел
                        str.Append($"|");
                        str.Append(new String(' ', genSize));
                    }
                }

                // Переводим курсор
                str.AppendLine("|");
            }

            // Выводим разделитель
            {
                str.Append($"{prefix}+");
                for (int i = 0; i < times; i++)
                {
                    str.Append(new String('-', genSize));
                    str.Append("+");
                }
                str.AppendLine();
            }

            // Преобразуем текстовые данные в множество байт
            byte[] buffer = Encoding.Default.GetBytes(str.ToString());
             
            // Записываем данные в файл
            fstream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Вернёт индекс ПЗ за которым следует последнее ПТО
        /// </summary>
        /// <param name="device">Индекс прибора</param>
        /// <returns>Индекс ПЗ за которым следует последнее ПТО или -1</returns>
        private int GetLastPreMPos(int device)
        {

            // Выполняем обход по всем ПЗ
            for (int batchIndex = this.matrixY[device].Count() - 1; batchIndex >= 0; batchIndex--)

                // Если в текущей позиции существует ПТО
                if (this.matrixY[device][batchIndex] != 0)

                    // Возвращаем индекс данной позиции
                    return batchIndex;

            // Вернём -1
            return -1;
        }

        /// <summary>
        /// Выполняем сдвиг для матрицы ПТО приборов
        /// </summary>
        private void ShiftMatrixY() 
        {

            // Если логирование установлена
            if (Form1.loggingOn)

                // Выводим информацию
                WriteToLogFile($"ShiftMatrixY start: Улучшаем позиции ПТО.{Environment.NewLine}");

            // Объявляем индекс прибора
            int bestDevice = -1;

            // Объявляем критерий текущего лучшего расписания
            int f2;

            // Объявляем критерий f2 для текущего расписания со сдвигом
            int new_f2;

            // Объявляем индекс ПЗ за которым следует последнее ПТО
            int last_prem_batch_index;

            // Объявляем индекс последнего ПЗ для текущего расписания
            int last_batch_index;

            // Выполняем обработку в цикле
            do {

                // Обнуляем критерий f2 для текущего расписания
                f2 = 0;

                // Если логирование установлено
                if (Form1.loggingOn) {
                    WriteToLogFile($"Новая итерация сдвигов{Environment.NewLine}");

                    CalcStartProcessing();
                    CalcMatrixTPM();

                    WriteToLogFile($"1{Environment.NewLine}");
                    WriteToLogFileMatrixY();
                    WriteToLogFileStartProcessing();
                    WriteToLogFile($"f2 для текущего расписания {this.GetPreMUtility()}{Environment.NewLine}");
                }

                // Для каждого прибора выполняем обработку
                for (int device = 0; device < config.deviceCount; device++)
                {

                    // Вычисляем матрицу моментов начала времени выполнения заданий
                    CalcStartProcessing();

                    // Вычисляем матрицу моментов окончания времени выполнения ПТО
                    CalcMatrixTPM();

                    // Определяем индекс ПЗ за которым следует последнее ПТО
                    last_prem_batch_index = this.GetLastPreMPos(device); // j*

                    // Определяем индекс последнего ПЗ для текущего расписания
                    last_batch_index = this.matrixY[device].Count() - 1; // j^max

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"Выполняем сдвиг для прибора {device} j*={last_prem_batch_index}; j^max={last_batch_index}{Environment.NewLine}");
                    

                    // Проверяем на необходимость проведения операций перестановки
                    if (last_prem_batch_index == last_batch_index) {
                        if (IsDebug && IsDebug_ShiftMatrixY)
                            WriteToLogFile($"Пропускаем сдвиг для прибора: {device}{Environment.NewLine}");

                        // Пропускаем итерацию для текущего прибора
                        continue;
                    }

                    // Выполняем сдвиг ПТО на следующую позицию
                    this.matrixY[device][last_prem_batch_index] = 0;
                    this.matrixY[device][last_prem_batch_index + 1] = 1;

                    // Вычисляем матрицу моментов начала времени выполнения заданий
                    this.CalcStartProcessing();

                    // Вычисляем матрицу моментов окончания времени выполнения ПТО
                    this.CalcMatrixTPM();

                    // Если логирование установлено
                    if (Form1.loggingOn)
                    {
                        WriteToLogFile($"2{Environment.NewLine}");
                        WriteToLogFileMatrixY();
                        WriteToLogFileStartProcessing();
                    }

                    // Если текущее решение не удовлетворяет условию надёжности
                    if (!this.IsSolutionAcceptable())
                    {
                        // Если логирование установлено
                        if (Form1.loggingOn)
                            WriteToLogFile($"РЕШЕНИЕ НЕ ДОПУСТИМО{Environment.NewLine}");
                        
                        // Выполняем обратный сдвиг ПТО
                        this.matrixY[device][last_prem_batch_index] = 1;
                        this.matrixY[device][last_prem_batch_index + 1] = 0;

                        // Пропускаем итерацию
                        continue;
                    }

                    // Вычисляем критерий f2 для текущего расписания со сдвигом
                    new_f2 = this.GetPreMUtility();

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"РЕШЕНИЕ ДОПУСТИМО. G(f2)={f2};new_f2={new_f2}{Environment.NewLine}");

                    // Если текущее расписания лучше предыдущего
                    if (new_f2 > f2)
                    {

                        // Запоминаем новый лучший критерий f2
                        f2 = new_f2;

                        // Переопределяем индекс прибора
                        bestDevice = device;
                    }

                    // Выполняем обратный сдвиг ПТО
                    this.matrixY[device][last_prem_batch_index] = 1;
                    this.matrixY[device][last_prem_batch_index + 1] = 0;
                }

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"f2={f2}{Environment.NewLine}");

                // Если улучшений позиций ПТО не было найдено
                if (f2 == 0)
                {
                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"Улучшений не было найдено{Environment.NewLine}");

                    // Прекращаем обработку
                    break;
                }

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"Было найдено улучшение.{Environment.NewLine}");

                // Определяем индекс ПЗ за которым следует последнее ПТО
                last_prem_batch_index = this.GetLastPreMPos(bestDevice); // j*

                // Выполняем их переопределение
                this.matrixY[bestDevice][last_prem_batch_index] = 0;
                this.matrixY[bestDevice][last_prem_batch_index + 1] = 1;

                // Если логирование установлено
                if (Form1.loggingOn) { 
                    WriteToLogFile($"Новое решение:{Environment.NewLine}");
                    WriteToLogFile($"3{Environment.NewLine}");
                    WriteToLogFileMatrixY();
                    WriteToLogFileStartProcessing();
                }

                // Продолжаем улучшения
            } while (true);

            // Если логирование установлено
            if (Form1.loggingOn)
            {
                WriteToLogFile($"Было найдено решение с помощью сдвигов.{Environment.NewLine}");
                WriteToLogFile($"Выполняется заполнение позиций ПТО не найденных с помощью сдвигов.{Environment.NewLine}");
            }

            // Для каждого прибора выполняем дополнение для матрицы ПТО 1
            for (int device = 0; device < this.config.deviceCount; device++)
            {
                // Если логирование установлено
                if (Form1.loggingOn) { 
                    WriteToLogFile($"Для прибора: {device};{Environment.NewLine}");
                }

                // Определяем индекс ПЗ за которым следует последнее ПТО
                last_prem_batch_index = this.GetLastPreMPos(device); // j*

                // Определяем индекс последнего ПЗ для текущего расписания
                last_batch_index = this.matrixY[device].Count() - 1; // j^max

                // Если матрица Y не оканчивается 1
                if (last_prem_batch_index < last_batch_index) {

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"ПТО добавляется.{Environment.NewLine}");

                    // Изменяем индекс последнего ПТО нп 1
                    this.matrixY[device][last_batch_index] = 1;
                }
                
                // Если логирование установлено
                else if (Form1.loggingOn)
                    WriteToLogFile($"ПТО не добавляется.{Environment.NewLine}");
            }

            // Если логирование установлено
            if (Form1.loggingOn)
            {
                WriteToLogFileStartProcessing();
                WriteToLogFileMatrixY();
                WriteToLogFile($"ShiftMatrixY stop.{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Данная функция выполняет локальную оптимизацию составов ПЗ
        /// </summary>
        /// <param name="swapCount">Количество перестановок</param>
        /// <returns>true, если была найдено перестановка удовлетворяющая условию надёжности. Иначе false</returns>
        private bool SearchByPosition(int swapCount = 999999)
        {

            // Если логирование установлено
            if (Form1.loggingOn)
                WriteToLogFile($"SearchByPosition start: Изменяем позиции пакета заданий. beta:{preMConfig.beta}; swapCount:{swapCount}{Environment.NewLine}");

            // Флаг перестановки выполняющей условию надёжности
            bool isFind = false;

            // Объявляем лучшее расписание
            List<Model.Batch> bestSchedule = new List<Model.Batch>(this.schedule);
            
            // Объявляем значение наилучшего критерия f2
            int bestTime = 2000000000;

            // Объявляем значение текущего критерия f2
            int newTime;

            // Вычисляем матрицу моментов времени начала выполнения заданий
            this.CalcStartProcessing();

            // Вычисляем матрицу моментов окончания ПТО приборов
            this.CalcMatrixTPM();

            // Если логирование установлено
            if (Form1.loggingOn)
            {
                WriteToLogFile($"Начальное расписание:{Environment.NewLine}");
                WriteToLogFileSchedule(); WriteToLogFileStartProcessing();
            }

            // Проверяем допустимость текущего решения
            if (this.IsSolutionAcceptable()) {

                // Устанавливаем флаг перестановки выполняющей условию надёжности
                isFind = true;

                // Получаем f2 критерий - момента времени окончания последнего задания
                bestTime = this.GetPreMUtility();

                // Устанавливаем лучшее расписание
                bestSchedule = new List<Batch>(this.schedule);

                // Если логирование установлено
                if (Form1.loggingOn)
                {
                    WriteToLogFile($"Начальное расписание допустимо{Environment.NewLine}");
                    WriteToLogFile($"f2 для текущего расписания:{bestTime}{Environment.NewLine}");
                }
            }
            
            // Если логирование установлено
            else if (Form1.loggingOn)
            
                WriteToLogFile($"Начальное расписание не допустимо{Environment.NewLine}");
            

            // Выполняем заявленное количество перестановок, заявленно количество раз
            for (int batchIndex = schedule.Count - 1; batchIndex > 0 && swapCount > 0; batchIndex--, swapCount--)
            {

                // Выполняем перестановку
                (this.schedule[batchIndex - 1], this.schedule[batchIndex]) = (this.schedule[batchIndex], this.schedule[batchIndex - 1]);

                // Batch batch = this.schedule[batchIndex];
                // this.schedule[batchIndex] = this.schedule[batchIndex - 1];
                // this.schedule[batchIndex - 1] = batch;

                // Если логирование установлено
                if (Form1.loggingOn)
                {
                    WriteToLogFile($"Выполняем перестановку {batchIndex} и {batchIndex - 1}{Environment.NewLine}");
                    WriteToLogFileSchedule();
                }

                // Вычисляем матрицу моментов времени начала выполнения заданий
                this.CalcStartProcessing();

                // Вычисляем матрицу моментов окончания времени ПТО приборов
                this.CalcMatrixTPM();

                // Проверяем допустимость текущего решения
                if (this.IsSolutionAcceptable())
                {

                    // Устанавливаем флаг перестановки выполняющей условию надёжности
                    isFind = true;

                    // Высчитываем новый критерий makespan
                    newTime = this.GetPreMUtility();

                    // Если логирование установлено
                    if (Form1.loggingOn)
                    {
                        WriteToLogFile($"Текущее расписание допустимо{Environment.NewLine}");
                        WriteToLogFile($"new_f2 для текущего расписания:{newTime}{Environment.NewLine}");
                    }

                    // Если новое время лучше, то выполняем переопределение
                    if (newTime > bestTime)
                    {

                        // Если логирование установлено
                        if (Form1.loggingOn)
                        
                            WriteToLogFile($"Текущее расписание лучше предыдущего. ({newTime} > {bestTime}).{Environment.NewLine}");
                        

                        // TODO: Избавиться от копирования списка в пользу использования индекса наилучшей позиции
                        // Переопределяем лучшее расписание
                        bestSchedule = new List<Batch>(schedule);

                        // Переопределяем лучшее время для лучшего расписания
                        bestTime = newTime;
                    }
                }
                
                // Если логирование установлено
                else if (Form1.loggingOn)
                {
                    WriteToLogFile($"Текущее расписание не допустимо{Environment.NewLine}");
                }
            }

            // Выполняем переопределение наилучшего раысписания составов ПЗ
            this.schedule = bestSchedule;

            // Если логирование установлено
            if (Form1.loggingOn)
            {
                WriteToLogFile($"Извлекаем лучшее расписание{Environment.NewLine}");
                WriteToLogFileSchedule();
            }

            // Возвращаем результат
            return isFind;
        }

        /// <summary>
        /// Функция проверяет допустимость решения
        /// </summary>
        /// <returns>true - если текущее решение допустимо. Иначе False</returns>
        private bool IsSolutionAcceptable()
        {

            // Если логирование установлено
            if (Form1.loggingOn)
                WriteToLogFile($"IsSolutionAcceptable start: Проверяем допустимость решения. beta:{preMConfig.beta}{Environment.NewLine}");
                
            // Для каджого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            
                // Для каждого ПЗ в расписании выполняем обработку
                for (int batch = 0; batch < this.schedule.Count; batch++)

                    // Если для данной позиции существует ПТО
                    if (this.matrixY[device][batch] != 0)
                    {

                        // Вычисляем момент времени окончания ПТО на текущем приборе в текущей позиции
                        int time =

                            // Момент времени начала выполнения последнего задания на текущем приборе в текущей позиции
                            this.startProcessing[device][batch].Last() +

                            // Время выполнения задания на текущем приборе в текущей позиции 
                            this.config.proccessingTime[device][this.schedule[batch].Type];

                        CalcMatrixTPM();

                        // Проверяем ограничение надёжности
                        if (!IsConstraint_CalcSysReliability(time)) {

                            // Если логирование установлено
                            if (Form1.loggingOn)
                                WriteToLogFile($"Ограничение не выполняется{Environment.NewLine}");

                            // Если ограничение не выполняется, вернуть false
                            return false;
                        }

                        // Если логирование установлено
                        if (Form1.loggingOn)
                            WriteToLogFile($"Ограничение выполняется{Environment.NewLine}");
                    }
            
            // Все ограничения выпоняются, вернуть true
            return true;
        }

        /// <summary>
        /// Конструктор выполняющий создание экземпляра данного класса 
        /// </summary>
        public SimplePreMSchedule(Config config, PreMConfig preMConfig) {
            this.config = config;
            this.preMConfig = preMConfig;

            // Если флаг оталдки установлен
            if (IsDebug) {

                // Выводим информацию о переданной конфигурационной структуре
                WriteToLogFile($"{config.ToString()}{Environment.NewLine}");
            }

            startProcessing = new Dictionary<int, List<List<int>>>();
            matrixTPM = new List<List<PreMSet>>();
        }

        public override bool Build(List<List<int>> _matrixA)
        {

            // Если установлено логирование
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Начинаем выполнять операции на нижнем уровне;{Environment.NewLine}");

            List<List<int>> matrixA = ListUtils.MatrixIntDeepCopy(_matrixA);

            // Если установлено логирование
            if (Form1.loggingOn)
            {

                // Cоздаём экземпляр класса для работы со строками
                StringBuilder str = new StringBuilder(200);

                // Объявляем временную строку
                str.AppendLine($"Матрица A:");

                // Для каждого типа данных
                for (int _dataType = 0; _dataType < matrixA.Count(); _dataType++)
                {

                    // Добавляем новые данные в строку
                    str.Append($"\tТип {_dataType + 1}: ");

                    // Для каждого пакета в векторе типа _dataType матрицы A
                    for (int _batchIndex = 0; _batchIndex < matrixA[_dataType].Count(); _batchIndex++)

                        // Добавляем в строку данные
                        str.Append($"{matrixA[_dataType][_batchIndex]} ");

                    // Добавляем перевод строки
                    str.Append(Environment.NewLine);
                }

                // Записываем заголовок
                WriteToLogFile(str.ToString());
            }

            // Объявляем тип данных
            int dataType;

            // Объявляем максимальное количество пакетов
            int maxBatchCount = 0;

            // Объявляем ПЗ
            int batch = 0;

            // Вычисляем максимальное количество пакетов среди всех типов данных
            calcMaxBatchCount();

            // Если установлено логирование
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"maxBatchCount: {maxBatchCount}{Environment.NewLine}");

            // Вернёт максимальное количество пакетов среди всех типов данных
            void calcMaxBatchCount()
            {
                // Выполняем обработку по типам
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                    // Выполняем поиск максимального количество пакетов
                    maxBatchCount = Math.Max(maxBatchCount, matrixA[dataType].Count);
            }

            Dictionary<int, double> m = new Dictionary<int, double>(capacity: this.config.dataTypesCount);
            List<int> dataTypes = new List<int>(capacity: this.config.dataTypesCount);
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
            {
                double sum = 0;
                for (int device = 1; device < this.config.deviceCount; device++)
                    sum +=
                        (double)this.config.proccessingTime[device][dataType] /
                        (double)this.config.proccessingTime[device - 1][dataType];
                m.Add(dataType, sum);
            }

            // Если установлено логирование
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Типы данных:{Environment.NewLine}");

            while (m.Any())
            {
                int myDataType = m.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                dataTypes.Add(myDataType);
                // Если установлено логирование
                if (Form1.loggingOn)

                    // Записываем данные в файл
                    WriteToLogFile($"\t{myDataType}: {m[myDataType]}{Environment.NewLine}");
                m.Remove(myDataType);
            }


            // Если установлено логирование
            if (Form1.loggingOn)
            {

                // Выводим информацию
                WriteToLogFile($"dataTypes:{Environment.NewLine}");

                // Для каждого типа
                for (int _dataType = 0; _dataType < this.config.dataTypesCount; _dataType++)

                    // Выводим информацию
                    WriteToLogFile($"\t{_dataType}: {dataTypes[_dataType]}{Environment.NewLine}");
            }

            // Сортируем матрицу A
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                matrixA[dataType].Sort();

            // Если установлено логирование
            if (Form1.loggingOn)
            {

                // Cоздаём экземпляр класса для работы со строками
                StringBuilder str = new StringBuilder(200);

                // Объявляем временную строку
                str.AppendLine($"Матрица A:");

                // Для каждого типа данных
                for (int _dataType = 0; _dataType < matrixA.Count(); _dataType++)
                {

                    // Добавляем новые данные в строку
                    str.Append($"\tТип {_dataType + 1}: ");

                    // Для каждого пакета в векторе типа _dataType матрицы A
                    for (int _batchIndex = 0; _batchIndex < matrixA[_dataType].Count(); _batchIndex++)

                        // Добавляем в строку данные
                        str.Append($"{matrixA[_dataType][_batchIndex]} ");

                    // Добавляем перевод строки
                    str.Append(Environment.NewLine);
                }

                // Записываем заголовок
                WriteToLogFile(str.ToString());
            }

            batch = 0;
            dataType = 0;

            // Объявляем количество пакетов заданий
            int batchCount = 0;

            // Для каждого типа данных
            for (int _dataType = 0; _dataType < matrixA.Count(); _dataType++)

                // Увеличиваем общее количество пакетов заданий
                batchCount += matrixA[_dataType].Count();

            // П.2 Добавляем 
            this.schedule = new List<Batch>(batchCount) { new Batch(
                dataTypes[dataType],
                matrixA[dataTypes[dataType]][batch]
            )};
            dataType++;

            // Если логирование установлено
            if (Form1.loggingOn) {
                CalcStartProcessing();
                WriteToLogFileSchedule();
            }
            // П.3 Инициализируем матрицу Y
            this.matrixY = new List<List<int>>(capacity: this.config.deviceCount);
            for (int device = 0; device < this.config.deviceCount; device++)
            {
                this.matrixY.Add(new List<int>());
                this.matrixY[device].Add(1);
            }
            // Если логирование установлено
            if (Form1.loggingOn)
            {
                WriteToLogFileMatrixY();
            }

            // Для каждого типа данных выполняем обрабоку
            for (; dataType < this.config.dataTypesCount; dataType++)
            {

                // Добавляем ПЗ в расписание 
                this.schedule.Add(new Batch(dataTypes[dataType], matrixA[dataTypes[dataType]][batch]));
                for (int device = 0; device < this.config.deviceCount; device++)
                    this.matrixY[device].Add(0);

                // Если логирование установлено
                if (Form1.loggingOn)
                {
                    WriteToLogFileStartProcessing();
                }

                // Если не было найдено расписания удовлетворяющему условию надёжности
                if (!this.SearchByPosition(5)) {

                    // Закрываем файл
                    UnsetLogFile();

                    // Возвращаем флаг неудачи
                    return false;
                }

                // Выполняем оптимизацию для позиций ПТО приборов
                this.ShiftMatrixY();

                // Проверяем условие надёжности
                if (!this.IsSolutionAcceptable()) {

                    // Закрываем файл
                    UnsetLogFile();

                    // Возвращаем флаг неудачи
                    return false;
                }
            }

            // Увеличиваем индекс вставляемого пакета задания
            batch++;

            // Выполняем обработку
            while (batch < maxBatchCount)
            {

                // Выполняем обработку для каждого типа данных
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                {

                    // Если индекс пакета превышает максимальный размер пакетов для типа данных dataType
                    if (batch >= matrixA[dataTypes[dataType]].Count)

                        // Продолжаем обработку для следующего типа данных
                        continue;

                    // Добавляем ПЗ в расписание 
                    this.schedule.Add(new Batch(dataTypes[dataType], matrixA[dataTypes[dataType]][batch]));
                    for (int device = 0; device < this.config.deviceCount; device++)
                        this.matrixY[device].Add(0);

                    // Если не было найдено расписания удовлетворяющему условию надёжности
                    if (!this.SearchByPosition(5)) {

                        // Закрываем файл
                        UnsetLogFile();

                        // Возвращаем флаг неудачи
                        return false;
                    }

                    // Выполняем оптимизацию для позиций ПТО приборов (ШАГ 15)
                    this.ShiftMatrixY();

                    // Проверяем условие надёжности
                    if (!this.IsSolutionAcceptable()) {

                        // Закрываем файл
                        UnsetLogFile();

                        // Возвращаем флаг неудачи
                        return false;
                    }
                }

                // Увеличиваем индекс пакета
                batch++;
            }

            // Если установлено логирование
            if (Form1.loggingOn) {

                // Записываем данные в файл
                WriteToLogFile($"Начинаем выполнять операции на нижнем уровне;{Environment.NewLine}");

                // Закрываем файл
                UnsetLogFile();
            }

            // Возвращяем флаг удачного построения расписания
            return true;
        }

        /// <summary>
        /// Выполняет построение матрицы моментов окончания времени выполнения ПТО.
        /// </summary>
        public void CalcMatrixTPM()
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn) 

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем матрицу T^pm;{Environment.NewLine}");
            
            // Отчищяем матрицу T^pm
            matrixTPM?.Clear();

            // Для каждого прибора выполням обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Инициализируем ПТО для прибора
                matrixTPM.Add(new List<PreMSet>());

                // Для каждого прибора в расписании выполняем обработку
                for (int batchIndex = 0; batchIndex < schedule.Count; batchIndex++)

                    // Если для текущей позиции есть ПТО
                    if (matrixY[device][batchIndex] == 1)

                        // Момент окончания времени выполнения ПТО на позиции batchIndex
                        matrixTPM[device].Add(

                            // Добавляем структуры данных
                            new PreMSet(

                                // Индекс ПЗ после которого будет выполняться ПТО
                                batchIndex,

                                // Момент начала времени выполнения последнего задания в пакете batchIndex на приборе device
                                startProcessing[device][batchIndex].Last() +

                                // Время выполнения задания с типов пакета на позиции batchIndex
                                config.proccessingTime[device][schedule[batchIndex].Type] +

                                // Время выполнения ПТО
                                preMConfig.preMaintenanceTimes[device]
                            )
                        );
            }
        }

        // ВЫРАЖЕНИЯ 1-6
        /// <summary>
        /// Выполняет построение матрицы начала времени выполнения заданий
        /// </summary>
        private void CalcStartProcessing()
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем матрицу T^0l;{Environment.NewLine}");

            // Объявляем индекс ПЗ
            int batchIndex;

            // Объявляем индекс прибора
            int device;

            // Объявляем индекс задания
            int job;

            // Отчищаяем матрицу моментов начала времени выполнения заданий
            startProcessing.Clear();

            // Инициалиизруем матрицу заданий в пакете
            List<List<int>> times = new List<List<int>>();
            for (batchIndex = 0; batchIndex < schedule.Count(); batchIndex++)
                times.Add(ListUtils.InitVectorInt(schedule[batchIndex].Size));

            // Инициализируем словарь соответствий
            for (device = 0; device < config.deviceCount; device++)
                startProcessing.Add(device, ListUtils.MatrixIntDeepCopy(times));

            // Выполняем обработку для первого прибора
            {

                // Инициализируем индекс первого прибора
                device = 0;

                // Инициализируем индекс первого ПЗ
                batchIndex = 0;

                // Инициализируем индекс первого задания
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на 1 приборе, как наладку
                startProcessing[device][batchIndex][job] = config.changeoverTime[device][schedule[batchIndex].Type][schedule[batchIndex].Type];

                // Пробегаемся по всем заданиям пакета в первой позиции
                for (job = 1; job < schedule[batchIndex].Size; job++)

                    // Устанавливаем момент начала времени выполнения задания job
                    startProcessing[device][batchIndex][job] =

                        // Момент начала времени выполнения предыдущего задания
                        startProcessing[device][batchIndex][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device][schedule[batchIndex].Type];

                // Пробегаемся по всем возможным позициям cо второго пакета
                for (batchIndex = 1; batchIndex < schedule.Count(); batchIndex++)
                {

                    // Инициализируем индекс первого задания
                    job = 0;

                    // Момент начала времени выполнения 1 задания в пакете на позиции batchIndex
                    startProcessing[device][batchIndex][job] =

                        // Момент начала времени выполнения последнего задания в предыдущем пакете
                        startProcessing[device][batchIndex - 1].Last() +

                        // Время выполнения задания в предыдущем пакете
                        config.proccessingTime[device][schedule[batchIndex - 1].Type] +

                        // Время переналадки с предыдущего типа на текущий
                        config.changeoverTime[device][schedule[batchIndex - 1].Type][schedule[batchIndex].Type] +

                        // Время выполнения ПТО после предыдущего ПЗ
                        preMConfig.preMaintenanceTimes[0] * matrixY[device][batchIndex - 1];

                    // Пробегаемся по всем заданиям пакета в позиции batchIndex
                    for (job = 1; job < schedule[batchIndex].Size; job++)

                        // Вычисляем момент начала времени выполнения задания job в позиции batchIndex на 1 приборе
                        startProcessing[device][batchIndex][job] =

                            // Момент начала времени выполнения предыдущего задания
                            startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device][schedule[batchIndex].Type];
                }
            }

            // Пробегаемся по всем приборам со второго
            for (device = 1; device < config.deviceCount; device++)
            {

                // Инициализируем индекс первого ПЗ
                batchIndex = 0;

                // Инициализируем индекс первого задания
                job = 0;

                // Устанавливаем момент начала времени выполнения 1 задания в 1 пакете на приборе device, как
                // Максимум, между временем наладки прибора на выполнение 1 задания в 1 пакете
                // и временем окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                startProcessing[device][batchIndex][job] = Math.Max(

                    // Время наладки прибора на выполнение 1 задания в 1 пакете
                    config.changeoverTime[device][schedule[batchIndex].Type][schedule[batchIndex].Type],

                    // Время окончания выполнения 1 задания в 1 пакете на предыдущем приборе
                    startProcessing[device - 1][batchIndex][job] + config.proccessingTime[device - 1][schedule[batchIndex].Type]
                );

                // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                for (job = 1; job < schedule[batchIndex].Size; job++)

                    // Устанавливаем момент начала времени выполнения текущего задания job, как
                    // Максимум, между временем окончания предыдущего задания на текущем приборе и
                    // временем окончания текущего задания на предыдущем приборе
                    startProcessing[device][batchIndex][job] = Math.Max(

                        // Момент начала времени выполнения предыдущего задания
                        startProcessing[device][batchIndex][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device][schedule[batchIndex].Type],

                        // Момент начала времени выполнения текущего задания на предыдущем приборе
                        startProcessing[device - 1][batchIndex][job] +

                        // Время выполнения текущего задания на предыдущем приборе
                        config.proccessingTime[device - 1][schedule[batchIndex].Type]
                    );

                // Пробегаемся по всем возможным позициям пакетов
                for (batchIndex = 1; batchIndex < schedule.Count(); batchIndex++)
                {

                    // Инициализируем индекс задания
                    job = 0;

                    // Устанавливаем момент начала времени выполнения 1 задания в пакете batchIndex на приборе device,
                    // как Максимум, между временем окончания выполнения последнего задания в предыдущем пакете вместе с переналадкой и ПТО
                    // и временем окончания выполнения 1 задания в пакете на в batchIndex на предыдущем приборе
                    startProcessing[device][batchIndex][job] = Math.Max(

                        // Момент начала времени выполнения последнего задания в предыдущем ПЗ
                        startProcessing[device][batchIndex - 1].Last() +

                        // Время выполнения последнего задания в предыдущем ПЗ
                        config.proccessingTime[device][schedule[batchIndex - 1].Type] +

                        // Время переналадки с предыдущего типа на текущий
                        config.changeoverTime[device][schedule[batchIndex - 1].Type][schedule[batchIndex].Type] +

                        // Время выполнения ПТО
                        preMConfig.preMaintenanceTimes[device] * matrixY[device][batchIndex - 1],

                        // Момент начала времени выполнения 1 задания на предыдущем приборе
                        startProcessing[device - 1][batchIndex][job] +

                        // Время выполнения 1 задания на предыдущем приборе
                        config.proccessingTime[device - 1][schedule[batchIndex].Type]);

                    // Пробегаемся по всем возможным заданиям пакета в позиции batchIndex
                    for (job = 1; job < schedule[batchIndex].Size; job++)

                        // Устанавливаем момент начала времени выполнения текущего задания job, как
                        // Максимум, между временем окончания предыдущего задания на текущем приборе и
                        // временем окончания текущего задания на предыдущем приборе
                        startProcessing[device][batchIndex][job] = Math.Max(

                            // Момент начала времени выполнения предыдущего задания
                            startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device][schedule[batchIndex].Type],

                            // Момент начала времени выполнения задания на предыдущем приборе
                            startProcessing[device - 1][batchIndex][job] +

                            // Время выполнения задания на предыдущем приборе
                            config.proccessingTime[device - 1][schedule[batchIndex].Type]
                        );
                }
            }
        }

        // ВЫРАЖЕНИЕ 7
        /// <summary>
        /// Возвращает простои для переданного индекса прибора, данного расписания
        /// </summary>
        /// <returns>Время простоя для переданного индекса прибора</returns>
        private int GetDowntimeByDevice(int device)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем простои для прибора {device};{Environment.NewLine}");

            // Объявляем и инициализируем простои
            int downtime = 0;

            // Подсчитываем простои связанные с наладкой
            downtime += startProcessing[device].First().First();

            // Для кажого задания пакета на первой позиции
            for (int job = 1; job < startProcessing[device].First().Count(); job++)

                // Подсчитываем простои между заданиями
                downtime +=

                    // Момент начала времени выполнения текущего задания
                    startProcessing[device][0][job] -

                    // Момент окончания времени выполнения предыдущего задания
                    (
                        // Момент начала времени выполнения предыдущего задания
                        startProcessing[device][0][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device][schedule[0].Type]
                    );

            // Для каждого пакета со второго выполняем обработку
            for (int batchIndex = 1; batchIndex < startProcessing[device].Count(); batchIndex++)
            {

                // Подсчитываем простои между пакетами
                downtime +=

                    // Момент начала времени выполнения первого задания текущего пакет
                    startProcessing[device][batchIndex][0] -

                    // Момент начала времени выполнения последнего задания на предыдущем пакете
                    (startProcessing[device][batchIndex - 1].Last() +

                    // Время выполнения задания в предыдущем пакете
                    config.proccessingTime[device][schedule[batchIndex - 1].Type]);

                // Для кажого задания пакета на первой позиции
                for (int job = 1; job < startProcessing[device][batchIndex].Count(); job++)

                    // Подсчитываем простои между заданиями
                    downtime +=

                        // Момент начала времени выполнения текущего задания
                        startProcessing[device][batchIndex][job] -

                        // Момент окончания времени выполнения предыдущего задания
                        (
                            // Момент начала времени выполнения предыдущего задания
                            startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device][schedule[batchIndex].Type]
                        );
            }
            
            // Возвращаем результат
            return downtime;
        }

        // ВЫРАЖЕНИЕ 7 ДЛЯ ВСЕХ ПРИБОРОВ
        /// <summary>
        /// Возвращает общие простои для данного расписания
        /// </summary>
        /// <returns>Время простоя</returns>
        public int GetDowntime()
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем простои для всех приборов;{Environment.NewLine}");

            // Объявляем и инициализируем простои
            int downtime = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Подсчитываем простои связанные с наладкой
                downtime += startProcessing[device].First().First();

                // Для кажого задания пакета на первой позиции
                for (int job = 1; job < startProcessing[device].First().Count(); job++)

                    // Подсчитываем простои между заданиями
                    downtime +=

                        // Момент начала времени выполнения текущего задания
                        startProcessing[device][0][job] -

                        // Момент начала времени выполнения предыдущего задания
                        (startProcessing[device][0][job - 1] +

                        // Время выполнения предыдущего задания
                        config.proccessingTime[device][schedule[0].Type]);

                // Для каждого пакета со второго выполняем обработку
                for (int batchIndex = 1; batchIndex < startProcessing[device].Count(); batchIndex++)
                {

                    // Подсчитываем простои между пакетами
                    downtime +=

                        // Момент начала времени выполнения первого задания текущего пакет
                        startProcessing[device][batchIndex][0] -

                        // Момент начала времени выполнения последнего задания на предыдущем пакете
                        (startProcessing[device][batchIndex - 1].Last() +

                        // Время выполнения задания в предыдущем пакете
                        config.proccessingTime[device][schedule[batchIndex - 1].Type]);

                    // Для кажого задания пакета на первой позиции
                    for (int job = 1; job < startProcessing[device][batchIndex].Count(); job++)

                        // Подсчитываем простои между заданиями
                        downtime +=

                            // Момент начала времени выполнения текущего задания
                            startProcessing[device][batchIndex][job] -

                            // Момент начала времени выполнения предыдущего задания
                            (startProcessing[device][batchIndex][job - 1] +

                            // Время выполнения предыдущего задания
                            config.proccessingTime[device][schedule[batchIndex].Type]);
                }
            }

            // Возвращаем результат
            return downtime;
        }

        // ВЫРАЖЕНИЕ 8 ДЛЯ ОДНОГО ПРИБОРА
        /// <summary>
        /// Возвращаем полезность для прибора по переданному индексу
        /// </summary>
        /// <returns>Критерий полезности</returns>
        private int GetUtilityByDevice(int device)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем полезность для прибора {device};{Environment.NewLine}");

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Добавляем момент времени окончания всех заданий на приборе
            sum +=

                // Момент начала времени выполнения на последнем задании в последнем пакете
                this.startProcessing[device].Last().Last() +

                // Время выполнения последнего заданий в последенем пакете
                this.config.proccessingTime[device][this.schedule.Last().Type];

            // Вычитаем простои
            sum -= this.GetDowntimeByDevice(device);

            // Возвращаем критерий
            return sum;
        }

        // ВЫРАЖЕНИЕ 8
        /// <summary>
        /// Возвращаем полезность для данного расписания
        /// </summary>
        /// <returns>Время полезности</returns>
        public int GetUtility()
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем полезность для всех приборов;{Environment.NewLine}");

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < this.config.deviceCount; device++)
            
                // Добавляем критерий для данного прибора
                sum += this.GetUtilityByDevice(device);
            
            // Возвращаем критерий
            return sum;
        }

        // ВЫРАЖЕНИЕ 9
        /// <summary>
        /// Возвращает сумму полезности и интервалов между ПТО для данного расписания
        /// </summary>
        /// <returns>Сумма полезности и интервалов между ПТО</returns>
        public int GetPreMUtility()
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn) {

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем полезность с учётом ПТО для всех приборов;{Environment.NewLine}");

                // Выводим информацию о матрице начал моментов времени выполнения
                WriteToLogFileStartProcessing();

                // Выводим информацию
                WriteToLogFile($"GetPreMUtility start: Вычисляем сумму полезности и итервалов между ПТО{Environment.NewLine}");
            }

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Если логирование установлено
                if (Form1.loggingOn) {
                    WriteToLogFile($"device:{device}{Environment.NewLine}");
                    WriteToLogFile($"\tМомент окончания последнего задания { this.startProcessing[device].Last().Last() + this.config.proccessingTime[device][this.schedule.Last().Type] }{Environment.NewLine}");
                }

                // Добавляем момент времени окончания всех заданий на приборе
                sum +=
                    
                    // Момент начала времени выполнения на последнем задании в последнем пакете
                    this.startProcessing[device].Last().Last() +

                    // Время выполнения последнего заданий в последенем пакете
                    this.config.proccessingTime[device][this.schedule.Last().Type];

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"\tПростои для данного прибора с учётом ПТО { this.GetDowntimeByDevice(device) }{Environment.NewLine}");

                // Вычитаем простои
                sum -= this.GetDowntimeByDevice(device);

                int intervals = matrixTPM[device].First().TimePreM;

                // Для каждого пакета выполняем обработку
                for (int batchIndex = 1; batchIndex < matrixTPM[device].Count(); batchIndex++)

                    // Добавляем интервалы времени между ПТО разных пакетов
                    intervals += matrixTPM[device][batchIndex].TimePreM - matrixTPM[device][batchIndex - 1].TimePreM;

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"\tИнтервалы времени между ПТО { intervals }{Environment.NewLine}");
                
                // Выполняем подсчёт суммы интервалов времени на первом пакете ПТО
                sum += intervals;
            }

            // Если логирование установлено
            if (Form1.loggingOn)
                WriteToLogFile($"Критерий f2 {sum}{Environment.NewLine}");

            // Возвращаем критерий
            return sum;
        }

        // ВЫРАЖЕНИЕ 10
        /// <summary>
        /// Возвращает надёжность, которая определяет вероятность находится ли некий прибор в работоспособном состоянии
        /// </summary>
        /// <param name="activity_time">Время активности прибора с момента последнего ПТО</param>
        /// <param name="device">Индекс прибора для которого расчитывается надёжность</param>
        /// <returns>Надёжность прибора по индексу device</returns>
        private double CalcReliabilityByDevice(int activity_time, int device)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем надёжность для прибора {device} с временем активности {activity_time};{Environment.NewLine}");

            // Выполняем расчёт и возврат доступности
            return (activity_time == 0) ? 1 :
                (double) preMConfig.restoringDevice[device] / (double)(preMConfig.failureRates[device] + preMConfig.restoringDevice[device]) +
                (double) preMConfig.failureRates[device]    / (double)(preMConfig.failureRates[device] + preMConfig.restoringDevice[device]) *
                (double) Math.Exp(-1 * (double)(preMConfig.failureRates[device] + preMConfig.restoringDevice[device]) * (double)activity_time);
        }

        // ВЫРАЖЕНИЕ 11
        /// <summary>
        /// Возвращает надёжность, которая определяет вероятность находится ли некий прибор в работоспособном состоянии
        /// </summary>
        /// <param name="activity_time">Время активности прибора с момента старта КС</param>
        /// <param name="prem_time">Момент времени окончания последнего ПТО</param>
        /// <param name="device">Индекс прибора для которого расчитывается надёжность</param>
        /// <returns>Надёжность прибора по индексу device</returns>
        // private double CalcReliabilityByDevice(int activity_time, int prem_time, int device)
        // {
        // 
        //     // Выполняем расчёт и возврат доступности по выражению 10
        //     return CalcReliabilityByDevice(activity_time - prem_time, device);
        // }

        // Выражение 12
        
        /// <summary>
        /// Функция вернёт время активности прибора от предыдущего ПТО
        /// </summary>
        /// <param name="device">Индекс прибора для которого расчитывается время активности</param>
        /// <param name="time">Крайний момент времени</param>
        /// <returns>Время активности</returns>
        private int GetActivityTimeByDevice(int device, int time)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем временя активности для прибора {device} для момента времени {time};{Environment.NewLine}");

            // Определяем начальный индекс
            int batchIndex = GetBatchIndex(device, time) + 1;

            // Определяем время активности
            int activityTime = 0;

            // Для каждого пакет выполняем обработку
            for (; batchIndex < schedule.Count; batchIndex++)

                // Для каждого задания выполняем обработку
                for (int job = 0; job < startProcessing[device][batchIndex].Count; job++)
                {

                    // Если момент начала времени выполнения выходит за границу
                    if (startProcessing[device][batchIndex][job] >= time)

                        // Вернём время активности
                        return activityTime;

                    // Высчитываем время выполнения
                    int proc_time = this.config.proccessingTime[device][this.schedule[batchIndex].Type];

                    // Высчитываем момент начала времени выполнения
                    int start_time = this.startProcessing[device][batchIndex][job];

                    // Если момент окончания задания выходит за указанные границы
                    if (start_time + proc_time > time)
                    {
                        // Увеличиваем время активности до прибора
                        activityTime += time - start_time;

                        // Возвращаем время активности
                        return activityTime;
                    }

                    // Увеличиваем время активности прибора
                    activityTime += proc_time;
                }

            // Возвращаем время активности
            return activityTime;
        }

        // ВЫРАЖЕНИЕ 12
        /// <summary>
        /// Возвращает время активности для конкретного прибора
        /// </summary>
        /// <param name="device">Прибор для которого расчитывается время активности</param>
        /// <param name="time">Крайний момент времени</param>
        /// <returns>Время активности</returns>
        // private int GetActivityTimeByDevice(int device, int time)
        // {
        // 
        //     // Определяем начальный индекс
        //     int startIndex = GetBatchIndex(device, time) + 1;
        //     if (IsDebug)
        //     {
        //         Console.WriteLine($"device:{device}; startIndex:{startIndex}");
        //     }
        // 
        //     // Определяем индекс ПЗ
        //     int batchIndex = startIndex;
        //     
        //     // Определяем количество ПЗ
        //     int batchCount = 0;
        //     
        //     // Определяем количество заданий
        //     int jobCount = 0;
        //     
        //     // Определяем время активности
        //     int activityTime = 0;
        //     
        //     // Для каждого пакет выполняем обработку
        //     for (; batchIndex < schedule.Count; batchIndex++)
        //     {
        //     
        //         // Если первое задани в ПЗ удовлетворяет условию
        //         if (startProcessing[device][batchIndex][0] >= time)
        //     
        //             // Прекращаем обарботку
        //             break;
        //     
        //         // Увеличиваем количество ПЗ
        //         batchCount++;
        //     
        //         // Увеличиваем активность
        //         activityTime += schedule[batchIndex].Size * config.proccessingTime[device, batchIndex];
        //     }
        //     
        //     // Если количество ПЗ равно 0
        //     if (batchCount == 0)
        //     
        //         // Добавляем информацию
        //         return 0;
        //     
        //     // Для каждого задания в последнем пакете выполняем обработку
        //     for (int job = 0; job < schedule[startIndex + batchCount - 1].Size; job++)
        //     {
        //     
        //         // Если некоторое задание в последенем ПЗ не удовлетворяет условию
        //         if (startProcessing[device][startIndex + batchCount - 1][job] >= time)
        //     
        //             // Прекращаем обарботку
        //             break;
        //     
        //         // Увеличиваем количество заданий
        //         jobCount++;
        //     }
        //     
        //     // Уменьшаем время
        //     activityTime -= (schedule[startIndex + batchCount - 1].Size - jobCount) * config.proccessingTime[device, startIndex + batchCount - 1];
        // 
        //     // Добавляем информацию
        //     return activityTime;
        // }

        // ВЫРАЖЕНИЕ 13

        /// <summary>
        /// Возвращает доступность для всех приборов для указанного момента времени
        /// </summary>
        /// <param name="time">Момент времени для которого выполняется расчёт надёжности</param>
        /// <returns>Доступность для всех приборов</returns>
        private double CalcSysReliability(int time)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем системную надёжность для момента времени {time};{Environment.NewLine}");
            
            // Объявляем надёжность
            double reliability = 1;

            // Объявляем время активности
            int activity_time;

            // Для каждого прибора подсчитываем надёжность
            for (int device = 0; device < config.deviceCount; device++) {

                // Вычисляем время активности
                activity_time = this.GetActivityTimeByDevice(device, time);

                // Если логирование установлено
                if (Form1.loggingOn)

                    // Выводим информацию
                    WriteToLogFile($"\tДля прибора {device} время активности {activity_time} и надёжность {this.CalcReliabilityByDevice(activity_time, device):0.000}{Environment.NewLine}");
                
                // Если прибор не был активным
                if (activity_time == 0)

                    // Пропускаем итерацию
                    continue;

                // Выполняем расчёт надёжности
                reliability *= this.CalcReliabilityByDevice(activity_time, device);
            }

            // Возвращаем надёжность
            return reliability;
        }

        // ВЫРАЖЕНИЕ 14
        public override int GetMakespan()
        {
            return startProcessing[config.deviceCount - 1].Last().Last() + config.proccessingTime[config.deviceCount - 1][schedule.Last().Type];
        }

        // ВЫРАЖЕНИЕ 15
        /// <summary>
        /// Возвращает результат совпадения количества заданий
        /// </summary>
        /// <param name="matrixA">Матрица количества заданий каждого типа на пакет[dataTypesCount x mi]</param>
        /// <returns>Если количество заданий совпадают - true, иначе false</returns>
        private bool IsConstraint_JobCount(List<List<int>> matrixA)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Проверяем ограничение 15;{Environment.NewLine}");

            // Объявляем количество заданий для текущего расписания
            int cur_jobCount = 0;

            // Объявляем необходимое количество заданий 
            int tar_jobCount = 0;

            // Для каждого пакета подсчитываем количество заданий
            for (int batch = 0; batch < this.schedule.Count; batch++)
            
                // Увеличиваем количество заданий в текущем расписании
                cur_jobCount += this.schedule[batch].Size;
            
            // Выполняем обход по типам
            for (int dataType = 0; dataType < matrixA.Count; dataType++)
            

                // Выполняем обход по пакетам
                for (int batch = 0; batch < matrixA[dataType].Count; batch++)

                    // Увеличиваем количество необходимых заданий
                    tar_jobCount += matrixA[dataType][batch];

            // Возвращаем результат сравнения
            return (cur_jobCount == tar_jobCount);
        }

        // ВЫРАЖЕНИЕ 16 = 9

        // ВЫРАЖЕНИЕ 17 = 11

        // ВЫРАЖЕНИЕ 18
        
        /// <summary>
        /// Возвращаем результат расчёта ограничения на общую надёжность
        /// </summary>
        /// <param name="time">Момент времени для которого выполняется расчёт надёжности</param>
        /// <returns>true, если ограничение выполняется. Иначе false</returns>
        private bool IsConstraint_CalcSysReliability(int time)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Проверяем ограничение 16 для момента времени {time};{Environment.NewLine}");

            double sysTime = this.CalcSysReliability(time);

            // Если логирование установлено
            if (Form1.loggingOn)

                // Выводим информацию
                WriteToLogFile($"Системная надёжность {sysTime:0.000} >= {preMConfig.beta:0.000}?{Environment.NewLine}");

            return (sysTime >= preMConfig.beta);
        }

        // ВЫРАЖЕНИЕ 19 ИЗБЫТОЧНО

        // ВЫРАЖЕНИЕ 20 
        
        /// <summary>
        /// Возвращает индекс последнего ПЗ после которого выполняется ПТО до заданного времени
        /// </summary>
        /// <param name="device">Индекс прибора по которому будет выполняться выборка</param>
        /// <param name="time">Крайний момент времени окончания ПТО</param>
        /// <returns>Индекс ПЗ после которого будет выполняться последнее ПТО</returns>
        private int GetBatchIndex(int device, int time)
        {
            
            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем индекс последнего ПЗ для прибора {device} для момента времени {time};{Environment.NewLine}");

            // Если список пустой
            if (matrixTPM[device].Count == 0)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Если в списке первый элемент не удовлетворяет условию
            // TODO: Баг, определяет пакет по окончанию ПТО, когда необходимо по началу ПТО.
            if (matrixTPM[device][0].TimePreM > time)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Если в списке первый элемент не удовлетворяет условию
            if (matrixTPM[device][0].TimePreM == time)

                // Вернём индекс начальный индекс ПЗ
                return matrixTPM[device][0].BatchIndex;

            // Объявляем индекс
            int index = 1;

            // Для каждой ПТО выполняем обработку
            for (; index < matrixTPM[device].Count; index++)
            {

                // Если момент окончания ПТО в позиции index не удовлетворяет условиям
                if (matrixTPM[device][index].TimePreM > time)

                    // Возвращаем индекс ПЗ после которого выполнится последнее ПТО
                    return matrixTPM[device][index - 1].BatchIndex;

                // Если момент окончания ПТО в позиции index не удовлетворяет условиям
                if (matrixTPM[device][index].TimePreM == time)

                    // Возвращаем индекс ПЗ после которого выполнится последнее ПТО
                    return matrixTPM[device][index].BatchIndex;
            }

            // Индекс ПЗ после которог выполниться ПТО, последний в списке
            return matrixTPM[device][index - 1].BatchIndex;
        }

        // ВЫРАЖЕНИЕ 21 = 12

        // ВЫРАЖЕНИЕ 22 ИЗБЫТОЧНО

        // ВЫРАЖЕНИЕ 23
        
        /// <summary>
        /// Возвращает результат совпадения количества пакетов заданий
        /// </summary>
        /// <param name="matrixA">Матрица количества заданий каждого типа на пакет[dataTypesCount x mi]</param>
        /// <returns>Если количество пакетов заданий совпадают - true, иначе false</returns>
        private bool IsConstraint_BatchCount(List<List<int>> matrixA)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Проверяем ограничение 21;{Environment.NewLine}");

            // Объявляем количество пакетов заданий
            int cur_batchCount = this.schedule.Count;

            // Объявляем необходимое количество пакетов заданий 
            int tar_batchCount = 0;

            // Выполняем обход по типам
            for (int dataType = 0; dataType < matrixA.Count; dataType++)
            
                // Увеличиваем количество пакетов
                tar_batchCount += matrixA[dataType].Count;

            // Возвращаем true
            return (cur_batchCount == tar_batchCount);
        }

        // ВЫРАЖЕНИЕ 24
        /// <summary>
        /// Возвращаем результат совпадения одного пакета на позицию расписания
        /// </summary>
        /// <returns>Если пакет на позицию 1, то true. Иначе false</returns>
        private bool IsConstraint_OneBatchOnPos()
        {
            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Проверяем ограничение 24;{Environment.NewLine}");

            // Существующая реализация расписания обязывает иметь один пакет на позицию
            return true;
        }

        // ВЫРАЖЕНИЕ 25
        /// <summary>
        /// Возвращает результат совпадения количества пакетов заданий каждого типа
        /// </summary>
        /// <param name="matrixA">Матрица количества заданий каждого типа на пакет[dataTypesCount x mi]</param>
        /// <returns>Если количество пакетов заданий по типам совпадают - true, иначе false</returns>
        private bool IsConstraint_BatchCountByType(List<List<int>> matrixA)
        {

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"Проверяем ограничение 25;{Environment.NewLine}");

            // Объявляем количество пакетов заданий
            int cur_batchCountByType;

            // Объявляем необходимое количество пакетов заданий 
            int tar_batchCountByType;

            // Выполняем обход по типам
            for (int dataType = 0; dataType < matrixA.Count; dataType++)
            {

                // Увеличиваем количество пакетов
                tar_batchCountByType = matrixA[dataType].Count;

                // Обнуляем значение количества пакето заданий заданного типа
                cur_batchCountByType = 0;

                // Для каждого пакета заданий выполняем обработку
                for (int batch = 0; batch < this.schedule.Count; batch++)

                    // Увеличиваем количество пакетов заданий текущего типа
                    cur_batchCountByType += (this.schedule[batch].Type == dataType) ? 1 : 0;

                // Выполяем проверку
                if (tar_batchCountByType != cur_batchCountByType)

                    // Возвращаем false
                    return false;
            }

            // Возвращаем true
            return true;
        }

        /// <summary>
        /// Вернёт тип данных по переданному индексу ПЗ
        /// </summary>
        /// <param name="batchIndex">Индекс ПЗ</param>
        /// <returns>Тип данных</returns>
        public int GetDataTypeByBatchIndex(int batchIndex)
        {
            return this.schedule[batchIndex].Type;
        }

        public override List<List<int>> GetMatrixP()
        {

            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(config.dataTypesCount);

            // Инициализируем матрицу
            for (int dataType = 0; dataType < this.config.dataTypesCount; dataType++)

                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(this.schedule.Count));

            // Для каждого элемента матрицы schedule
            for (int batchIndex = 0; batchIndex < this.schedule.Count; batchIndex++)

                // Заполняем элементы матрицы количества заданий в пакетах
                res[this.schedule[batchIndex].Type][batchIndex] = 1;

            // Возвращаем результат
            return res;
        }

        public override List<List<int>> GetMatrixR()
        {
            
            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(this.config.dataTypesCount);

            // Инициализируем матрицу
            for (int dataType = 0; dataType < config.dataTypesCount; dataType++)
            
                // Инициализируем строку матрицы нулями
                res.Add(ListUtils.InitVectorInt(schedule.Count));

            // Для каждого элемента матрицы schedule
            for (int batchIndex = 0; batchIndex < this.schedule.Count; batchIndex++)

                // Заполняем элементы матрицы количества заданий в пакетах
                res[this.schedule[batchIndex].Type][batchIndex] = this.schedule[batchIndex].Size;

            // Возвращаем результат
            return res;
        }

        public override List<List<int>> GetMatrixTPM()
        {

            CalcMatrixTPM();

            // Объявляем матрицу
            List<List<int>> res = new List<List<int>>(matrixTPM.Count);

            // Инициализируем матрицу
            for (int device = 0; device < config.deviceCount; device++) {

                // Инициализируем строки матрицы
                res.Add(new List<int>(matrixTPM[device].Count));

                // Для каждого элемента матрицы matrixTPM
                for (int batchIndex = 0; batchIndex < schedule.Count; batchIndex++)

                    // Устанавливаем в 0
                    res[device].Add(0);

                // Для каждого элемента матрицы matrixTPM
                for (int batchIndex = 0; batchIndex < matrixTPM[device].Count; batchIndex++)
                    
                    // Инициализируем столбцы матрицы
                    res[device][matrixTPM[device][batchIndex].BatchIndex] = matrixTPM[device][batchIndex].TimePreM;
            }

            // Возвращаем результат
            return res;
        }

        public override List<List<int>> GetMatrixY()
        {
            return matrixY;
        }

        public override Dictionary<int, List<List<int>>> GetStartProcessing()
        {
            CalcStartProcessing();
            return startProcessing;
        }
    }
}
