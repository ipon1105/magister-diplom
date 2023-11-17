using System.Collections.Generic;

namespace newAlgorithm.Model
{
    /// <summary>
    /// Данный класс описывает матрицу количества данных i-ых типов в j-ых партиях
    /// </summary>
    public class RMatrix
    {

        /// <summary>
        /// Данная переменная содержит в себе список узлов матрицы R
        /// </summary>
        public List<RMatrixNode> rMatrix;

        /// <summary>
        /// Данная переменная содержит в себе приоритеты типов данных
        /// </summary>
        public List<int> typePriority;

        /// <summary>
        /// Данная переменная содержит индекс последнего элемента
        /// </summary>
        public int lastPosition;

        /// <summary>
        /// Данная пермення содержит количество типов данных
        /// </summary>
        public int countType;

        /// <summary>
        /// Данный конструктор определяет матрицу количества данных определённых типов в определённых партий. Принимает количество типов и возвращает экземпляр данного класса
        /// </summary>
        /// <param name="countType">Количетсво типов данных матрицы R</param>
        public RMatrix(int countType)
        {
            rMatrix = new List<RMatrixNode>();
            typePriority = new List<int>();
            lastPosition = 0;
            this.countType = countType;
        }

        /// <summary>
        /// Данная функция добавляет в матрицу R элемент необходимый для увеличения матрицы
        /// </summary>
        /// <param name="type">Тип данных нового элемента</param>
        /// <param name="count">Количество данных в новом элементе</param>
        public void AddNode(int type, int count)
        {
            RMatrixNode newNode = new RMatrixNode(type, ++lastPosition, count);
            rMatrix.Add(newNode);
        }

        /// <summary>
        /// Данная функция выполняет глубокое копирование данного класса
        /// </summary>
        /// <returns>Склонированный экземпляр класса RMatrix</returns>
        public RMatrix Clone()
        {
            // Определяем новую матрицу R
            RMatrix newRMatrix = new RMatrix(countType);

            // Определяем новый список узлов
            List<RMatrixNode> newNodeList = new List<RMatrixNode>();

            // Выполяем глубокое копирования каждого элемента матрица
            foreach (RMatrixNode rMatrixNode in rMatrix)
                newNodeList.Add(new RMatrixNode(rMatrixNode.Type, rMatrixNode.Position, rMatrixNode.Count));

            // Выполняем необходимое переопределение данных для нового класса
            newRMatrix.rMatrix = newNodeList;
            newRMatrix.typePriority = new List<int>(typePriority);
            newRMatrix.lastPosition = lastPosition;

            // Возвращаем новый экземпляр класса
            return newRMatrix;
        }

        /// <summary>
        /// Данная функция находит узел в матрице R в заданной позиции
        /// </summary>
        /// <param name="position">Позиция узла по которой необходимо выполнить поиск</param>
        /// <returns>Узел из матрица R</returns>
        public RMatrixNode Find(int position)
        {
            // Выполяем поиск узла по позиции
            foreach (RMatrixNode rMatrixNode in rMatrix)
                if (rMatrixNode.Position == position)
                    return rMatrixNode;

            // Вовзращаем null в случае отсутствия узла в списке
            return null;
        }

        /// <summary>
        /// Данная функция возвращает последнюю позицию в матрице R
        /// </summary>
        /// <returns>Индкс последней позиции в матрице R</returns>
        public int GetLastPosition()
        {
            return lastPosition;
        }
    }
}
