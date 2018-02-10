using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iota.Lib.Test.UtilsTests
{
    [TestClass]
    public class ArrayUtilsTest
    {
        [TestMethod]
        public void TestAdjustTryteString()
        {
            string tryteString = "TZTZTZ";
            string paddedTryteString = ArrayUtils.AdjustTryteString(tryteString, 9);
            Assert.AreEqual(paddedTryteString, "TZTZTZ999");
            Assert.AreEqual("TZTZTZ", ArrayUtils.AdjustTryteString(paddedTryteString, 6));
        }

    }
}
