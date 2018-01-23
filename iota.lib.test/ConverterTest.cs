using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Iota.Lib.CSharpTests
{
    [TestClass]
    public class ConverterTest
    {
        //int[] trits_01 = { 1, 1, 1 };          //13
        //int[] trits_02 = { -1, -1, -1, 1 };    //14
        //int[] trits_03 = { 1, 1, 1, -1 };      //-14
        //int[] trits_04 = { 1, 0, 0, 1 };       //28

        //[TestMethod]
        //public void Check_Trits_To_BigInt_Conversion()
        //{
        //    Assert.AreEqual(Converter.ConvertTritsToBigInt(trits_01), new BigInteger(13));
        //    Assert.AreEqual(Converter.ConvertTritsToBigInt(trits_02), new BigInteger(14));
        //    Assert.AreEqual(Converter.ConvertTritsToBigInt(trits_03), new BigInteger(-14));
        //    Assert.AreEqual(Converter.ConvertTritsToBigInt(trits_04), new BigInteger(28));
        //    Assert.AreNotEqual(Converter.ConvertTritsToBigInt(trits_04), new BigInteger(1337));
        //}

        //[TestMethod]
        //public void Check_BigInt_To_Trits_Conversion()
        //{
        //    var tmp_01 = Converter.ConvertBigIntToTrits(new BigInteger(13));
        //    var tmp_02 = Converter.ConvertBigIntToTrits(new BigInteger(14));
        //    var tmp_03 = Converter.ConvertBigIntToTrits(new BigInteger(-14));
        //    var tmp_04 = Converter.ConvertBigIntToTrits(new BigInteger(28));
        //    var tmp_05 = Converter.ConvertBigIntToTrits(new BigInteger(1337));

        //    Assert.IsTrue(CompareEachElement(tmp_01, trits_01));
        //    Assert.IsTrue(CompareEachElement(tmp_02, trits_02));
        //    Assert.IsTrue(CompareEachElement(tmp_03, trits_03));
        //    Assert.IsTrue(CompareEachElement(tmp_04, trits_04));
        //    Assert.IsFalse(CompareEachElement(tmp_05, trits_04));
        //}

        //private bool CompareEachElement(int[] firstArray, int[] secondArray)
        //{
        //    if (firstArray == null || firstArray.Length != secondArray.Length)
        //    {
        //        return false;
        //    }

        //    for (int i = 0; i < firstArray.Length; i++)
        //    {
        //        if (firstArray[i] != secondArray[i])
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
