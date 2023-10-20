using System.Collections.Generic;

namespace newAlgorithm.Model
{
    public class RMatrix
    {
        public List<RMatrixNode> rMatrix = new List<RMatrixNode>();
        public List<int> typePriority = new List<int>();
        public int lastPosition = 0;
        public int countType = 0;

        public RMatrix(int countType)
        {
            this.countType = countType;
        }

        /// <summary>
        /// Добавляет в матрицу R элемент (нужно для построения следующих уровней)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public void AddNode(int type, int count)
        {
            RMatrixNode newNode = new RMatrixNode(type, ++lastPosition, count);
            rMatrix.Add(newNode);
        }

        /// <summary>
        /// Clone RMatrix class
        /// </summary>
        /// <returns></returns>
        public RMatrix Clone()
        {
            RMatrix newRMatrix = new RMatrix(countType);

            List<RMatrixNode> newNodeList = new List<RMatrixNode>();

            foreach (RMatrixNode rMatrixNode in rMatrix)
            {
                newNodeList.Add(new RMatrixNode(rMatrixNode.Type, rMatrixNode.Position, rMatrixNode.Count));
            }

            newRMatrix.rMatrix = newNodeList;
            newRMatrix.typePriority = new List<int>(typePriority);
            newRMatrix.lastPosition = lastPosition;

            return newRMatrix;
        }

        /// <summary>
        /// Находит узел в матрице R в заданной позиции
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public RMatrixNode Find(int position)
        {
            foreach (RMatrixNode rMatrixNode in rMatrix)
            {
                if (rMatrixNode.Position == position)
                {
                    return rMatrixNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Возвращает последнюю позицию в матрице R
        /// </summary>
        /// <returns></returns>
        public int GetLastPosition()
        {
            return lastPosition;
        }
    }
}
