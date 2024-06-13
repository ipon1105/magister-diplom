using magisterDiplom.Model;
using magisterDiplom.Utils;
using newAlgorithm;
using Newtonsoft.Json.Linq;
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
        /// Список приоритетных типов данных
        /// </summary>
        private List<int> priorityDataTypesList;
        
        /// <summary>
        /// Поток записи в файл
        /// </summary>
        private StreamWriter fstream = null;

        /// <summary>
        /// Установит файл записи
        /// </summary>
        /// <param name="filename">Имя файла для записи</param>
        public void SetLogFile(string filename)
        {

            // Создаём объект для записи в файл
            fstream = new StreamWriter(filename, false, Encoding.UTF8);
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

        public string GetMatrixByString(List<List<int>> matrix, string prefix = "")
        {
            // Объявляем и вычисляем количество символов для отрисовки значения матрицы
            int charLen = (int)(Math.Log10(matrix.Max(row => row.Max()))) + 1;

            // Cоздаём экземпляр класса для работы со строками
            StringBuilder stringBuilder = new StringBuilder(

                // Вычисляем количество символов необходимое для отрисовки значений матрицы
                matrix.Select(row => row.Count).Sum() * charLen +

                // Количество разделителей между элементами матрицы в строке
                matrix.Select(row => (row.Count + 1)).Sum() +

                // Количество разделительных символов между строками данных
                matrix.Select(row => (row.Count * charLen) + row.Count + 1).Sum() +

                // Разделитель в конце
                matrix.Last().Count * charLen + matrix.Last().Count + 1 +

                // Количество символов для префикса
                (matrix.Count + 1) * prefix.Length +

                // Количество символов перевода строки
                (matrix.Count * 2 + 1) * (Environment.NewLine).Length
            );

            // Для каждой строки
            for (int i = 0; i < matrix.Count; i++)
            {

                // Добавляем в строку префикс
                stringBuilder.Append(prefix);

                // Добавляем межстрочный разделитель
                for (int j = 0; j < matrix[i].Count; j++)
                    stringBuilder.Append("+".PadRight(charLen + 1, '-'));
                stringBuilder.AppendLine("+");

                // Добавляем в строку префикс
                stringBuilder.Append(prefix);

                // Для каждого столбца
                for (int j = 0; j < matrix[i].Count; j++)

                    // Добавляем в строку данные
                    stringBuilder.Append(string.Format("|{0}", $"{matrix[i][j]}".PadRight(charLen)));

                // Добавляем в текст перевод строки
                stringBuilder.AppendLine("|");
            }

            // Добавляем в строку префикс
            stringBuilder.Append(prefix);

            // Добавляем межстрочный разделитель
            for (int j = 0; j < matrix.Last().Count; j++)
                stringBuilder.Append("+".PadRight(charLen + 1, '-'));
            stringBuilder.AppendLine("+");

            // Возвращяем строку
            return stringBuilder.ToString(); 
        }

        /// <summary>
        /// Выполнит запись матрицы в файл
        /// </summary>
        /// <param name="matrix">Матрица для записи в файл</param>
        /// <param name="prefix">Префикс перед каждой стройо матрицы</param>
        private void WriteToLogFileMatrix(List<List<int>> matrix, string prefix = "")
        {

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Объявляем и вычисляем количество символов для отрисовки значения матрицы
            int charLen = (int)(Math.Log10(matrix.Max(row => row.Max()))) + 1;

            // Cоздаём экземпляр класса для работы со строками
            StringBuilder stringBuilder = new StringBuilder(

                // Вычисляем количество символов необходимое для отрисовки значений матрицы
                matrix.Select(row => row.Count).Sum() * charLen +

                // Количество разделителей между элементами матрицы в строке
                matrix.Select(row => (row.Count + 1)).Sum() +

                // Количество разделительных символов между строками данных
                matrix.Select(row => (row.Count * charLen) + row.Count + 1).Sum() +

                // Разделитель в конце
                matrix.Last().Count * charLen + matrix.Last().Count + 1 +

                // Количество символов для префикса
                (matrix.Count + 1) * prefix.Length +

                // Количество символов перевода строки
                (matrix.Count * 2 + 1) * (Environment.NewLine).Length
            );

            // Для каждой строки
            for (int i = 0; i < matrix.Count; i++) {

                // Добавляем в строку префикс
                stringBuilder.Append(prefix);

                // Добавляем межстрочный разделитель
                for (int j = 0; j < matrix[i].Count; j++)
                    stringBuilder.Append("+".PadRight(charLen + 1, '-'));
                stringBuilder.AppendLine("+");

                // Добавляем в строку префикс
                stringBuilder.Append(prefix);

                // Для каждого столбца
                for (int j = 0; j < matrix[i].Count; j++)
                
                    // Добавляем в строку данные
                    stringBuilder.Append(string.Format("|{0}", $"{matrix[i][j]}".PadRight(charLen)));

                // Добавляем в текст перевод строки
                stringBuilder.AppendLine("|");
            }

            // Добавляем в строку префикс
            stringBuilder.Append(prefix);

            // Добавляем межстрочный разделитель
            for (int j = 0; j < matrix.Last().Count; j++)
                stringBuilder.Append("+".PadRight(charLen + 1, '-'));
            stringBuilder.AppendLine("+");

            // Записываем данные
            WriteToLogFile(stringBuilder.ToString());
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

            // Записываем данные в файл
            fstream.Write(text);
        }

        /// <summary>
        /// Выводим матриц порядка ПТО
        /// </summary>
        private void WriteToLogFileMatrixY(string prefix = "")
        {

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Выводим информационное сообщение
            WriteToLogFile($"{prefix}\"Матрица порядка ПТО приборов [Y]\": {{{Environment.NewLine}");
            WriteToLogFileMatrix(this.matrixY, prefix + "\t");
            WriteToLogFile($"{prefix}}},{Environment.NewLine}");
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

            // Выводим информацию о порядке ПЗ
            WriteToLogFile($"{prefix}\"Матрица порядка ПЗ [P]\": {{{Environment.NewLine}");
            WriteToLogFileMatrix(this.GetMatrixP(), prefix + "\t");
            WriteToLogFile($"{prefix}}},{Environment.NewLine}");

            // Выводим информацию о количестве заданий ПЗ
            WriteToLogFile($"{prefix}\"Матрица количества заданий в ПЗ [R]\": {{{Environment.NewLine}");
            WriteToLogFileMatrix(this.GetMatrixR(), prefix + "\t");
            WriteToLogFile($"{prefix}}},{Environment.NewLine}");
        }

        /// <summary>
        /// Функция выполняет вывод матрицы моментов начала времени выполнения заданий и ПТО
        /// </summary>
        /// <param name="prefix">Префикс перед выводом матрицы</param>
        private void WriteToLogFileStartProcessing(String startPrefix = "")
        {

            // Объявляем и инциализируем префикс строк
            string prefix = startPrefix + "\t";

            // Если объекта для записи не существует
            if (fstream == null)

                // Закончим обработку
                return;

            // Выводим сообщение
            WriteToLogFile($"{startPrefix}\"Матрица моментов начала времени выполнения заданий и ПТО\": {{{Environment.NewLine}");

            // Создаём строку с 10000 символов вместимостью
            StringBuilder str = new StringBuilder(10000);

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

            // Записываем данные в файл
            fstream.Write(str);
            
            // Выводим сообщение
            WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
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
        private void ShiftMatrixY(string startPrefix = "") 
        {
            
            // Вычисляем префикс
            string prefix = startPrefix + "\t";
            
            // Если логирование установлена
            if (Form1.loggingOn){

                // Выводим информацию
                WriteToLogFile($"{startPrefix}\"Улучшаем позиции ПТО\": {{{Environment.NewLine}");
            }

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

                    // Выводим информацию
                    WriteToLogFile($"{prefix}\"Итерация сдвигов\":{{{Environment.NewLine}");
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

                    // Проверяем на необходимость проведения операций перестановки
                    if (last_prem_batch_index == last_batch_index) 

                        // Пропускаем итерацию для текущего прибора
                        continue;

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"{prefix}\t\"Сдвиг для прибора {device + 1}\": \"j*={last_prem_batch_index}; j^max={last_batch_index}\",{Environment.NewLine}");

                    // Выполняем сдвиг ПТО на следующую позицию
                    this.matrixY[device][last_prem_batch_index] = 0;
                    this.matrixY[device][last_prem_batch_index + 1] = 1;

                    // Вычисляем матрицу моментов начала времени выполнения заданий
                    this.CalcStartProcessing();

                    // Вычисляем матрицу моментов окончания времени выполнения ПТО
                    this.CalcMatrixTPM();

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFileStartProcessing(prefix + "\t");

                    // Если текущее решение не удовлетворяет условию надёжности
                    if (!this.IsSolutionAcceptable(prefix + "\t"))
                    {
                        // Если логирование установлено
                        if (Form1.loggingOn)
                            WriteToLogFile($"{prefix}\t\"Решение\": \"Не допустимо\",{Environment.NewLine}");
                        
                        // Выполняем обратный сдвиг ПТО
                        this.matrixY[device][last_prem_batch_index] = 1;
                        this.matrixY[device][last_prem_batch_index + 1] = 0;

                        // Пропускаем итерацию
                        continue;
                    }

                    // Вычисляем критерий f2 для текущего расписания со сдвигом
                    new_f2 = this.GetPreMUtility(prefix + "\t");

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"{prefix}\t\"Решение\": \"Допустимо с f2={new_f2}\",{Environment.NewLine}");

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

                // Если улучшений позиций ПТО не было найдено
                if (f2 == 0) {

                    // Если логирование установлено
                    if (Form1.loggingOn) {


                        // Выводим информацию
                        WriteToLogFile($"{prefix}\t\"Статус итерации\": \"Сдвигов найдено не было.\",{Environment.NewLine}");    // Выводим информацию
                        WriteToLogFile($"{prefix}}}{Environment.NewLine}");
                    }
                    
                    // Прекращаем обработку
                    break;
                }

                // Определяем индекс ПЗ за которым следует последнее ПТО
                last_prem_batch_index = this.GetLastPreMPos(bestDevice); // j*

                // Выполняем их переопределение
                this.matrixY[bestDevice][last_prem_batch_index] = 0;
                this.matrixY[bestDevice][last_prem_batch_index + 1] = 1;

                // Если логирование установлено
                if (Form1.loggingOn) {

                    // Выводим информацию
                    WriteToLogFile($"{prefix}\t\"Статус итерации\": \"Сдвиг был найден с f2={f2}\",{Environment.NewLine}");
                    WriteToLogFileMatrixY(prefix + "\t");
                    WriteToLogFileStartProcessing(prefix + "\t");
                    WriteToLogFile($"{prefix}}},{Environment.NewLine}");
                }

                // Продолжаем улучшения
            } while (true);

            // Если логирование установлено
            if (Form1.loggingOn)
            {

                // Выводим информацию
                WriteToLogFile($"{prefix}\"Решение с помощью дополнения\": {{{Environment.NewLine}");
            }

            // Для каждого прибора выполняем дополнение для матрицы ПТО 1
            for (int device = 0; device < this.config.deviceCount; device++)
            {
                // Если логирование установлено
                if (Form1.loggingOn) { 
                    WriteToLogFile($"{prefix}\t\"Для прибора {device + 1}\": ");
                }

                // Определяем индекс ПЗ за которым следует последнее ПТО
                last_prem_batch_index = this.GetLastPreMPos(device); // j*

                // Определяем индекс последнего ПЗ для текущего расписания
                last_batch_index = this.matrixY[device].Count() - 1; // j^max

                // Если матрица Y не оканчивается 1
                if (last_prem_batch_index < last_batch_index) {

                    // Если логирование установлено
                    if (Form1.loggingOn)
                        WriteToLogFile($"\"Добавляется\",{Environment.NewLine}");

                    // Изменяем индекс последнего ПТО нп 1
                    this.matrixY[device][last_batch_index] = 1;
                }
                
                // Если логирование установлено
                else if (Form1.loggingOn)
                    WriteToLogFile($"\"Не добавляется\",{Environment.NewLine}");
            }

            // Если логирование установлено
            if (Form1.loggingOn)
            {

                // Выводим информацию
                WriteToLogFile($"{prefix}}},{Environment.NewLine}");
            
                // Выводим информационное сообщение
                WriteToLogFileMatrixY(prefix);
                WriteToLogFileStartProcessing(prefix);
                WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Данная функция выполняет локальную оптимизацию составов ПЗ
        /// </summary>
        /// <param name="swapCount">Количество перестановок</param>
        /// <returns>true, если была найдено перестановка удовлетворяющая условию надёжности. Иначе false</returns>
        private bool SearchByPosition(int swapCount = 999999, string startPrefix = "")
        {

            // Вычисляем префикс
            string prefix = startPrefix + "\t";

            // Если логирование установлено
            if (Form1.loggingOn)

                // Выводим информационное сообщение
                WriteToLogFile($"{startPrefix}\"Находим лучшую позицию ПТО в окрестности {swapCount}\": {{{Environment.NewLine}");

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
            if (Form1.loggingOn) {
                
                // Выводим сообщение
                WriteToLogFile($"{prefix}\"Начальное расписание\":{{{Environment.NewLine}");
                WriteToLogFileSchedule(prefix + "\t");
                WriteToLogFileStartProcessing(prefix + "\t");
                WriteToLogFile($"{prefix}}},{Environment.NewLine}");
            }

            // Проверяем допустимость текущего решения
            if (this.IsSolutionAcceptable(prefix)) {

                // Устанавливаем флаг перестановки выполняющей условию надёжности
                isFind = true;

                // Получаем f2 критерий - момента времени окончания последнего задания
                bestTime = this.GetPreMUtility(prefix);

                // Устанавливаем лучшее расписание
                bestSchedule = new List<Batch>(this.schedule);

                // Если логирование установлено
                if (Form1.loggingOn) {
                    WriteToLogFile($"{prefix}\"Начальное расписание\": \"Допустимо с f2={bestTime}\",{Environment.NewLine}");
                }
            }
            
            // Если логирование установлено
            else if (Form1.loggingOn)
                WriteToLogFile($"{prefix}\"Начальное расписание\": \"Не допустимо\",{Environment.NewLine}");

            // Выполняем заявленное количество перестановок, заявленно количество раз
            for (int batchIndex = schedule.Count - 1; batchIndex > 0 && swapCount > 0; batchIndex--, swapCount--)
            {

                // Выполняем перестановку
                (this.schedule[batchIndex - 1], this.schedule[batchIndex]) = (this.schedule[batchIndex], this.schedule[batchIndex - 1]);

                // Вычисляем матрицу моментов времени начала выполнения заданий
                this.CalcStartProcessing();

                // Вычисляем матрицу моментов окончания времени ПТО приборов
                this.CalcMatrixTPM();

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"{prefix}\"Меняем местами ПЗ\": \"{batchIndex + 1} и {batchIndex}\",{Environment.NewLine}");
                
                // Проверяем допустимость текущего решения
                if (this.IsSolutionAcceptable(prefix))
                {

                    // Устанавливаем флаг перестановки выполняющей условию надёжности
                    isFind = true;

                    // Высчитываем новый критерий makespan
                    newTime = this.GetPreMUtility(prefix);

                    // Если логирование установлено
                    if (Form1.loggingOn)
                    {
                        // Если логирование установлено
                        WriteToLogFileStartProcessing(prefix);
                        WriteToLogFile($"{prefix}\"Текущее расписание\": \"Допустимо с f2={newTime}\",{Environment.NewLine}");
                    }

                    // Если новое время лучше, то выполняем переопределение
                    if (newTime > bestTime)
                    {

                        // Если логирование установлено
                        if (Form1.loggingOn)
                            WriteToLogFile($"{prefix}\"Текущее расписание\": \"Лучше предыдущего. ({newTime} > {bestTime})\",{Environment.NewLine}");

                        // TODO: Избавиться от копирования списка в пользу использования индекса наилучшей позиции
                        // Переопределяем лучшее расписание
                        bestSchedule = new List<Batch>(schedule);

                        // Переопределяем лучшее время для лучшего расписания
                        bestTime = newTime;
                    }
                }
                
                // Если логирование установлено
                else if (Form1.loggingOn) {

                    // Если логирование установлено
                    WriteToLogFileStartProcessing(prefix);
                    WriteToLogFile($"{prefix}\"Текущее расписание\": \"Не допустимо\",{Environment.NewLine}");
                }
            }

            // Выполняем переопределение наилучшего раысписания составов ПЗ
            this.schedule = bestSchedule;

            // Если логирование установлено
            if (Form1.loggingOn ) {

                // Выводим сообщение
                WriteToLogFile($"{prefix}\"Извлекаем лучшее расписание\": {{{Environment.NewLine}");
                    WriteToLogFileSchedule(prefix + "\t");
                    WriteToLogFileStartProcessing(prefix + "\t");
                WriteToLogFile($"{prefix}}},{Environment.NewLine}");

                // Выводим информационное сообщение
                WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
            
            }

            // Возвращаем результат
            return isFind;
        }

        /// <summary>
        /// Функция проверяет допустимость решения
        /// </summary>
        /// <returns>true - если текущее решение допустимо. Иначе False</returns>
        private bool IsSolutionAcceptable(string startPrefix = "")
        {

            // Вычисляем префикс
            string prefix = startPrefix + "\t";

            // Если логирование установлено
            if (Form1.loggingOn) {

                // Выводим информационное сообщение
                WriteToLogFile($"{startPrefix}\"Проверяем расписание на надёжность\": {{{Environment.NewLine}");

                // Выводим информационное сообщение
                WriteToLogFile($"{prefix}\"Информацию\":\"Проверяем допустимость решения при нижнем пороге надёжности {preMConfig.beta}\", {Environment.NewLine}");
                
                // Выводим информационное сообщение
                WriteToLogFileMatrixY(prefix);

                // Выводим информационное сообщение
                WriteToLogFileStartProcessing(prefix);
            }

            // Для каджого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            
                // Для каждого ПЗ в расписании выполняем обработку
                for (int batch = 0; batch < this.schedule.Count; batch++)

                    // Если для данной позиции существует ПТО
                    if (this.matrixY[device][batch] != 0)
                    {
                        
                        // Вычисляем момент времени начала выполнения ПТО для текущего прибора в текущей позиции
                        int time =

                            // Момент времени начала выполнения последнего задания на текущем приборе в текущей позиции
                            this.startProcessing[device][batch].Last() +

                            // Время выполнения задания на текущем приборе в текущей позиции 
                            this.config.proccessingTime[device][this.schedule[batch].Type];

                        // Вычисляем матрицу T^pm
                        CalcMatrixTPM();

                        // Проверяем ограничение надёжности
                        if (!IsConstraint_CalcSysReliability(time, prefix)) {

                            // Если логирование установлено
                            if (Form1.loggingOn) {

                                // Выводим информационное сообщение
                                WriteToLogFile($"{prefix}\"Результат\":\"Ограничение не выполняется\",{Environment.NewLine}");
                                WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
                            }

                            // Если ограничение не выполняется, вернуть false
                            return false;
                        }

                        // Если логирование установлено
                        if (Form1.loggingOn)
                            WriteToLogFile($"{prefix}\"Результат\":\"Ограничение выполняется\",{Environment.NewLine}");
                    }

            // Если логирование установлено
            if (Form1.loggingOn) {

                // Выводим информационное сообщение
                WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
            }

            // Все ограничения выпоняются, вернуть true
            return true;
        }

        /// <summary>
        /// Конструктор выполняющий создание экземпляра данного класса 
        /// </summary>
        public SimplePreMSchedule(Config config, PreMConfig preMConfig) {

            // Выполняем присваивание переменных
            this.config = config;
            this.preMConfig = preMConfig;

            startProcessing = new Dictionary<int, List<List<int>>>();
            matrixTPM = new List<List<PreMSet>>();

            // Инициализируем список приоритетных типов данных
            priorityDataTypesList = new List<int>(capacity: this.config.dataTypesCount);
            
            // Вычисляем приоритетные типы данных
            {

                // Объявляем словарь типа и отношения
                Dictionary<int, double> m = new Dictionary<int, double>(capacity: this.config.dataTypesCount);

                // Объявляем отношение типа
                double ratio = 0.0d;

                // Объявляем тип данных
                int dataType = 0;

                // Для каждого типа данных
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++) {

                    // Обнуляем отношение типа данных
                    ratio = 0;

                    // Для каждого типа данных
                    for (int device = 1; device < this.config.deviceCount; device++)

                        // Вычисляем отношение между временем выполнения на разных типах данных
                        ratio +=
                            (double)this.config.proccessingTime[device][dataType] /
                            (double)this.config.proccessingTime[device - 1][dataType];

                    // Добавляем в словарь тип и его отношение
                    m.Add(dataType, ratio);
                }

                // До тех пор, пока словарь не пуст
                while (m.Any())
                {

                    // Получаем тип с наилучшим отношением
                    dataType = m.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

                    // Добавляем тип с наилучшим отношением в список приоритетных типов
                    priorityDataTypesList.Add(dataType);

                    // Удаляем из словаря извлечённый тип данных
                    m.Remove(dataType);
                }
            }
        }

        public override bool Build(List<List<int>> _matrixA)
        {

            // Отчищаем расписание ПЗ
            this.schedule?.Clear();

            // Отчищаем матрицу порядка ПТО приборов
            this.matrixY?.Clear();

            // Объявляем тип данных
            int dataType;

            // Объявляем индекс ПЗ
            int batchIndex;

            // Объявляем максимальное количество ПЗ среди всех типов данных
            int maxBatchCount;

            // Объявляем общее количество ПЗ среди всех типов данных
            int batchCount;

            // Устанавливаем prefix строк
            string prefix = "\t";

            // Проверяем входные данные
            {

                // Проверяем количество строк матрицы A
                if (_matrixA.Count != config.dataTypesCount) {

                    // Если установлено логгирование
                    if (Form1.loggingOn) {

                        // Записываем данные в файл
                        WriteToLogFile($"[Ошибка] Количество строк в матрице A некорректно;{Environment.NewLine}");

                        // Закрываем файл
                        UnsetLogFile();
                    }

                    // Выбрасываем исключение
                    throw new IndexOutOfRangeException($"The number of rows in matrix A is incorrect.");
                }

                // Для каждого типа данных
                for (dataType = 0; dataType < config.dataTypesCount; dataType++)

                    // Проверяем количество ПЗ в строках матрицы A
                    if (_matrixA[dataType].Count == 0) {

                        // Если установлено логгирование
                        if (Form1.loggingOn) { 

                            // Записываем данные в файл
                            WriteToLogFile($"[Ошибка] Количество ПЗ в матрице A некорректно;{Environment.NewLine}");
                            
                            // Закрываем файл
                            UnsetLogFile();
                        }

                        // Выбрасываем исключение
                        throw new IndexOutOfRangeException($"The number of batches of jobs in matrix A is incorrect.");
                    }

                // Для каждого типа данных
                for (dataType = 0; dataType < config.dataTypesCount; dataType++)

                    // Для каждого состава
                    for (batchIndex = 0; batchIndex < _matrixA[dataType].Count; batchIndex++)

                        // Проверяем диапазон данных
                        if (_matrixA[dataType][batchIndex] <= 0 || _matrixA[dataType][batchIndex] >= 1000000) {

                            // Если установлено логгирование
                            if (Form1.loggingOn) { 

                                // Записываем данные в файл
                                WriteToLogFile($"[Ошибка] Знаение в матрице A некорректно;{Environment.NewLine}");

                                // Закрываем файл
                                UnsetLogFile();
                            }

                            // Выбрасываем исключение
                            throw new ArgumentException($"The value in matrix A must be between (0, 1000000).");
                        }
            }

            // Если установлено логирование
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"{{{Environment.NewLine}{prefix}\"Информация\":\"Начинаем выполнять операции на нижнем уровне\",{Environment.NewLine}");
            
            // Копируем переданную матрицу составов ПЗ
            List<List<int>> matrixA = ListUtils.MatrixIntDeepCopy(_matrixA);

            // Если установлено логирование
            if (Form1.loggingOn) {

                // Выводим матрицу в файл
                WriteToLogFile($"{prefix}\"Входная матрица [A] составов ПЗ\": {{{Environment.NewLine}");
                WriteToLogFileMatrix(matrixA, prefix + "\t");
                WriteToLogFile($"{prefix}}},{Environment.NewLine}");
            }

            // Объявляем и вычисляем максимальное количество пакетов среди всех типов данных
            maxBatchCount = matrixA.Select(x => x.Count).Max();

            // Если установлено логирование
            if (Form1.loggingOn) {

                // Записываем данные в файл
                WriteToLogFile($"{prefix}\"Максимальное количество ПЗ\": {maxBatchCount},{Environment.NewLine}");
            }

            // Объявляем и вычисляем количество пакетов заданий
            batchCount = matrixA.Select(row => row.Count()).Sum();

            // Если установлено логирование
            if (Form1.loggingOn)
            {

                // Записываем данные в файл
                WriteToLogFile($"{prefix}\"Общее количество ПЗ\": {batchCount},{Environment.NewLine}");
            }
            
            // Сортируем матрицу A
            for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                matrixA[dataType].Sort();

            // Если установлено логирование
            if (Form1.loggingOn)
            {

                // Выводим матрицу в файл
                WriteToLogFile($"{prefix}\"Выходная матрица [A] составов ПЗ\": {{{Environment.NewLine}");
                WriteToLogFileMatrix(matrixA, prefix + "\t");
                WriteToLogFile($"{prefix}}},{Environment.NewLine}");

                // Выводим информацию
                WriteToLogFile($"{prefix}\"Типы по приоритетам\": {{{Environment.NewLine}");
                for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                    WriteToLogFile($"{prefix}\t\"Тип {dataType + 1} по приоритету стоит в позиции {priorityDataTypesList[dataType] + 1}\",{Environment.NewLine}");
                WriteToLogFile($"{prefix}}},{Environment.NewLine}");
            }

            // Создаём расписание с нужной вместимостью
            this.schedule = new List<Batch>(batchCount);

            // Создаём матрицу порядк ПТО с нужной вместимостью
            this.matrixY = new List<List<int>>(capacity: this.config.deviceCount);
            for (dataType = 0; dataType < this.config.deviceCount; dataType++)
                this.matrixY.Add(new List<int>(maxBatchCount));

            // Добавляем первые ПЗ
            {

                // Устанавливаем индекс типа данных в 0
                dataType = 0;

                // Устанавливаем индекс пакета задания в 0
                batchIndex = 0;

                // Добавляем новый ПЗ в расписание
                this.schedule.Add(
                    new Batch(
                        priorityDataTypesList[dataType],
                        matrixA[priorityDataTypesList[dataType]][batchIndex]
                    )
                );

                // Для каждой строки матрицы порядка ПТО приборов добавляем 1
                this.matrixY.ForEach(row => { row.Add(1); });

                // Вычисляем словарь матрицы моментов начала времени выполнения
                this.CalcStartProcessing();

                // Если логирование установлено
                if (Form1.loggingOn) {

                    // Выводим информационное сообщение
                    WriteToLogFile($"{prefix}\"Первый ПЗ с типом {priorityDataTypesList[dataType] + 1} и количеством заданий {matrixA[priorityDataTypesList[dataType]][batchIndex]} был добавлен в расписание\": {{{Environment.NewLine}");
                    WriteToLogFileSchedule(prefix + "\t");

                    // Выводим информационное сообщение
                    WriteToLogFileMatrixY(prefix + "\t");

                    // Выводим информационное сообщение
                    WriteToLogFileStartProcessing(prefix + "\t");
                }

                // Проверяем условие надёжности
                if (!this.IsSolutionAcceptable(prefix + "\t")) {

                    // Если логирование установлено
                    if (Form1.loggingOn) {

                        // Выводим информационное сообщение
                        WriteToLogFile($"{prefix}}},{Environment.NewLine}");

                        // Записываем данные в файл
                        WriteToLogFile($"{prefix}\"Неуспешно заканчиваем выполнять операции на нижнем уровне\",{Environment.NewLine}");
                        WriteToLogFile($"}}");
                    }

                    // Закрываем файл
                    UnsetLogFile();

                    // Возвращаем флаг неудачи
                    return false;
                }

                // Если логирование установлено
                if (Form1.loggingOn) {

                    // Выводим информационное сообщение
                    WriteToLogFile($"{prefix}}},{Environment.NewLine}");
                }

                // Для остальных типов данных выполняем обработку
                for (dataType = 1; dataType < this.config.dataTypesCount; dataType++) {

                    // Добавляем новый ПЗ в расписание 
                    this.schedule.Add(
                        new Batch(
                            priorityDataTypesList[dataType],
                            matrixA[priorityDataTypesList[dataType]][batchIndex]
                        )
                    );

                    // Для каждой строки матрицы порядка ПТО приборов добавляем 0
                    this.matrixY.ForEach(row => { row.Add(0); });

                    // Если логирование установлено
                    if (Form1.loggingOn) {

                        // Выводим информационное сообщение
                        WriteToLogFile($"{prefix}\"Новый ПЗ с типом {priorityDataTypesList[dataType] + 1} и количеством заданий {matrixA[priorityDataTypesList[dataType]][batchIndex]} был добавлен в расписание\": {{{Environment.NewLine}");
                        WriteToLogFileSchedule(prefix + "\t");

                        // Выводим информационное сообщение
                        WriteToLogFileMatrixY(prefix + "\t");

                        // Выводим информационное сообщение
                        WriteToLogFileStartProcessing(prefix + "\t");
                    }

                    // Если не было найдено расписания удовлетворяющему условию надёжности
                    if (!this.SearchByPosition(5, prefix + "\t")) {

                        // Если логирование установлено
                        if (Form1.loggingOn) {

                            // Записываем данные в файл
                            WriteToLogFile($"{prefix}\t\"Неуспешно заканчиваем выполнять операции на нижнем уровне\",{Environment.NewLine}");
                            WriteToLogFile($"{prefix}}}{Environment.NewLine}");
                            WriteToLogFile($"}}");
                        }

                        // Закрываем файл
                        UnsetLogFile();

                        // Возвращаем флаг неудачи
                        return false;
                    }

                    // Выполняем оптимизацию для позиций ПТО приборов
                    this.ShiftMatrixY(prefix + "\t");

                    // Проверяем условие надёжности
                    if (!this.IsSolutionAcceptable(prefix + "\t")) {

                        // Если логирование установлено
                        if (Form1.loggingOn) {

                            // Записываем данные в файл
                            WriteToLogFile($"{prefix}\t\"Неуспешно заканчиваем выполнять операции на нижнем уровне\",{Environment.NewLine}");
                            WriteToLogFile($"{prefix}}}{Environment.NewLine}");
                            WriteToLogFile($"}}");
                        }

                        // Закрываем файл
                        UnsetLogFile();

                        // Возвращаем флаг неудачи
                        return false;
                    }

                    // Если логирование установлено
                    if (Form1.loggingOn) {

                        // Записываем данные в файл
                        WriteToLogFile($"{prefix}}},{Environment.NewLine}");
                    }
                }
            }

            // Добавляем оставшиеся ПЗ
            {

                // Устанавливаем индекс пакета задания в 1
                batchIndex = 1;

                // Выполняем обработку
                while (batchIndex < maxBatchCount)
                {

                    // Выполняем обработку для каждого типа данных
                    for (dataType = 0; dataType < this.config.dataTypesCount; dataType++)
                    {

                        // Если индекс пакета превышает максимальный размер пакетов для типа данных dataType
                        if (batchIndex >= matrixA[priorityDataTypesList[dataType]].Count)

                            // Продолжаем обработку для следующего типа данных
                            continue;

                        // Добавляем новый ПЗ в расписание 
                        this.schedule.Add(
                            new Batch(
                                priorityDataTypesList[dataType],
                                matrixA[priorityDataTypesList[dataType]][batchIndex]
                            )
                        );

                        // Для каждой строки матрицы порядка ПТО приборов добавляем 0
                        this.matrixY.ForEach(row => { row.Add(0); });

                        // Если логирование установлено
                        if (Form1.loggingOn)
                        {

                            // Выводим информационное сообщение
                            WriteToLogFile($"{prefix}\"Новый ПЗ с типом {priorityDataTypesList[dataType] + 1} и количеством заданий {matrixA[priorityDataTypesList[dataType]][batchIndex]} был добавлен в расписание\": {{{Environment.NewLine}");
                            WriteToLogFileSchedule(prefix + "\t");

                            // Выводим информационное сообщение
                            WriteToLogFileMatrixY(prefix + "\t");

                            // Выводим информационное сообщение
                            WriteToLogFileStartProcessing(prefix + "\t");
                        }

                        // Если не было найдено расписания удовлетворяющему условию надёжности
                        if (!this.SearchByPosition(5, prefix + "\t"))
                        {

                            // Если логирование установлено
                            if (Form1.loggingOn)
                            {

                                // Записываем данные в файл
                                WriteToLogFile($"{prefix}\t\"Неуспешно заканчиваем выполнять операции на нижнем уровне\":1{Environment.NewLine}");
                                WriteToLogFile($"{prefix}}}{Environment.NewLine}");
                                WriteToLogFile($"}}");
                            }

                            // Закрываем файл
                            UnsetLogFile();

                            // Возвращаем флаг неудачи
                            return false;
                        }

                        // Выполняем оптимизацию для позиций ПТО приборов
                        this.ShiftMatrixY(prefix + "\t");

                        // Проверяем условие надёжности
                        if (!this.IsSolutionAcceptable(prefix + "\t"))
                        {

                            // Если логирование установлено
                            if (Form1.loggingOn)
                            {

                                // Записываем данные в файл
                                WriteToLogFile($"{prefix}\t\"Неуспешно заканчиваем выполнять операции на нижнем уровне\":1{Environment.NewLine}");
                                WriteToLogFile($"{prefix}}}{Environment.NewLine}");
                                WriteToLogFile($"}}");
                            }

                            // Закрываем файл
                            UnsetLogFile();

                            // Возвращаем флаг неудачи
                            return false;
                        }

                        // Если логирование установлено
                        if (Form1.loggingOn)
                        {

                            // Записываем данные в файл
                            WriteToLogFile($"{prefix}}},{Environment.NewLine}");
                        }
                    }

                    // Увеличиваем индекс пакета
                    batchIndex++;
                }
            }

            // Если установлено логирование
            if (Form1.loggingOn) {

                // Записываем данные в файл
                WriteToLogFile($"{prefix}\"Успешно заканчиваем выполнять операции на нижнем уровне\":1{Environment.NewLine}");
                WriteToLogFile($"}}");

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

        // ВЫРАЖЕНИЯ 1-6
        /// <summary>
        /// Выполняет построение матрицы начала времени выполнения заданий
        /// </summary>
        /// <param name="startBatchIndex">Индекс ПЗ с которого будет рассчитываться матрица моментов начала времени выполнения</param>
        private void CalcStartProcessingFromBatchIndex(int startBatchIndex = 0, string prefix = "")
        {

            // Если индекс ПЗ меньше 0
            if (startBatchIndex < 0)

                // Устанавливаем индекс ПЗ равным 0
                startBatchIndex = 0;

            // Если установлено логирование
            if (Form1.loggingOn) { 

                // Записываем данные в файл
                WriteToLogFile($"Вычисляем матрицу T^0l начиная от ПЗ в позиции {startBatchIndex + 1};{Environment.NewLine}");

                // Выводим информационное сообщение
                WriteToLogFileStartProcessing(prefix);
            }
            
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

            // Если установлено логирование
            if (Form1.loggingOn) {

                // Выводим информационное сообщение
                WriteToLogFileStartProcessing(prefix);
            }
        }

        // ВЫРАЖЕНИЕ 7
        /// <summary>
        /// Возвращает простои для переданного индекса прибора, данного расписания
        /// </summary>
        /// <returns>Время простоя для переданного индекса прибора</returns>
        private int GetDowntimeByDevice(int device)
        {

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
        public int GetPreMUtility(string startPrefix = "\t")
        {

            // Вычисляем префикс
            string prefix = startPrefix + "\t";

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn) {

                // Записываем данные в файл
                WriteToLogFile($"{startPrefix}\"Вычисляем критерий f2\": {{{Environment.NewLine}");

                // Выводим информационное сообщение
                WriteToLogFileStartProcessing(prefix);
            }

            // Объявляем значение критерия на нижнем уровне
            int sum = 0;

            // Для каждого прибора выполняем обработку
            for (int device = 0; device < config.deviceCount; device++)
            {

                // Если логирование установлено
                if (Form1.loggingOn) {
                    WriteToLogFile($"{prefix}\"Прибор {device + 1}\": {{{Environment.NewLine}");
                    WriteToLogFile($"{prefix}\t\"Момент окончания последнего задания\": { this.startProcessing[device].Last().Last() + this.config.proccessingTime[device][this.schedule.Last().Type] },{Environment.NewLine}");
                }

                // Добавляем момент времени окончания всех заданий на приборе
                sum +=
                    
                    // Момент начала времени выполнения на последнем задании в последнем пакете
                    this.startProcessing[device].Last().Last() +

                    // Время выполнения последнего заданий в последенем пакете
                    this.config.proccessingTime[device][this.schedule.Last().Type];

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"{prefix}\t\"Простои для данного прибора с учётом ПТО\": { this.GetDowntimeByDevice(device) },{Environment.NewLine}");

                // Вычитаем простои
                sum -= this.GetDowntimeByDevice(device);

                int intervals = matrixTPM[device].First().TimePreM;

                // Для каждого пакета выполняем обработку
                for (int batchIndex = 1; batchIndex < matrixTPM[device].Count(); batchIndex++)

                    // Добавляем интервалы времени между ПТО разных пакетов
                    intervals += matrixTPM[device][batchIndex].TimePreM - matrixTPM[device][batchIndex - 1].TimePreM;

                // Если логирование установлено
                if (Form1.loggingOn)
                    WriteToLogFile($"{prefix}\t\"Интервалы времени между ПТО\": { intervals }{Environment.NewLine}{prefix}}},{Environment.NewLine}");
                
                // Выполняем подсчёт суммы интервалов времени на первом пакете ПТО
                sum += intervals;
            }

            // Если логирование установлено
            if (Form1.loggingOn) {
                WriteToLogFile($"{prefix}\"Критерий f2\": {sum},{Environment.NewLine}");
                WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
            }

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
        private double CalcSysReliability(int time, string startPrefix = "")
        {
            
            // Вычисляем префикс
            string prefix = startPrefix + "\t";

            // Если установлено логирование и объект для записи существует
            if (Form1.loggingOn)

                // Записываем данные в файл
                WriteToLogFile($"{startPrefix}\"Время {time}\": {{{Environment.NewLine}");

            // Объявляем надёжность
            double reliability = 1;

            // Объявляем временную надёжность
            double tempReliability = 1;

            // Объявляем время активности
            int activity_time;

            // Для каждого прибора подсчитываем надёжность
            for (int device = 0; device < config.deviceCount; device++) {

                // Вычисляем время активности
                activity_time = this.GetActivityTimeByDevice(device, time);

                // Вычисляем временную надёжность
                tempReliability = this.CalcReliabilityByDevice(activity_time, device);

                // Если логирование установлено
                if (Form1.loggingOn)

                    // Выводим информацию
                    WriteToLogFile($"{prefix}\"Прибор {device + 1}\": \"Время активности {activity_time} и надёжность {tempReliability:0.000}\",{Environment.NewLine}");
                
                // Если прибор не был активным
                if (activity_time == 0)

                    // Пропускаем итерацию
                    continue;

                // Выполняем расчёт надёжности
                reliability *= tempReliability;
            }

            // Если логирование установлено
            if (Form1.loggingOn){

                // Выводим информацию
                WriteToLogFile($"{prefix}\"Вопрос\": \"Системная надёжность {reliability:0.0000} >= {preMConfig.beta:0.0000}?\",{Environment.NewLine}");

                // Записываем данные в файл
                WriteToLogFile($"{startPrefix}}},{Environment.NewLine}");
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
        private bool IsConstraint_CalcSysReliability(int time, string prefix = "")
        {

            // Возвращяем результат
            return (this.CalcSysReliability(time, prefix) >= preMConfig.beta);
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
            
            // Если список пустой
            if (matrixTPM[device].Count == 0)

                // Вернём индекс начальный индекс ПЗ
                return -1;

            // Если в списке первый элемент не удовлетворяет условию
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
