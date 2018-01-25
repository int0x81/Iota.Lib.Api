using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests
{
    [TestClass]
    public class ChecksumTest
    {
        private static string[] TEST_ADDRESSES_WITHOUT_CHECKSUM = {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9D",
            "FAJIXQNBJHCCVBIW9PDYIAXAHWJZHHUNTOPLXTPGYIHYGUGTCTOWJSJZLJQZPBNL9FCRSFTENJLVSDMPD"
        };

        private static string[] TEST_ADDRESSES_WITH_CHECKSUM = {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY",
            "FAJIXQNBJHCCVBIW9PDYIAXAHWJZHHUNTOPLXTPGYIHYGUGTCTOWJSJZLJQZPBNL9FCRSFTENJLVSDMPDETBRCTSI9"
        };

        [TestMethod]
        public void TestAddChecksum()
        {
            Assert.AreEqual(TEST_ADDRESSES_WITH_CHECKSUM[0], Checksum.AddChecksum(TEST_ADDRESSES_WITHOUT_CHECKSUM[0]));
            Assert.AreEqual(TEST_ADDRESSES_WITH_CHECKSUM[1], Checksum.AddChecksum(TEST_ADDRESSES_WITHOUT_CHECKSUM[1]));
        }

        [TestMethod]
        public void TestRemoveChecksum()
        {
            Assert.AreEqual(Checksum.RemoveChecksum(TEST_ADDRESSES_WITH_CHECKSUM[0]), TEST_ADDRESSES_WITHOUT_CHECKSUM[0]);
            Assert.AreEqual(Checksum.RemoveChecksum(TEST_ADDRESSES_WITH_CHECKSUM[1]), TEST_ADDRESSES_WITHOUT_CHECKSUM[1]);
        }

        [TestMethod]
        public void TestIsAddress()
        {
            Assert.IsTrue(InputValidator.IsAddress(TEST_ADDRESSES_WITHOUT_CHECKSUM[0]));
            Assert.IsTrue(InputValidator.IsAddress(TEST_ADDRESSES_WITHOUT_CHECKSUM[1]));
            Assert.IsTrue(InputValidator.IsAddress(TEST_ADDRESSES_WITH_CHECKSUM[0]));
            Assert.IsTrue(InputValidator.IsAddress(TEST_ADDRESSES_WITH_CHECKSUM[1]));
        }
    }
}