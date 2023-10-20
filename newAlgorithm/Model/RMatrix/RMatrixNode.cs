namespace newAlgorithm.Model
{
    public class RMatrixNode
    {
        /// <summary>
        /// Тип заданий
        /// </summary>
        public int Type { get; } = 0;
        /// <summary>
        /// Позиция в матрице
        /// </summary>
        public int Position { get; } = 0;
        /// <summary>
        /// Количество пакетов в текущей позиции
        /// </summary>
        public int Count { get; } = 0;

        public RMatrixNode(int type, int position, int count)
        {
            Type = type;
            Position = position;
            Count = count;
        }
    }
}
