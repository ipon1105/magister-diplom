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
        public void MyTest()
        {

            // Создаём структуру конфигурации
            Config config = Config.GetDebugConfig_2();

            // Создаём объект класса расписания
            SimplePreMSchedule schedule = new (config);

            // Выполняем вызов функции Build
            schedule.Build(matrixA: new List<List<int>> {
                new () { 4, 4, 2 },
                new () { 4, 4, 2 },
                new () { 4, 4, 2 },
            });
        }

    }
}
