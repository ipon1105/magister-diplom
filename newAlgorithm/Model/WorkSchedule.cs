using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magisterDiplom.Model
{
    
    /// <summary>
    /// Данный класс описывает расписание и процесс его симулирования
    /// </summary>
    public class WorkSchedule
    {

        /// <summary>
        /// Список заданий для симуляции расписания
        /// </summary>
        private List<Job> jobs;

        /// <summary>
        /// Список заданий
        /// </summary>
        private List<Job> schedule;

        /// <summary>
        /// Количество приборов
        /// </summary>
        private int deviceCount;

        /// <summary>
        /// Размер буфера перед приборами
        /// </summary>
        private int buffer;

        /// <summary>
        /// Конструктор принимает список заданий для будущей симуляции и возвращает экземпляр данного класса
        /// </summary>
        public WorkSchedule(List<Job> jobs, int deviceCount, int buffer)
        {

            // Инициализируем переменные
            this.jobs = jobs;
            this.buffer = buffer;
            this.deviceCount = deviceCount;

            // Сбрасываем значения
            Reset();
        }

        /// <summary>
        /// Данная функция сбрасывает список заданий до значений по умолчанию
        /// </summary>
        public void Reset()
        {

            // Для каждого задания сбрасываем значения заданий
            foreach (Job job in jobs)
                job.Reset();

            // Отчищаем существующее расписание, если оно есть
            if (this.schedule != null)
                this.schedule.Clear();

            // Инициализируем расписание
            this.schedule = new List<Job>((buffer + 1) * deviceCount);

            // Заполняем расписание пустотой
            for (int i = 0; i < (buffer + 1) * deviceCount; i++)
                this.schedule.Add(null);
        }

        /// <summary>
        /// Обновление выполняется как оптимизация расписания
        /// </summary>
        public void Update()
        {
            // TODO:
        }

        /// <summary>
        /// Выполняем шаг расписания
        /// </summary>
        public void Step()
        {

            // Перед каждым шагом выполняем оптимизацию
            Update();

            // Выполняем шаг для всех заданий в системе
            // TODO:
        }
    }
}
