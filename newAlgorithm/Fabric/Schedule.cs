using magisterDiplom.Model;
using System.Collections.Generic;

namespace magisterDiplom
{

    /// <summary>
    /// Абстрактный класс содержащий общие функции для расписания
    /// </summary>
    public abstract class Schedule
    {

        /// <summary>
        /// Конфигурационная структура содержащая информацию о конвейерной системе
        /// </summary>
        protected Config config;

        /// <summary>
        /// Матрица порядка и количества пакетов заданий [deviceCount]
        /// </summary>
        protected List<Batch> schedule;

        /// <summary>
        /// Словарь соответсвий приборов и матриц моментов начала времени выполнения заданий
        /// </summary>
        protected Dictionary<int, List<List<int>>> startProcessing;

        /// <summary>
        /// Возвращяет матрицу количества заданий в пакетах
        /// </summary>
        /// <returns>Матрица количества заданий в пакетах</returns>
        public abstract List<List<int>> GetMatrixR();

        /// <summary>
        /// Возвращяет матрицу распорядке пакетов заданий
        /// </summary>
        /// <returns>Матрица распорядкя пакетов заданий</returns>
        public abstract List<List<int>> GetMatrixP();

        /// <summary>
        /// Возвращяет 3 мерную матрицу моментов времени выполнения заданий
        /// </summary>
        /// <returns>Словарь соответствия прибора к матрице моментов времени выполнения заданий в пакетах</returns>
        public abstract Dictionary<int, List<List<int>>> GetStartProcessing();

        /// <summary>
        /// Возвращает критерий оптимизации makespan, определяющий время выполнения всех заданий в конвейерной системе
        /// </summary>
        /// <returns>Makespan - время выполнения всех заданий в системе</returns>
        public abstract int GetMakespan();
    }
}