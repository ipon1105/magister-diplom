using System.Collections.Generic;

namespace newAlgorithm.Model
{
    /// <summary>
    /// Данный класс описывает матрицу количества данных i-ых типов в j-ых партиях
    /// </summary>
    public class RMatrix
    {

        #region Поля данных

        /// <summary>
        /// Данная переменная содержит в себе список узлов матрицы R
        /// </summary>
        public List<RMatrixNode> rMatrix;

        /// <summary>
        /// Данная переменная содержит в себе приоритеты типов данных
        /// </summary>
        public List<int> typePriority;

        /// <summary>
        /// Данная пермення содержит количество типов данных
        /// </summary>
        public int dataTypesCount;

        #endregion

        /// <summary>
        /// Данный конструктор определяет матрицу количества данных определённых типов в определённых партий. Принимает количество типов и возвращает экземпляр данного класса
        /// </summary>
        /// <param name="dataTypesCount">Количетсво типов данных матрицы R</param>
        public RMatrix(int dataTypesCount)
        {
            rMatrix = new List<RMatrixNode>();
            typePriority = new List<int>();
            this.dataTypesCount = dataTypesCount;
        }

        /// <summary>
        /// Данная функция добавляет в матрицу R элемент необходимый для увеличения матрицы
        /// </summary>
        /// <param name="dataType">Тип данных нового элемента</param>
        /// <param name="count">Количество данных в новом элементе</param>
        public void AddNode(int dataType, int count)
        {
            RMatrixNode newNode = new RMatrixNode(dataType, count);
            rMatrix.Add(newNode);
        }

        /// <summary>
        /// Данное переопределение оператора индексирования позволяет получить элементы по индексу
        /// </summary>
        /// <param name="position">Индекс позиции элемента матрицы RMatrix</param>
        /// <returns>Целочисленное значение по позиции position</returns>
        public RMatrixNode this[int position]
        {
            // Переопределяем возвращение данных из матрицы R
            get
            {
                // Вовзращаем null в случае отсутствия узла в списке
                return rMatrix[position];
            }
        }
    }
}
