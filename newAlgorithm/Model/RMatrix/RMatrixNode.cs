namespace newAlgorithm.Model
{
    
    /// <summary>
    /// Определяет множество узлов матрицы R и определяется как количетсво данных типа Type в партии Position
    /// </summary>
    public class RMatrixNode
    {

        /// <summary>
        /// Данная переменная представляет из себя тип задания данного узла
        /// </summary>
        public int Type { get; }

        /// <summary>
        /// Данная переменная представляет из себя позицию в матрице данного узла
        /// </summary>
        public int Position { get; }

        /// <summary>
        ///  Данная переменная содержит в себе количество пакетов в текущей позиции текущего типа данного узла
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Данный конструктор принимает 3 параметра и возвращает экземпляр класса узла матрицы R
        /// </summary>
        /// <param name="type">Определяет тип задания, данного узла</param>
        /// <param name="position">Определяет позицию задания, данного узла</param>
        /// <param name="count">Определяет количество данных в данном узле</param>
        public RMatrixNode(int type, int position, int count)
        {
            Type = type;
            Position = position;
            Count = count;
        }
    }
}
