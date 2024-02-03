using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace magisterDiplom.Model
{

    /// <summary>
    /// Структура описывает конфигурацию по которой необходимо выполнить построение расписания
    /// </summary>
    public struct Config
    {

        /// <summary>
        /// Данная переменная устанавливаем режим отладки для всей программы
        /// </summary>
        public readonly static bool isDebug = true;

        /// <summary>
        /// Данная переменная определяет являются ли партии фиксированными
        /// </summary>
        public readonly bool isFixedBatches;

        /// <summary>
        /// Данная переменная определяет количество типов данных в конвейерной системе
        /// </summary>
        public readonly int dataTypesCount;

        /// <summary>
        /// Данная переменная определяет размер буфера перед приборами
        /// </summary>
        public readonly int buffer;

        /// <summary>
        /// Данная переменная определяет длину конвейера, как количество приборов
        /// </summary>
        public readonly int deviceCount;

        /// <summary>
        /// Данная переменная представляет из словарь соответсвия прибора к матрице переналадки. 
        /// Для каждого прибора есть матрица переналадки приборов с одного типа задания на другой.
        /// Таким образом changeoverTime = [deviceCount] : [dataTypesCount x dataTypesCount]
        /// </summary>
        public readonly Dictionary<int, Matrix> changeoverTime;

        /// <summary>
        /// Данная переменная представляет из себя двумрную матрицу и используется, как матрица времени выполнения заданий.
        /// Первое измерение представляет определяется, как количество приборов на конвейере. Второе измерения это количество
        /// типов данных. Таким образом proccessingTime = [deviceCount x dataTypesCount]
        /// </summary>
        public readonly Matrix proccessingTime;

        /// <summary>
        /// Данная переменная содержим в себе список из времён вермени выполнения ПТО для соответсвующего прибора.
        /// preMaintenanceTimes = [deviceCount]
        /// </summary>
        public readonly Vector preMaintenanceTimes;

        /// <summary>
        /// Конструктор конфигурации конвейерной системы. Содержит в себе всю входную
        /// информацию небходимую для выполнения локально-оптимальных вычислений
        /// </summary>
        public Config(
            int dataTypesCount,
            int deviceCount,
            int buffer,
            Matrix proccessingTime,
            Dictionary<int, Matrix> changeoverTime,
            Vector preMaintenanceTimes,
            bool isFixedBatches
            )
        {
            this.dataTypesCount = dataTypesCount;
            this.deviceCount = deviceCount;
            this.buffer = buffer;
            this.proccessingTime = proccessingTime;
            this.changeoverTime = changeoverTime;
            this.preMaintenanceTimes = preMaintenanceTimes;
            this.isFixedBatches = isFixedBatches;
        }

        /// <summary>
        /// Данная функция выполняем преобразование данных из матричной 3-ёх мерной формы в словарь матриц
        /// </summary>
        /// <param name="changeoverTime">3-ёх мерная матрица</param>
        /// <returns>Словарь соответствий приборов и матриц переналадок</returns>
        public static Dictionary<int, Matrix> changeoverTimeConverter(List<List<List<int>>> changeoverTime)
        {

            // Создаём словарь матриц переналадки
            Dictionary<int, Matrix> _changeoverTime = new Dictionary<int, Matrix>();

            // Для каждого прибора выполняем переопределение данных в необходимую форму
            for (int device = 0; device < changeoverTime.Count; device++)
                _changeoverTime.Add(device, (Matrix)(new Matrix(changeoverTime[device])));

            // Возвращаем словарь матриц переналадки
            return _changeoverTime;
        }

        /// <summary>
        /// Данная функция формируем выходную строку об конфигурационной структуре
        /// </summary>
        /// <param name="prefix">Префикс для формированного вывода</param>
        /// <returns>Результирующая строка со всей необходимой информацией</returns>
        public string ToString(string prefix = "\t")
        {

            // Результирующая информация
            string res = "";

            // Добавляем информацию о фиксированности пакетов
            res += prefix + $"isFixedBatches: {this.isFixedBatches}" + Environment.NewLine;

            // Добавляем информацию о количестве типов данных
            res += prefix + $"dataTypesCount: {this.dataTypesCount}" + Environment.NewLine;

            // Добавляем информацию о количестве приборов
            res += prefix + $"deviceCount:    {this.deviceCount}" + Environment.NewLine;

            // Добавляем информацию о размере буфера
            res += prefix + $"buffer:         {this.buffer}" + Environment.NewLine;

            // Выполняем формирование вывода времени переналадки
            int _device = 0;
            res += prefix + "preMaintenanceTimes: ";
            for (_device = 0; _device < this.deviceCount - 1; _device++)
                res += $"{this.preMaintenanceTimes[_device],-2}, ";
            res += $"{this.preMaintenanceTimes[_device]};" + Environment.NewLine;

            // Выполняем формирование вывода времени выполнения
            res += prefix + "proccessingTime:" + Environment.NewLine;
            for (int device = 0; device < this.deviceCount; device++)
            {
                int dataType = 0;
                res += prefix + prefix + $"Device {device}: " + prefix;
                for (dataType = 0; dataType < this.dataTypesCount - 1; dataType++)
                    res += $"{this.proccessingTime[device, dataType],-2}, ";
                res += $"{this.proccessingTime[device, dataType]};" + Environment.NewLine;
            }
            res += prefix + "changeoverTime:" + Environment.NewLine;

            // Выполняем формирование вывода времени переналадки
            for (int device = 0; device < this.deviceCount; device++)
            {
                res += prefix + prefix + $"Device {device}: " + Environment.NewLine;
                for (int dataTypeRow = 0; dataTypeRow < this.dataTypesCount; dataTypeRow++)
                {
                    int dataTypeCol = 0;
                    res += prefix + prefix + prefix + $"Type {dataTypeRow}: " + prefix;
                    for (dataTypeCol = 0; dataTypeCol < this.dataTypesCount - 1; dataTypeCol++)
                        res += $"{this.changeoverTime[device][dataTypeRow, dataTypeCol],-2}, ";
                    res += $"{this.changeoverTime[device][dataTypeRow, dataTypeCol]};" + Environment.NewLine;
                }
            }

            // Возвращяем результат
            return res;
        }
    }
}
