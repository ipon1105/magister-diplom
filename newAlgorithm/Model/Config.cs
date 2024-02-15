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
        /// Данный список определяет интенсивность отказов для приборов соответсвенно: [deviceCount]
        /// </summary>
        public readonly Vector failureRates;

        /// <summary>
        /// Данный список определяет востановление прибора соответсвенно: [deviceCount]
        /// </summary>
        public readonly Vector restoringDevice;

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
            Vector failureRates,
            Vector restoringDevice,
            bool isFixedBatches
            )
        {

            // Вектор ПТО равен null
            if (preMaintenanceTimes == null)
                throw new ArgumentNullException("The PreM vector is null.");

            // Вектор отказов равен null
            if (failureRates == null)
                throw new ArgumentNullException("The failureRates vector is null.");

            // Вектор отказов равен null
            if (failureRates.getCount() != deviceCount)
                throw new ArgumentException("The number of items in the list failureRates does not match the deviceCount.");

            // Вектор востановления равен null
            if (restoringDevice == null)
                throw new ArgumentNullException("The restoringDevice vector is null.");

            // Вектор отказов равен null
            if (restoringDevice.getCount() != deviceCount)
                throw new ArgumentException("The number of items in the list restoringDevice does not match the deviceCount.");

            // Размер вектора ПТО не совпадает с количеством приборов
            if (preMaintenanceTimes.getCount() != deviceCount)
                throw new ArgumentException("The number of items in the list preMaintenanceTimes does not match the deviceCount.");

            // Матрица времени выполнения равна null
            if (proccessingTime == null)
                throw new ArgumentNullException("The proccessingTime matrix is null.");

            // Размер матрицы времени выполнения не совпадает с количеством приборов
            if (proccessingTime.rowCount != deviceCount)
                throw new ArgumentException("The number of items in the list proccessingTime does not match the deviceCount.");

            // Выполняем проверку исключений для матрицы времени выполнения
            for (int device = 0; device < proccessingTime.rowCount; device++)
            {

                // Размер матрицы времени выполнения не совпадает с количеством типов
                if (proccessingTime.GetVectorSize(device) != dataTypesCount)
                    throw new ArgumentException("The number of items in the list proccessingTime does not match the dataTypesCount.");
            }

            // Словарь соответствий времени переналадки равен null
            if (changeoverTime == null)
                throw new ArgumentNullException("The changeoverTime matrix is null.");

            // Размер словаря переналадки не совпадает с количеством приборов
            if (changeoverTime.Count != deviceCount)
                throw new ArgumentException("The number of items in the Dictionary changeoverTime does not match the deviceCount.");

            // Выполняем проверку для каждого прибора
            for (int device = 0; device < changeoverTime.Count; device++)
            {

                // Размер матрицы по словарю переналадки не совпадает с количеством типов
                if (changeoverTime[device].rowCount != dataTypesCount)
                    throw new ArgumentException("The number of items in the Dictionary changeoverTime does not match the dataTypesCount.");

                // Размер вектора матрицы по словарю переналадки не совпадает с количеством типов
                for (int dataType = 0; dataType < changeoverTime[device].rowCount; dataType++)
                    if (changeoverTime[device].GetVectorSize(dataType) != dataTypesCount)
                        throw new ArgumentException("The number of items in the Dictionary changeoverTime does not match the dataTypesCount.");
            }

            // Выполняем инициализацию
            this.dataTypesCount = dataTypesCount;
            this.deviceCount = deviceCount;
            this.buffer = buffer;
            this.proccessingTime = proccessingTime;
            this.changeoverTime = changeoverTime;
            this.preMaintenanceTimes = preMaintenanceTimes;
            this.isFixedBatches = isFixedBatches;
            this.restoringDevice = restoringDevice;
            this.failureRates = failureRates;
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

            // Выполняем формирование вывода времени отказов приборов
            _device = 0;
            res += prefix + "restoringDevice: ";
            for (_device = 0; _device < this.deviceCount - 1; _device++)
                res += $"{this.restoringDevice[_device],-2}, ";
            res += $"{this.restoringDevice[_device]};" + Environment.NewLine;

            // Выполняем формирование вывода времени востановления приборов
            _device = 0;
            res += prefix + "failureRates: ";
            for (_device = 0; _device < this.deviceCount - 1; _device++)
                res += $"{this.failureRates[_device],-2}, ";
            res += $"{this.failureRates[_device]};" + Environment.NewLine;
            
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
