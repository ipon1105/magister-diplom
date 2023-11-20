using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magisterDiplom.Utils
{

    /// <summary>
    /// Данный класс используется для вспомогательных преобразований
    /// </summary>
    public class ListUtils
    {

        /// <summary>
        /// Данная статическая функция выполняет конкатинацию списка в строку
        /// </summary>
        /// <param name="list">Целочисленный список для преобразования в строку</param>
        /// <param name="separator">Раздилитель между элементами списка, по умолчанию пробел</param>
        /// <param name="prefix">Префикс в начале всей строки, по умолчанию пустая строка</param>
        /// <param name="postfix">Постфикс в конце всей строки, по умолчанию пустая строка</param>
        /// <returns>Строка со всеми элементами разделёнными separator</returns>
        public static string ListIntToString(List<int> list, string separator=" ", string prefix="", string postfix="")
        {
            string result = prefix;
            foreach (var element in list)
                result += element + separator;
            return result + postfix;
        }

        /// <summary>
        /// Данная статическая функция выполняет конкатинацию вложенного списка в строки раздлённые переводами строки
        /// </summary>
        /// <param name="listList">Вложенный список</param>
        /// <param name="separator">Разделитель между элементами крайнего списка, по умолчанию пробел</param>
        /// <param name="prefix">Префикс перед каждой строкой, по умолчанию пустая строка</param>
        /// <param name="postfix">Постфикс после каждой строки, по умолчанию пустая строка</param>
        /// <returns>Строка со всеми элементами вложенного списка разделёнными separator и переводами строк</returns>
        public static string ListListIntToString(List<List<int>> listList, string separator = " ", string prefix = "", string postfix = "")
        {
            string result = "";
            foreach (var list in listList)
                result += ListIntToString(list, separator, prefix, postfix) + Environment.NewLine;
            return result;
        }

    }
}
