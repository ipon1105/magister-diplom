namespace newAlgorithm.Model
{
    /// <summary>
    /// Данный класс описывает задание в системе, которое обладает позицией и типом.
    /// </summary>
    public class Job
    {
        
        /// <summary>
        /// Данная переменная представляет из себя тип задания в системе, который описывается целочисленным типом int
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// Данная переменная представляет из себя данные о позиции задания в системе, которые описываются целочисленным типом int и определяется, как (buffer+1)*deviceCount
        /// </summary>
        private int Position;

        /// <summary>
        /// Данная переменная представляет из себя данные о состоянии задания в системе и может принимать значения в диапазоне от -1 до n включительно.
        /// Когда State = 0b00, тогда ("Задание в буфере" или "Задание вне системы") и "Спереди занято"
        /// Когда State = 0b01, тогда "Задание в приборе" и "Спереди занято"
        /// Когда State = 0b10, тогда ("Задание в буфере" или "Задание вне системы") и "Спереди свободно"
        /// Когда State = 0b11, тогда "Задание в приборе" и "Спереди свободно"
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Время выполнения задания в системе
        /// </summary>
        private int proccessingTime;

        /// <summary>
        /// Данный конструктор принимает 3 параметра и возвращает экземпляр класса задания
        /// </summary>
        /// <param name="type">Определяет тип задания</param>
        /// <param name="position">Определяет позицию задания в системе</param>
        /// <param name="state">Определяет состояние задания, по умолчанию принимает значение -1, а следовательно не определённое состояние</param>
        public Job(int type, int position, int state = -1)
        {
            Type = type;
            Position = position;
            State = state;
        }

        /// <summary>
        /// Сбрасываем значения задания по умолчанию
        /// </summary>
        public void Reset()
        {

            Position = 0;
            State = -1;
        }

        /// <summary>
        /// Возвращаем позицию
        /// </summary>
        /// <returns>Позиция</returns>
        public int GetPosition()
        {
            return Position;
        }

        /// <summary>
        /// Выполняем установку времени выполнения текущего задания
        /// </summary>
        /// <param name="proccessingTime"></param>
        public void SetProccessingTime(int proccessingTime)
        {
            this.proccessingTime = proccessingTime;
        }

        /// <summary>
        /// Выполняем единичный шаг для одного задания
        /// </summary>
        public void Step()
        {

            // Шаг длиной 1 временной промежуток
            const int step = 1;

            // Из текущего времени выполнения 
            proccessingTime -= step;
        }

        /// <summary>
        /// Возвращаем состояние задания, как завершённое
        /// </summary>
        /// <returns>True - Значит задание выполнено, иначе False</returns>
        public bool IsFinish()
        {
            return proccessingTime <= 0;
        }

    }
}
