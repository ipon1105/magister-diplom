using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newAlgorithm
{
    // TODO: Выполнить проверку корректного выполнения программы в сравнение со старым вариантом на разных входных данных
    //       Приборов: 5; Типов: 4; Каждого типа: 12; Множитель: 8;
    //       Приборов: 5; Типов: 3; Каждого типа: 12; Множитель: 8;
    //       Приборов: 4; Типов: 4; Каждого типа: 12; Множитель: 8;
    //       Приборов: 4; Типов: 3; Каждого типа: 12; Множитель: 8;
    //       Приборов: 4; Типов: 4; Каждого типа: 12; Множитель: 6;
    //       Приборов: 4; Типов: 3; Каждого типа: 12; Множитель: 6;
    //       Приборов: 4; Типов: 3; Каждого типа: 14; Множитель: 8;
    //       Приборов: 4; Типов: 3; Каждого типа: 14; Множитель: 8;
    //       Приборов: 4; Типов: 3; Каждого типа: 12; Множитель: 16;
    //       Приборов: 4; Типов: 3; Каждого типа: 12; Множитель: 16;
    // TODO: Узнать что за число в начале файла outputFirstAlgorithm.txt? Лучшее или Фиксированное?
    /*
    Описание использованных имён переменных:
    n = dataTypesCount - количество типов данных
    i = dataType (от 1 до dataTypesCount) - индекс (идентификатор) типа данных
    L = deviceCount - количество приборов в конвейерной системе
    l = device - индекс (идентификатор) прибора
    A' = matrixA_Prime - Матрица составов партий
    mi = matrixA[i].Count (или matrixA_Prime[i].Count) - Количество партий данных i-ого типа
    n_p = maxBatchesPositions = sum(mi) = sum(matrixA[i].Count) - максимальное количество партий в последовательности pi_l
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
