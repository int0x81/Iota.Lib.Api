using Iota.Lib.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

using static Iota.Lib.Api.Utils.Converter;

namespace Iota.Lib.Test
{
    [TestClass]
    public class ConverterTest
    {
        readonly int[] trits_01 = { 1, 1, 1 };          //13
        readonly int[] trits_02 = { -1, -1, -1, 1 };    //14
        readonly int[] trits_03 = { 1, 1, 1, -1 };      //-14
        readonly int[] trits_04 = { 1, 0, 0, 1 };       //28
        readonly int[] trits_05 = { 0 };                //0

        readonly string trytes_01 = "M";
        readonly string trytes_02 = "NA";
        readonly string trytes_03 = "MZ";
        readonly string trytes_04 = "AA";
        readonly string trytes_05 = "9";

        [TestMethod]
        public void TestTritsToBigInt()
        {
            Assert.AreEqual(ConvertTritsToBigInt(trits_01), new BigInteger(13));
            Assert.AreEqual(ConvertTritsToBigInt(trits_02), new BigInteger(14));
            Assert.AreEqual(ConvertTritsToBigInt(trits_03), new BigInteger(-14));
            Assert.AreEqual(ConvertTritsToBigInt(trits_04), new BigInteger(28));
            Assert.AreEqual(ConvertTritsToBigInt(trits_05), new BigInteger(0));
            Assert.AreNotEqual(ConvertTritsToBigInt(trits_04), new BigInteger(1337));
        }

        [TestMethod]
        public void TestBigIntToTrits()
        {
            var tmp_01 = ConvertBigIntToTrits(new BigInteger(13));
            var tmp_02 = ConvertBigIntToTrits(new BigInteger(14));
            var tmp_03 = ConvertBigIntToTrits(new BigInteger(-14));
            var tmp_04 = ConvertBigIntToTrits(new BigInteger(28));
            var tmp_05 = ConvertBigIntToTrits(new BigInteger(1337));

            Assert.IsTrue(ArrayUtils.CompareEachElement(tmp_01, trits_01));
            Assert.IsTrue(ArrayUtils.CompareEachElement(tmp_02, trits_02));
            Assert.IsTrue(ArrayUtils.CompareEachElement(tmp_03, trits_03));
            Assert.IsTrue(ArrayUtils.CompareEachElement(tmp_04, trits_04));
            Assert.IsFalse(ArrayUtils.CompareEachElement(tmp_05, trits_04));
        }

        [TestMethod]
        public void TestTritsToInt()
        {
            int tmp_01 = ConvertTritsToInteger(trits_01);
            int tmp_02 = ConvertTritsToInteger(trits_02);
            int tmp_03 = ConvertTritsToInteger(trits_03);
            int tmp_04 = ConvertTritsToInteger(trits_04);
            int tmp_05 = ConvertTritsToInteger(trits_05);

            Assert.AreEqual(13, tmp_01);
            Assert.AreEqual(14, tmp_02);
            Assert.AreEqual(-14, tmp_03);
            Assert.AreEqual(28, tmp_04);
            Assert.AreEqual(0, tmp_05);
        }

        [TestMethod]
        public void TestTritsIncremention()
        {
            var tmp_01 = Increment(trits_01);
            var tmp_02 = Increment(trits_02);
            var tmp_03 = Increment(trits_03);
            var tmp_04 = Increment(trits_04, 28);
            var tmp_05 = Increment(trits_05);

            Assert.AreEqual(14, ConvertTritsToInteger(tmp_01));
            Assert.AreEqual(15, ConvertTritsToInteger(tmp_02));
            Assert.AreEqual(-13, ConvertTritsToInteger(tmp_03));
            Assert.AreEqual(56, ConvertTritsToInteger(tmp_04));
            Assert.AreEqual(1, ConvertTritsToInteger(tmp_05));
        }

        [TestMethod]
        public void TestTritsToTrytes()
        {
            Assert.AreEqual(trytes_01, ConvertTritsToTrytes(trits_01));
            Assert.AreEqual(trytes_02, ConvertTritsToTrytes(trits_02));
            Assert.AreEqual(trytes_03, ConvertTritsToTrytes(trits_03));
            Assert.AreEqual(trytes_04, ConvertTritsToTrytes(trits_04));
            Assert.AreEqual(trytes_05, ConvertTritsToTrytes(trits_05));
        }

        [TestMethod]
        public void TestTritsToLong()
        {
            long tmp_01 = 13;
            long tmp_02 = 14;
            long tmp_03 = -14;
            long tmp_04 = 28;
            long tmp_05 = 0;

            Assert.AreEqual(tmp_01, ConvertTritsToLong(trits_01));
            Assert.AreEqual(tmp_02, ConvertTritsToLong(trits_02));
            Assert.AreEqual(tmp_03, ConvertTritsToLong(trits_03));
            Assert.AreEqual(tmp_04, ConvertTritsToLong(trits_04));
            Assert.AreEqual(tmp_05, ConvertTritsToLong(trits_05));
        }

        [TestMethod]
        public void TestTrytesToTrits()
        {
            Assert.IsTrue(ArrayUtils.CompareEachElement(trits_01, ConvertTrytesToTrits(trytes_01)));
            Assert.IsTrue(ArrayUtils.CompareEachElement(trits_02, ConvertTrytesToTrits(trytes_02)));
            Assert.IsTrue(ArrayUtils.CompareEachElement(trits_03, ConvertTrytesToTrits(trytes_03)));
            Assert.IsTrue(ArrayUtils.CompareEachElement(trits_04, ConvertTrytesToTrits(trytes_04)));
            Assert.IsTrue(ArrayUtils.CompareEachElement(trits_05, ConvertTrytesToTrits(trytes_05)));
        }

        [TestMethod]
        public void TestIncrementTrytes()
        {
            Assert.AreEqual("NA", Increment(trytes_01));
            Assert.AreEqual("OA", Increment(trytes_02));
            Assert.AreEqual("N", Increment(trytes_03));
            Assert.AreEqual("BA", Increment(trytes_04));
            Assert.AreEqual("A", Increment(trytes_05));
            Assert.AreEqual("W", Increment(trytes_05, -4));
        }
    }
}
