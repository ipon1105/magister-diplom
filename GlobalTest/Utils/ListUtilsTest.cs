using magisterDiplom.Utils;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using newAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalTest.Utils
{
    /// <summary>
    /// Данный класс описывает тесты для модуля ListUtils
    /// </summary>
    [TestClass]
    public class ListUtilsTest
    {

        [TestMethod]
        public void VectorIntToStringTest_default()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "1 2";
            Assert.AreEqual(output, ListUtils.VectorIntToString(input));
        }

        [TestMethod]
        public void VectorIntToStringTest_separator()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "1552";
            Assert.AreEqual(output, ListUtils.VectorIntToString(input, "55"));
        }

        [TestMethod]
        public void VectorIntToStringTest_prefix()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "\t1 2";
            Assert.AreEqual(output, ListUtils.VectorIntToString(input, " ", "\t"));
        }

        [TestMethod]
        public void VectorIntToStringTest_postfix()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "1 2\t";
            Assert.AreEqual(output, ListUtils.VectorIntToString(input, " ", "", "\t"));
        }

        [TestMethod]
        public void VectorIntToStringTest_equalsValuesDeafault()
        {
            List<int> input = new List<int> { 6, 6 };
            string output = "6 6";
            Assert.AreEqual(output, ListUtils.VectorIntToString(input, " ", "", ""));
        }

        [TestMethod]
        public void MatrixIntToStringTest_default()
        {

            List<List<int>> input = new List<List<int>> {new List<int> { 1, 2 }, new List<int> { 3, 4 }};
            string output = $"1 2{Environment.NewLine}3 4{Environment.NewLine}";
            Assert.AreEqual(output, ListUtils.MatrixIntToString(input));
        }

        [TestMethod]
        public void MatrixIntToStringTest_separator()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"15582{Environment.NewLine}35584{Environment.NewLine}";
            Assert.AreEqual(output, ListUtils.MatrixIntToString(input, "558"));
        }

        [TestMethod]
        public void MatrixIntToStringTest_prefix()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"\t1 2{Environment.NewLine}\t3 4{Environment.NewLine}";
            Assert.AreEqual(output, ListUtils.MatrixIntToString(input, " ", "\t"));
        }

        [TestMethod]
        public void MatrixIntToStringTest_postfix()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"\t1 2\t{Environment.NewLine}\t3 4\t{Environment.NewLine}";
            Assert.AreEqual(output, ListUtils.MatrixIntToString(input, " ", "\t", "\t"));
        }

        [TestMethod]
        public void VectorIntDeepCopy()
        {

            List<int> input = new List<int> { 1, 2 };
            List<int> output = ListUtils.VectorIntDeepCopy(input);

            // Проверяем результат копирования
            Assert.AreEqual(output[0], input[0]);
            Assert.AreEqual(output[1], input[1]);

            // Изменяем исходный вектор и выполняем проверку
            input[0] = 0;
            Assert.AreNotEqual(output[0], input[0]);
            Assert.AreEqual(output[1], input[1]);
        }

        [TestMethod]
        public void MatrixIntDeepCopy()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            List<List<int>> output = ListUtils.MatrixIntDeepCopy(input);

            // Проверяем результат копирования
            Assert.AreEqual(output[0][0], input[0][0]);
            Assert.AreEqual(output[0][1], input[0][1]);
            Assert.AreEqual(output[1][0], input[1][0]);
            Assert.AreEqual(output[1][1], input[1][1]);

            // Изменяем исходный вектор и выполняем проверку
            input[0][0] = 0;
            input[1][1] = 0;
            Assert.AreNotEqual(output[0][0], input[0][0]);
            Assert.AreEqual(output[0][1], input[0][1]);
            Assert.AreEqual(output[1][0], input[1][0]);
            Assert.AreNotEqual(output[1][1], input[1][1]);
        }

        [TestMethod]
        public void MatrixIntRowSwapTest()
        {

            var output = new List<List<int>> { new List<int> { 3, 4 }, new List<int> { 1, 2 }, new List<int> { 5, 6 } };
            var input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 5, 6 } };
            ListUtils.MatrixIntRowSwap(input, 0, 1);

            for (int i = 0; i < output.Count; i++)
                for (int j = 0; j < output[i].Count; j++)
                    Assert.AreEqual(output[i][j], input[i][j]);

        }

        [TestMethod]
        public void MatrixIntRowSwapTest_Panic()
        {

            var output = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 5, 6 } };
            var input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 }, new List<int> { 5, 6 } };
            ListUtils.MatrixIntRowSwap(input, 0, 5);

            for (int i = 0; i < output.Count; i++)
                for (int j = 0; j < output[i].Count; j++)
                    Assert.AreEqual(output[i][j], input[i][j]);

        }

        [TestMethod]
        public void MatrixIntColumnSwapTest()
        {

            var output = new List<List<int>> {
                new List<int> { 2, 1 },
                new List<int> { 4, 3 },
                new List<int> { 6, 5 }
            };
            var input = new List<List<int>> {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 }
            };
            ListUtils.MatrixIntColumnSwap(input, 0, 1);

            for (int i = 0; i < output.Count; i++)
                for (int j = 0; j < output[i].Count; j++)
                    Assert.AreEqual(output[i][j], input[i][j]);

        }

        [TestMethod]
        public void MatrixIntColumnSwapTest_Panic()
        {

            var output = new List<List<int>> {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 }
            };
            var input = new List<List<int>> {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 }
            };
            ListUtils.MatrixIntColumnSwap(input, 0, 5);

            for (int i = 0; i < output.Count; i++)
                for (int j = 0; j < output[i].Count; j++)
                    Assert.AreEqual(output[i][j], input[i][j]);

        }

        [TestMethod]
        public void IsMatrixIntEqualTest_1()
        {

            var output = new List<List<int>> {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 }
            };
            var input = new List<List<int>> {
                new List<int> { 1, 2 },
                new List<int> { 3, 4 },
                new List<int> { 5, 6 }
            };

            Assert.IsTrue(ListUtils.IsMatrixIntEqual(output, input));
        }

        [TestMethod]
        public void IsMatrixIntEqualTest_2()
        {

            var output = new List<List<int>> {
                new List<int> { 2, 2 },
                new List<int> { 2, 2 },
                new List<int> { 2, 2 }
            };
            var input = new List<List<int>> {
                new List<int> { 3, 3 },
                new List<int> { 3, 3 },
                new List<int> { 3, 3 }
            };

            Assert.IsFalse(ListUtils.IsMatrixIntEqual(output, input));
        }

        [TestMethod]
        public void IsMatrixIntEqualTest_3()
        {

            var output = new List<List<int>> {
                new List<int> { 2, 2 },
                new List<int> { 2, 2, 4 },
                new List<int> { 2, 2 }
            };
            var input = new List<List<int>> {
                new List<int> { 3, 3 },
                new List<int> { 3, 3 },
                new List<int> { 3, 3 }
            };

            Assert.IsFalse(ListUtils.IsMatrixIntEqual(output, input));
        }
    }

}
