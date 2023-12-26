using newAlgorithm.Model;
using System.Collections.Generic;

namespace magisterDiplom.Model
{

    /// <summary>
    /// Структура описывает конфигурацию по которой необходимо выполнить построение расписания
    /// </summary>
    public struct Config
    {

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
        /// Конструктор конфигурации конвейерной системы. Содержит в себе всю входную
        /// информацию небходимую для выполнения локально-оптимальных вычислений
        /// </summary>
        public Config(
            int dataTypesCount,
            int deviceCount,
            int buffer,
            Matrix proccessingTime,
            Dictionary<int, Matrix> changeoverTime,
            bool isFixedBatches
            )
        {
            this.dataTypesCount = dataTypesCount;
            this.deviceCount = deviceCount;
            this.buffer = buffer;
            this.proccessingTime = proccessingTime;
            this.changeoverTime = changeoverTime;
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
    }
}
