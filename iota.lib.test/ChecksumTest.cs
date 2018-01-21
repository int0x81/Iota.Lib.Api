using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api.Utils
{
    [TestClass]
    public class ChecksumTest
    {
        private static string TEST_ADDRESS_WITHOUT_CHECKSUM =
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9D";

        private static string TEST_ADDRESS_WITH_CHECKSUM =
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY";

        [TestMethod]
        public void ShouldAddChecksum()
        {
            string result = Checksum.AddChecksum(TEST_ADDRESS_WITHOUT_CHECKSUM);
            Assert.AreEqual(result.Length, TEST_ADDRESS_WITH_CHECKSUM.Length);
            Assert.AreEqual(result, TEST_ADDRESS_WITH_CHECKSUM);
        }

        [TestMethod]
        public void ShouldRemoveChecksum()
        {
            Assert.AreEqual(Checksum.RemoveChecksum(TEST_ADDRESS_WITH_CHECKSUM), TEST_ADDRESS_WITHOUT_CHECKSUM);
        }
    }
}