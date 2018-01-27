using System;
using System.Linq;
using Iota.Lib.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.Test
{
    [TestClass]
    public class ASCIIConverterTest
    {
        [TestMethod]
        public void TestConvertASCIIStringToTrytes()
        {
            Assert.AreEqual("IC", ASCIIConverter.ToTrytes("Z"));
            Assert.AreEqual(ASCIIConverter.ToTrytes("JOTA JOTA"), "TBYBCCKBEATBYBCCKB");
        }

        [TestMethod]
        public void TestConvertTrytesToASCIIString()
        {
            Assert.AreEqual("Z", ASCIIConverter.ToString("IC"));
            Assert.AreEqual(ASCIIConverter.ToString("TBYBCCKBEATBYBCCKB"), "JOTA JOTA");
        }
    }
}