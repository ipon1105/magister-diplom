using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newAlgorithm
{
    /*
    Описание использованных имён переменных:
    n_p = batchCount - количество партий
    n = dataTypesCount - количество типов данных
    i = dataType (от 1 до dataTypesCount) - индекс (идентификатор) типа данных
    L = deviceCount - количество приборов в конвейерной системе
    l = device - индекс (идентификатор) прибора
    A' = matrixA_Prime - Матрица составов партий
    mi = matrixA[i].Count (или matrixA_Prime[i].Count) - Количество партий данных i-ого типа
    h  = batchIndex = matrixA[i][0..batchCount-1] (или matrixA_Prime[i][0..batchCount-1]) - h (0..batchCount-1) Индекс количества партий
     */
    static class Program
    {
        /// <summary>
        /// Точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
