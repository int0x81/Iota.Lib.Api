using System;
using System.Linq;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api.Utils
{
    [TestClass]
    public class TrytesConverterTest
    {
        private static Random random = new Random();

        [TestMethod]
        public void ShouldConvertStringToTrytes()
        {
            Assert.AreEqual("IC", TrytesConverter.ToTrytes("Z"));
            Assert.AreEqual(TrytesConverter.ToTrytes("JOTA JOTA"), "TBYBCCKBEATBYBCCKB");
        }

        [TestMethod]
        public void ShouldConvertTrytesToString()
        {
            Assert.AreEqual("Z", TrytesConverter.ToString("IC"));
            Assert.AreEqual(TrytesConverter.ToString("TBYBCCKBEATBYBCCKB"), "JOTA JOTA");
        }

        public void ShouldConvertBackAndForth()
        {
            string str = RandomString(1000);
            string back = TrytesConverter.ToString(TrytesConverter.ToTrytes(str));
            Assert.AreEqual(str, back);
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}