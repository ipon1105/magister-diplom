using magisterDiplom.Model;

namespace GlobalTest.Model
{
    [TestClass]
    public class ConveyorTest
    {

        [TestMethod]
        public void ConveyorTest_1()
        {

            Conveyor conveyor = new Conveyor(1, 1);

            string output =
                "|" + Environment.NewLine +
                "+------+" + Environment.NewLine +
                "|device|" + Environment.NewLine +
                "+------+" + Environment.NewLine;

            Assert.AreEqual(output, conveyor.ToString());
        }
    }
}
