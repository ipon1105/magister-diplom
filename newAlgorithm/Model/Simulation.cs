using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magisterDiplom.Model
{

    /// <summary>
    /// Данный класс используется для симуляции работы конвейерной системы
    /// </summary>
    public class Simulation
    {

        /// <summary>
        /// Если true значит симуляция запущена, иначе False
        /// </summary>
        private bool isStart;

        /// <summary>
        /// Определяем переменную с информацией о конвейере
        /// </summary>
        private Conveyor conveyor;

        /// <summary>
        /// Словарь, для каждого прибора которого возвращается матрица переналадки приборов
        /// </summary>
        private Dictionary<int, Matrix> changeoverTime;

        /// <summary>
        /// Матрица времени выполнения заданий
        /// </summary>
        private Matrix proccessingTime;

        private Job job;

        /// <summary>
        /// Данный конструктор возвращает экземпляр данного класса
        /// </summary>
        public Simulation(Conveyor conveyor, Dictionary<int, Matrix> changeoverTime, Matrix proccessingTime)
        {

            // Выполняем инициализацию
            this.isStart = false;
            this.job = new Job(1, 0);
            this.conveyor = conveyor;
            this.changeoverTime = changeoverTime;
            this.proccessingTime = proccessingTime;

            // TODO: Выполнить обработку конструктора

        }

        /// <summary>
        /// Выполняет начало симуляции
        /// </summary>
        public void Start()
        {

            // Выполняем проверку на состояние симуляции
            if (this.isStart)
                return;

            // Выполняем запуск симуляции
            this.isStart = true;

            // TODO: Выполнить запуск симуляции

        }

        /// <summary>
        /// Выполняет шаг симуляции на заданное число шагов, по умолчанию 1
        /// </summary>
        /// <param name="step">Число шагов симуляции</param>
        public void Step(int step = 1)
        {

            // Выполняем проверку на состояние симуляции
            if (!this.isStart)
                return;

            // Выполняем проверку входных данных
            if (step <= 1)
                return;

            // TODO: Сделать шаг по времени

        }

        /// <summary>
        /// Выполняет остановку симуляции на любом шаге
        /// </summary>
        public void Stop()
        {

            // Выполняем проверку на состояние симуляции
            if (!this.isStart)
                return;

            // Выполняем остановку симуляции
            this.isStart = false;

            // TODO: Выполнить остановку симуляции

        }

        public void Draw()
        {

            // Выполняем проверку на состояние симуляции
            if (!this.isStart)
                return;

            // TODO: Выполнить отрисовку симуляции

        }

        public override string ToString()
        {

            // Выполняем проверку на состояние симуляции
            if (!this.isStart)
                return "Simulation is not start";

            // Инициализируем переменную результата
            string res = "";

            // TODO: Выполнить передачу пакетов симуляции
            res = conveyor.ToString();

            // Возвращаем результат
            return res;
        }
    }
}
