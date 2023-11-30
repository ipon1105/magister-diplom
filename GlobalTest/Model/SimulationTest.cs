using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using magisterDiplom.Model;
using newAlgorithm.Model;

namespace GlobalTest.Model
{

    [TestClass]
    public class SimulationTest
    {

        [TestMethod]
        public void SimulationTest_2Device_1Buffer_1Job()
        {

            // Описываем конвейерную систему, как 2 конвейера и длина буфера 1
            //+------+
            //|device|
            //+------+
            //|
            //+------+
            //|device|
            //+------+
            Conveyor conveyor = new Conveyor(2, 1);

            // Для одного задания описывает матрицу времени выполнения следующим образом
            // [[ 5  ]
            //  [ 10 ]]
            Matrix proccessingTime = new Matrix(new List<List<int>> {
                new List<int> { 5 },
                new List<int> { 10 }
            });

            // Для 2 конвейеров описываем следующий словарь
            // 0: [[3]]
            // 1: [[3]]
            Dictionary<int, Matrix> changeoverTime = new Dictionary<int, Matrix>();
            changeoverTime.Add(0, new Matrix(new List<List<int>> {
                new List<int> { 3 },
            }));
            changeoverTime.Add(1, new Matrix(new List<List<int>> {
                new List<int> { 3 },
            }));

            Simulation simulation = new Simulation(conveyor, changeoverTime, proccessingTime);
            simulation.Start();



        }

    }
}
