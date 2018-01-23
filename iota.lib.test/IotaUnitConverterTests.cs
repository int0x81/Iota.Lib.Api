using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests
{
    [TestClass]
    public class IotaUnitConverterTest
    {
        [TestMethod]
        public void shouldConvertUnitItoKi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Iota, IotaUnits.Kilo), 1);
        }

        [TestMethod]
        public void shouldConvertUnitKiToMi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Kilo, IotaUnits.Mega), 1);
        }

        [TestMethod]
        public void shouldConvertUnitMiToGi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Mega, IotaUnits.Giga), 1);
        }

        [TestMethod]
        public void shouldConvertUnitGiToTi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Giga, IotaUnits.Terra), 1);
        }

        [TestMethod]
        public void shouldConvertUnitTiToPi()
        {
            Assert.AreEqual(IotaUnitConverter.ConvertUnits(1000, IotaUnits.Terra, IotaUnits.Peta), 1);
        }

        [TestMethod]
        public void shouldFindOptimizeUnitToDisplay()
        {
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1), IotaUnits.Iota);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000), IotaUnits.Kilo);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000), IotaUnits.Mega);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000), IotaUnits.Giga);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000L), IotaUnits.Terra);
            Assert.AreEqual(IotaUnitConverter.FindOptimalIotaUnitToDisplay(1000000000000000L), IotaUnits.Peta);
        }
    }
}