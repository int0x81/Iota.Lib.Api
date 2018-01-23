using System.IO;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests
{
  [TestClass]
  public class KerlTest_02
  {

    [TestMethod]
    public void TestKerlOneAbsorb()
    {
      var tritValue = Converter.ToTrits("KFNNRVYTYYYNHJLBTXOEFYBZTHGXHTX9XKXB9KUZDHGLKBQGPQCNHPGDSGYKWGHVXVLHPOEAWREBIVK99");

      var kerl = new Kerl();
      kerl.Absorb(tritValue);

      var hash = Converter.ToTrytes(kerl.Squeeze());
      Assert.AreEqual("SHTKPLZWIXLDVHAEAGFSVWNDGVIX9SDVGEHAFGXEIMLWSHDTQYNZZKPBGMUF9GNEWIGIFYWWMSCLJ9RCD", hash);
    }

    [TestMethod]
    public void KurlMultiSqueeze() 
    {
      var tritValue = Converter.ToTrits("9MIDYNHBWMBCXVDEFOFWINXTERALUKYYPPHKP9JJFGJEIUY9MUDVNFZHMMWZUYUSWAIOWEVTHNWMHANBH");

      var kerl = new Kerl();
      kerl.Absorb(tritValue);

      var hashValue = new int[Kerl.HASH_LENGTH * 2];
      kerl.Squeeze(hashValue, 0, hashValue.Length);

      var hash = Converter.ToTrytes(hashValue);
      Assert.AreEqual("G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA", hash);
    }

    [TestMethod]
    public void KurlMultiAbsorbMultiSqueeze() 
    {
      var tritValue = Converter.ToTrits("G9JYBOMPUXHYHKSNRNMMSSZCSHOFYOYNZRSZMAAYWDYEIMVVOGKPJBVBM9TDPULSFUNMTVXRKFIDOHUXXVYDLFSZYZTWQYTE9SPYYWYTXJYQ9IFGYOLZXWZBKWZN9QOOTBQMWMUBLEWUEEASRHRTNIQWJQNDWRYLCA");

      var kerl = new Kerl();
      kerl.Absorb(tritValue);

      var hashValue = new int[Kerl.HASH_LENGTH * 2];
      kerl.Squeeze(hashValue, 0, hashValue.Length);

      var hash = Converter.ToTrytes(hashValue);
      Assert.AreEqual("LUCKQVACOGBFYSPPVSSOXJEKNSQQRQKPZC9NXFSMQNRQCGGUL9OHVVKBDSKEQEBKXRNUJSRXYVHJTXBPDWQGNSCDCBAIRHAQCOWZEBSNHIJIGPZQITIBJQ9LNTDIBTCQ9EUWKHFLGFUVGGUWJONK9GBCDUIMAYMMQX", hash);
    }

    [TestMethod]
    public void TestGenerateTrytesAndMultiSqueeze()
    {
      using (var reader = new StreamReader("../../generate_trytes_and_multi_squeeze.csv"))
      {
        var i = 0;
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine();
          if (line == null || i == 0)
          {
            i++;
            continue;
          }

          var values = line.Split(',');

          var trytes = values[0];
          var hashes1 = values[1];
          var hashes2 = values[2];
          var hashes3 = values[3];

          var trits = Converter.ToTrits(trytes);

          var kerl = new Kerl();
          kerl.Absorb(trits);

          var trytesOut = Converter.ToTrytes(kerl.Squeeze());

          Assert.AreEqual(hashes1, trytesOut);

          trytesOut = Converter.ToTrytes(kerl.Squeeze());

          Assert.AreEqual(hashes2, trytesOut);

          trytesOut = Converter.ToTrytes(kerl.Squeeze());

          Assert.AreEqual(hashes3, trytesOut);
          i++;
        }
      }
    }
  }
}