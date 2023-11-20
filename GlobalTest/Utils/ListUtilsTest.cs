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
        public void ListIntToStringTest_default()
        {
            List<int> input = new List<int> { 1, 2 };
            string output = "1 2 ";
            Assert.AreEqual(ListUtils.ListIntToString(input), output);
        }

        [TestMethod]
        public void ListIntToStringTest_separator()
        {
            List<int> input = new List<int> { 1, 2 };
            string output = "155255";
            Assert.AreEqual(ListUtils.ListIntToString(input, "55"), output);
        }

        [TestMethod]
        public void ListIntToStringTest_prefix()
        {
            List<int> input = new List<int> { 1, 2 };
            string output = "\t1 2 ";
            Assert.AreEqual(ListUtils.ListIntToString(input, " ", "\t"), output);
        }

        [TestMethod]
        public void ListIntToStringTest_postfix()
        {
            List<int> input = new List<int> { 1, 2 };
            string output = "1 2 \t";
            Assert.AreEqual(ListUtils.ListIntToString(input, " ", "", "\t"), output);
        }

        [TestMethod]
        public void ListListIntToStringTest_default()
        {
            List<List<int>> input = new List<List<int>> {new List<int> { 1, 2 }, new List<int> { 3, 4 }};
            string output = $"1 2 {Environment.NewLine}3 4 {Environment.NewLine}";
            Assert.AreEqual(ListUtils.ListListIntToString(input), output);
        }

        [TestMethod]
        public void ListListIntToStringTest_separator()
        {
            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"15582558{Environment.NewLine}35584558{Environment.NewLine}";
            Assert.AreEqual(ListUtils.ListListIntToString(input, "558"), output);
        }

        [TestMethod]
        public void ListListIntToStringTest_prefix()
        {
            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"\t1 2 {Environment.NewLine}\t3 4 {Environment.NewLine}";
            Assert.AreEqual(ListUtils.ListListIntToString(input, " ", "\t"), output);
        }

        [TestMethod]
        public void ListListIntToStringTest_postfix()
        {
            List<List<int>> input = new List<List<int>> { new List<int> { 1, 2 }, new List<int> { 3, 4 } };
            string output = $"\t1 2 \t{Environment.NewLine}\t3 4 \t{Environment.NewLine}";
            Assert.AreEqual(ListUtils.ListListIntToString(input, " ", "\t", "\t"), output);
        }

    }

}
