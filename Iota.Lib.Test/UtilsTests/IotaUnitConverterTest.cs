using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.Test
{
    [TestClass]
    public class IotaUnitConverterTest
    {
        [TestMethod]
        public void TestConvertUnitItoKi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnit.Iota, IotaUnit.Kilo), 1);
        }

        [TestMethod]
        public void TestConvertUnitKiToMi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnit.Kilo, IotaUnit.Mega), 1);
        }

        [TestMethod]
        public void TestConvertUnitMiToGi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnit.Mega, IotaUnit.Giga), 1);
        }

        [TestMethod]
        public void TestConvertUnitGiToTi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnit.Giga, IotaUnit.Terra), 1);
        }

        [TestMethod]
        public void TestConvertUnitTiToPi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnit.Terra, IotaUnit.Peta), 1);
        }

        [TestMethod]
        public void TestFindOptimalUnitToDisplay()
        {
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1), IotaUnit.Iota);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000), IotaUnit.Kilo);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000), IotaUnit.Mega);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000), IotaUnit.Giga);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000L), IotaUnit.Terra);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000000L), IotaUnit.Peta);
        }
    }
}