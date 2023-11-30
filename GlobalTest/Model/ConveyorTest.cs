using magisterDiplom.Model;

namespace GlobalTest.Model
{
    [TestClass]
    public class ConveyorTest
    {

        [TestMethod]
        public void ConveyorTest_1Device_1buffer()
        {

            Conveyor conveyor = new Conveyor(1, 1);

            string output =
                "+------+" + Environment.NewLine +
                "|device|" + Environment.NewLine +
                "+------+" + Environment.NewLine;

            Assert.AreEqual(output, conveyor.ToString());
        }

        [TestMethod]
        public void ConveyorTest_2Device_1buffer()
        {

            Conveyor conveyor = new Conveyor(2, 1);

            string output =
                "+------+" + Environment.NewLine +
                "|device|" + Environment.NewLine +
                "+------+" + Environment.NewLine +
                "|" + Environment.NewLine +
                "+------+" + Environment.NewLine +
                "|device|" + Environment.NewLine +
                "+------+" + Environment.NewLine;

            Assert.AreEqual(output, conveyor.ToString());
        }

        [TestMethod]
        public void ConveyorTest_2Device_2buffer()
        {

            Conveyor conveyor = new Conveyor(2, 2);

            string output =
                "+------+" + Environment.NewLine +
                "|device|" + Environment.NewLine +
                "+------+" + Environment.NewLine +
                "|" + Environment.NewLine +
                "|" + Environment.NewLine +
                "+------+" + Environment.NewLine +
                "|device|" + Environment.NewLine +
                "+------+" + Environment.NewLine;

            Assert.AreEqual(output, conveyor.ToString());
        }
    }
}
