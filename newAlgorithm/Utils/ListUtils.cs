using System;
using System.Collections.Generic;
using System.Linq;

namespace magisterDiplom.Utils
{

    /// <summary>
    /// Данный класс используется для вспомогательных преобразований
    /// </summary>
    public class ListUtils
    {

        /// <summary>
        /// Данная статическая функция выполняет конкатинацию вектора в строку
        /// </summary>
        /// <param name="vector">Целочисленный список (вектор) для преобразования в строку</param>
        /// <param name="separator">Раздилитель между элементами списка, по умолчанию пробел</param>
        /// <param name="prefix">Префикс в начале всей строки, по умолчанию пустая строка</param>
        /// <param name="postfix">Постфикс в конце всей строки, по умолчанию пустая строка</param>
        /// <returns>Строка со всеми элементами разделёнными separator</returns>
        public static string VectorIntToString(List<int> vector, string separator=" ", string prefix="", string postfix="")
        {
            string result = prefix;
            int index = 1;
            foreach (var element in vector)
                result += element + ((index++ == vector.Count) ? "" : separator);
            return result + postfix;
        }

        /// <summary>
        /// Данная статическая функция выполняет конкатинацию матрицы в строки раздлённые переводами строки
        /// </summary>
        /// <param name="matrix">Вложенный список - матрица</param>
        /// <param name="separator">Разделитель между элементами крайнего списка, по умолчанию пробел</param>
        /// <param name="prefix">Префикс перед каждой строкой, по умолчанию пустая строка</param>
        /// <param name="postfix">Постфикс после каждой строки, по умолчанию пустая строка</param>
        /// <returns>Строка со всеми элементами вложенного списка разделёнными separator и переводами строк</returns>
        public static string MatrixIntToString(List<List<int>> matrix, string separator = " ", string prefix = "", string postfix = "")
        {
            string result = "";
            foreach (var list in matrix)
                result += VectorIntToString(list, separator, prefix, postfix) + Environment.NewLine;
            return result;
        }

        /// <summary>
        /// Данная статическая фукнция выполняет глубокое копирования целочисленного вектора
        /// </summary>
        /// <param name="vector">Вектор который необходимо скопировать в новый вектор</param>
        /// <returns>Скопированный вектор</returns>
        public static List<int> VectorDeepCopy(List<int> vector)
        {
            return vector.ToList();
        }

        /// <summary>
        /// Данная статическая фукнция выполняет глубокое копирования целочисленной матрицы
        /// </summary>
        /// <param name="matrix">Матрица которую необходимо скопировать в новую</param>
        /// <returns>Скопированная матрица</returns>
        public static List<List<int>> MatrixDeepCopy(List<List<int>> matrix)
        {
            return matrix.Select(VectorDeepCopy).ToList();
        }

    }
}
