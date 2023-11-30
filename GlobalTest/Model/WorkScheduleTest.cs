using magisterDiplom.Model;
using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTest.Model
{

    [TestClass]
    public class WorkScheduleTest
    {

        [TestMethod]
        public void WorkScheduleTest_1()
        {
            List<Job> jobs = new List<Job>()
            {
                new Job(0, 0),
                new Job(1, 0),
                new Job(2, 0),
                new Job(3, 0),
            };
            int deviceCount = 2;
            int buffer = 2;
            Matrix proccessingTime = new Matrix(
                new List<List<int>>
                {
                    new List<int>{4, 1, 2, 1},
                    new List<int>{4, 1, 2, 1},
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
            schedule.Step();
            schedule.Step();

            Assert.AreEqual("2", schedule.ToString());
        }
    }
}