using magisterDiplom.Utils;
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
            Assert.AreEqual(ListUtils.VectorIntToString(input), output);
        }

        [TestMethod]
        public void VectorIntToStringTest_separator()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "1552";
            Assert.AreEqual(ListUtils.VectorIntToString(input, "55"), output);
        }

        [TestMethod]
        public void VectorIntToStringTest_prefix()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "\t1 2";
            Assert.AreEqual(ListUtils.VectorIntToString(input, " ", "\t"), output);
        }

        [TestMethod]
        public void VectorIntToStringTest_postfix()
        {

            List<int> input = new List<int> { 1, 2 };
            string output = "1 2\t";
            Assert.AreEqual(ListUtils.VectorIntToString(input, " ", "", "\t"), output);
        }

        [TestMethod]
        public void MatrixIntToStringTest_default()
        {

            List<List<int>> input = new List<List<int>> {new List<int> { 1, 2 }, new List<int> { 3, 4 }};
            string output = $"1 2{Environment.NewLine}3 4{Environment.NewLine}";
            Assert.AreEqual(ListUtils.MatrixIntToString(input), output);
        }

        [TestMethod]
        public void MatrixIntToStringTest_separator()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"15582{Environment.NewLine}35584{Environment.NewLine}";
            Assert.AreEqual(ListUtils.MatrixIntToString(input, "558"), output);
        }

        [TestMethod]
        public void MatrixIntToStringTest_prefix()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"\t1 2{Environment.NewLine}\t3 4{Environment.NewLine}";
            Assert.AreEqual(ListUtils.MatrixIntToString(input, " ", "\t"), output);
        }

        [TestMethod]
        public void MatrixIntToStringTest_postfix()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"\t1 2\t{Environment.NewLine}\t3 4\t{Environment.NewLine}";
            Assert.AreEqual(ListUtils.MatrixIntToString(input, " ", "\t", "\t"), output);
        }

        [TestMethod]
        public void VectorDeepCopy()
        {

            List<int> input = new List<int> { 1, 2 };
            List<int> output = ListUtils.VectorDeepCopy(input);

            // Проверяем результат копирования
            Assert.AreEqual(input[0], output[0]);
            Assert.AreEqual(input[1], output[1]);

            // Изменяем исходный вектор и выполняем проверку
            input[0] = 0;
            Assert.AreNotEqual(input[0], output[0]);
            Assert.AreEqual(input[1], output[1]);
        }

        [TestMethod]
        public void MatrixDeepCopy()
        {

            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            List<List<int>> output = ListUtils.MatrixDeepCopy(input);

            // Проверяем результат копирования
            Assert.AreEqual(input[0][0], output[0][0]);
            Assert.AreEqual(input[0][1], output[0][1]);
            Assert.AreEqual(input[1][0], output[1][0]);
            Assert.AreEqual(input[1][1], output[1][1]);

            // Изменяем исходный вектор и выполняем проверку
            input[0][0] = 0;
            input[1][1] = 0;
            Assert.AreNotEqual(input[0][0], output[0][0]);
            Assert.AreEqual(input[0][1], output[0][1]);
            Assert.AreEqual(input[1][0], output[1][0]);
            Assert.AreNotEqual(input[1][1], output[1][1]);
        }

    }
}
