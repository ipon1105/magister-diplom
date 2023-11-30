using magisterDiplom.Model;
using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace newAlgorithm
{
    /*
    Описание использованных имён переменных:
    n = dataTypesCount - количество типов данных
    i = dataType (от 1 до dataTypesCount) - индекс (идентификатор) типа данных
    L = deviceCount - количество приборов в конвейерной системе
    l = device - индекс (идентификатор) прибора
    A' = matrixA_Prime - Матрица составов партий
    mi = matrixA[i].Count (или matrixA_Prime[i].Count) - Количество партий данных i-ого типа
    n_p = batchesForAllDataTypes = sum(mi) = sum(matrixA[i].Count) - количество партий в последовательности pi_l
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
            /// Для тестов
            List<Job> jobs = new List<Job>()
            {
                new Job(0, 0),
                new Job(1, 0),
                new Job(2, 0),
                new Job(3, 0),
            };
            int deviceCount = 3;
            int buffer = 2;
            Matrix proccessingTime = new Matrix(
                new List<List<int>>
                {
                    new List<int>{4, 3, 2, 1},
                    new List<int>{4, 3, 2, 1},
                }
            );
            WorkSchedule schedule = new WorkSchedule(
                jobs,
                deviceCount,
                buffer,
                proccessingTime
            );

            bool a = false;

            schedule.Step();
            schedule.Step();
            schedule.Step();
            schedule.Step();
            schedule.Step();

            ///
            ///Application.EnableVisualStyles();
            ///Application.SetCompatibleTextRenderingDefault(false);
            ///Application.Run(new Form1());
        }
    }
}
