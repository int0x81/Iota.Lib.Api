using System.IO;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.Test
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
            int[] hashInTrits = new int[Kerl.HASH_LENGTH];
            kerl.Squeeze(ref hashInTrits, 0 , hashInTrits.Length);
            string hash = Converter.ConvertTritsToTrytes(hashInTrits);
            Assert.AreEqual("SHTKPLZWIXLDVHAEAGFSVWNDGVIX9SDVGEHAFGXEIMLWSHDTQYNZZKPBGMUF9GNEWIGIFYWWMSCLJ9RCD", hash);
        }
    }
}