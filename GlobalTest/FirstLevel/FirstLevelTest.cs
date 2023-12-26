using magisterDiplom;
using magisterDiplom.Model;
using newAlgorithm;
using newAlgorithm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTest.FirstLevel
{
    [TestClass]
    public class FirstLevelTest
    {

        [TestMethod]
        public void GenerateStartSolutionTest()
        {
            
            // Создаём экземпляр конфигурационной структуры
            Config config = new Config( 3, 4, 3, null, null, false );

            var firstLevel = new newAlgorithm.FirstLevel(config, new List<int> { 12, 12, 12 });
            var output = new List<List<int>> { new List<int> { 10, 2 }, new List<int> { 10, 2 }, new List<int> { 10, 2 } };
            firstLevel.GenerateStartSolution();

            for (int dataType = 0; dataType < output.Count; dataType++)
                for (int batch = 0; batch < output[dataType].Count; batch++)
                    Assert.AreEqual(output[dataType][batch], firstLevel.matrixA_Prime[dataType][batch]);
        }

        [TestMethod]
        public void GenerateFixedBatchesSolutionTest()
        {

            Config config = new Config(3, 4, 3, null, null, true);
            var firstLevel = new newAlgorithm.FirstLevel(config, new List<int> { 12, 12, 12 });
            var output = new List<List<int>> { new List<int> { 12 }, new List<int> { 12 }, new List<int> { 12 } };
            firstLevel.GenerateFixedBatchesSolution();

            for (int dataType = 0; dataType < output.Count; dataType++)
                for (int batch = 0; batch < output[dataType].Count; batch++)
                    Assert.AreEqual(output[dataType][batch], firstLevel.matrixA_Prime[dataType][batch]);
        }

        /*
        [TestMethod]
        public void NewDataTest()
        {
            var firstLevel = new newAlgorithm.FirstLevel(2, new List<int> { 12, 12, 12 }, false);
            var input = new List<List<int>> { new List<int> { 10, 2 }, new List<int> { 10, 2 } };
            var output = new List<List<int>> { new List<int> { 9, 3 }, new List<int> { 10, 2 } };
            
            firstLevel._a1 = input;
            var result = firstLevel.NewData(0);

            for (int dataType = 0; dataType < output.Count; dataType++)
                for (int batch = 0; batch < output[dataType].Count; batch++)
                    Assert.AreEqual(output[dataType][batch], result[dataType][batch]);

        }
        */
    }
}
