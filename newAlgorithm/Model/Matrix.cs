using System.Collections.Generic;

namespace newAlgorithm.Model
{
    public class Matrix
    {
        public List<List<int>> matrix = new List<List<int>>();

        public Matrix(List<List<int>> matrix)
        {
            this.matrix = matrix;
        }

        public int GetItem(int i, int j)
        {
            try
            {
                return matrix[i - 1][j - 1];
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}
