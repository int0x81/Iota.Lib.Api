using System.IO;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.Tests
{
    [TestClass]
    public class KerlTest
    {
        Kerl kerl = new Kerl();

        [TestMethod]
        public void TestKerlAbsorbAndSqueeze()
        {
            kerl.Reset();
            int[] tritValue = Converter.ConvertTrytesToTrits("KFNNRVYTYYYNHJLBTXOEFYBZTHGXHTX9XKXB9KUZDHGLKBQGPQCNHPGDSGYKWGHVXVLHPOEAWREBIVK99");

            kerl.Absorb(tritValue);
            string hash = Converter.ConvertTritsToTrytes(kerl.Squeeze());
            Assert.AreEqual("SHTKPLZWIXLDVHAEAGFSVWNDGVIX9SDVGEHAFGXEIMLWSHDTQYNZZKPBGMUF9GNEWIGIFYWWMSCLJ9RCD", hash);
        }
    }
}