using magisterDiplom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using magisterDiplom.Fabric;
using magisterDiplom.Model;

namespace GlobalTest
{

    [TestClass]
    public class SimplePreMScheduleTest
    {

        [TestMethod]
        public void test_1()
        {

            // Создаём структуру конфигурации
            Config config = Config.GetDebugConfig_1();

            // Создаём объект класса расписания
            magisterDiplom.Fabric.SimplePreMSchedule schedule = new magisterDiplom.Fabric.SimplePreMSchedule(config);

            // Выполняем вызов функции Build
            schedule.Build(matrixA: new List<List<int>>
            {
                new List<int>{ 8, 4 },
                new List<int>{ 8, 4 },
                new List<int>{ 8, 4 },
            });
        }

    }
}
