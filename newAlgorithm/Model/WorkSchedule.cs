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
        /// Список заданий для симуляции расписания
        /// </summary>
        private List<Job> _jobs;

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
        /// Матрица времён выполнения типов заданий на приборах
        /// </summary>
        private Matrix proccessingTime;

        /// <summary>
        /// Конструктор принимает список заданий для будущей симуляции и возвращает экземпляр данного класса
        /// </summary>
        public WorkSchedule(List<Job> jobs, int deviceCount, int buffer, Matrix proccessingTime)
        {

            // Инициализируем переменные
            this.jobs = jobs;
            this.buffer = buffer;
            this.deviceCount = deviceCount;
            this.proccessingTime = proccessingTime;

            // Сбрасываем значения
            Reset();
        }

        /// <summary>
        /// Данная функция сбрасывает список заданий до значений по умолчанию
        /// </summary>
        public void Reset()
        {

            // Копируем список заданий во временный список
            _jobs = new List<Job>(jobs);

            // Для каждого задания сбрасываем значения заданий
            foreach (Job job in jobs)
                job.Reset();

            // Отчищаем существующее расписание, если оно есть
            if (this.schedule != null)
                this.schedule.Clear();

            // Инициализируем расписание
            this.schedule = new List<Job>(deviceCount + (deviceCount - 1) * buffer);

            // Заполняем расписание пустотой
            for (int i = 0; i < deviceCount + (deviceCount - 1) * buffer; i++)
                this.schedule.Add(null);
        }

        /// <summary>
        /// Обновление выполняется как оптимизация расписания
        /// </summary>
        public void Update()
        {

            // Отвечаем за обновление
            bool isUpdating = true;

            // До тех пор пока расписание не составлено выполняем обновление
            while(isUpdating)
            {

                // Вызываем шаг обновления
                UpdateStep(out isUpdating);
            }
        }

        /// <summary>
        /// Данная функция выполняет шаг оптимизации расписания
        /// </summary>
        /// <param name="isUpdating">Состояния обновления, установит True, если требуется выполнить ещё шаг, иначе False</param>
        public void UpdateStep(out bool isUpdating)
        {

            // Устанавливаем состояния обновления, как False
            isUpdating = false;

            // Для каждой позиции выполняем обработку
            for (int position = 0; position < schedule.Count; position++)
            {

                // Если по расписанию нет задания, выполняем обработку
                if (schedule[position] == null)
                {

                    // Позиция в расписании первая и задания есть на обработке
                    if (position == 0 && _jobs.Count != 0)
                    {

                        // Достаём задание из списка заданий
                        Job job = _jobs[0];
                        _jobs.RemoveAt(0);

                        // Добавляем задание в расписание
                        schedule[position] = job;
                        schedule[position].SetProccessingTime(proccessingTime[0, job.Type]);
                        isUpdating = true;
                    }

                    // Пропускаем обработку
                    continue;
                }

                // Если по расписанию задание в приборе
                if (position % (buffer + 1) == 0)
                {

                    // Если задание завершено
                    if (schedule[position].IsFinish())
                    {

                        // Если прибор последний
                        if (position + 1 >= schedule.Count)
                        {

                            // Выводим задание из системы
                            schedule[position] = null;
                            isUpdating = true;

                            continue;
                        }

                        // Если спереди свободно
                        if (schedule[position + 1] == null)
                        {

                            // Выполняем смещение задания вперёд
                            schedule[position + 1] = (Job)schedule[position].Clone();
                            schedule[position] = null;

                            int newDevice = Convert.ToInt32(Math.Ceiling((double)(position + 1) / (buffer + 1)));
                            schedule[position + 1].SetProccessingTime(proccessingTime[newDevice, schedule[position + 1].Type]);

                            isUpdating = true;
                        }
                    }

                    // Пропускаем обработку
                    continue;
                }

                // Если по расписанию задание в буфере
                else
                {

                    // Если спереди свободно
                    if (schedule[position + 1] == null)
                    {

                        // Выполняем перестановку
                        schedule[position + 1] = (Job)schedule[position].Clone();
                        schedule[position] = null;
                    }

                    continue;
                }
            }
        }

        /// <summary>
        /// Выполняем шаг расписания
        /// </summary>
        public void Step()
        {

            // Перед каждым шагом выполняем оптимизацию
            Update();

            // Выполняем шаг для всех заданий в системе, которые находятся на приборах
            for (int device = 0; device < schedule.Count; device += (buffer + 1))
                if (schedule[device] != null)
                    schedule[device].Step();
        }

        public override string ToString()
        {
            string res = "";

            foreach (Job j in schedule)
                res += "|" + ((j == null) ? "-" : $"+{j}" ) + Environment.NewLine;

            return res;
        }
    }
}
