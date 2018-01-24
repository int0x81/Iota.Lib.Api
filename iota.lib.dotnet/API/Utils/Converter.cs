﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using Iota.Lib.CSharp.Api.Exception;

using static Iota.Lib.CSharp.Api.Utils.Constants;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This class provides a set of methods used to convert between different formats
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// Converts a tryte-string into a trit-array
        /// </summary>
        /// <param name="trytes">The tryte-string</param>
        /// <returns>A trit-array</returns>
        public static int[] ConvertTrytesToTrits(string trytes)
        {
            if(!InputValidator.IsTrytes(trytes, trytes.Length))
            {
                throw new InvalidTryteException();
            }

            List<int> trits = new List<int>();
            foreach(char c in trytes)
            {
                if(Constants.TRYTE_ALPHABET.TryGetValue(c, out int[] tryteAsTrits))
                {
                    trits.Add(tryteAsTrits[0]);
                    trits.Add(tryteAsTrits[1]);
                    trits.Add(tryteAsTrits[2]);
                }
            }

            return ArrayUtils.EraseNullValuesFromEnd(trits.ToArray());
        }

        /// <summary>
        /// Converts a trit-array into a tryte-string
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <returns>A tryte-string</returns>
        public static string ConvertTritsToTrytes(int[] trits)
        {
            StringBuilder builder = new StringBuilder();
            int index = 0;
            int remainding = trits.Length % NUMBER_OF_TRITS_IN_A_TRYTE;

            while (index <= trits.Length - remainding)
            {
                if(index % NUMBER_OF_TRITS_IN_A_TRYTE == 0 && index != 0)
                {
                    int[] currentTrits = new int[NUMBER_OF_TRITS_IN_A_TRYTE];
                    Array.Copy(trits, index - NUMBER_OF_TRITS_IN_A_TRYTE, currentTrits, 0, NUMBER_OF_TRITS_IN_A_TRYTE);
                    int dictionaryIndex = ConvertTritsToInteger(currentTrits);
                    if(dictionaryIndex < 0)
                    {
                        dictionaryIndex += 27;
                    }
                    builder.Append(Constants.TRYTE_ALPHABET.ElementAt(dictionaryIndex).Key);
                }
                index++;
            }

            if(remainding != 0)
            {
                int[] currentTrits = new int[remainding];
                Array.Copy(trits, trits.Length - remainding, currentTrits, 0, remainding);
                int dictionaryIndex = ConvertTritsToInteger(currentTrits);
                if (dictionaryIndex < 0)
                {
                    dictionaryIndex += 27;
                }
                builder.Append(Constants.TRYTE_ALPHABET.ElementAt(dictionaryIndex).Key);
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Converts a trit-array to a BigInteger
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <returns>A BigInteger</returns>
        public static BigInteger ConvertTritsToBigInt(int[] trits)
        {
            BigInteger value = BigInteger.Zero;

            for (var i = trits.Length; i-- > 0;)
            {
                value = BigInteger.Add(BigInteger.Multiply(Constants.RADIX, value), new BigInteger(trits[i]));
            }

            return value;
        }

        /// <summary>
        /// Converts a BigInteger into a trit-array
        /// </summary>
        /// <param name="value">The BigInteger</param>
        /// <returns>A trit-array</returns>
        public static int[] ConvertBigIntToTrits(BigInteger value)
        {
            List<int> trits = new List<int>();
            BigInteger absoluteValue = BigInteger.Abs(value);
            int counter = 0;
            while (absoluteValue > 0)
            {
                BigInteger quotient = BigInteger.DivRem(absoluteValue, new BigInteger(RADIX), out BigInteger remainder_as_bi);

                int remainder = (int)remainder_as_bi;
                absoluteValue = quotient;

                if (remainder > MAX_TRIT_VALUE)
                {
                    remainder = MIN_TRIT_VALUE;
                    absoluteValue = BigInteger.Add(absoluteValue, BigInteger.One);
                }
                trits.Add(remainder);
                counter++;
            }

            if (value < 0)
            {
                for(int i = 0; i < trits.Count; i++)
                {
                    trits[i] = -trits[i];
                }
            }
            return trits.ToArray();
        }

        /// <summary>
        /// Converts a BigInteger into a byte-array
        /// </summary>
        /// <param name="value">The BigInteger</param>
        /// <param name="outputLength">The length of the returning byte-array</param>
        /// <returns>A byte-array</returns>
        public static byte[] ConvertBigIntToBytes(BigInteger value, int outputLength)
        {
            byte[] result = new byte[outputLength];
            byte[] bytes = value.ToByteArray();              
            bytes = bytes.Reverse().ToArray(); // In .NET Standard the 'ToByteArray()'-Function fills the array in reverse order compared to .NET Framework 4


            var i = 0;
            while (i + bytes.Length < outputLength)
            {
                if (value < 0)
                {
                    result[i++] = 255;
                }
                else
                {
                    result[i++] = 0;
                }
            }

            for (int j = bytes.Length; j-- > 0;)
            {
                result[i++] = bytes[bytes.Length - 1 - j];
            }

            return result;
        }

        /// <summary>
        /// Converts a byte-array into a BigInteger
        /// </summary>
        /// <param name="bytes">The byte-array</param>
        /// <returns>A BigInteger</returns>
        public static BigInteger ConvertBytesToBigInt(IEnumerable<byte> bytes)
        {
            return new BigInteger(bytes.ToArray());
        }

        /// <summary>
        /// Converts a trit-array into a byte-array
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <param name="outputLength">The length of the returning byte-array</param>
        /// <returns>A byte-array</returns>
        public static byte[] ConvertTritsToBytes(int[] trits, int outputLength)
        {
            return ConvertBigIntToBytes(ConvertTritsToBigInt(trits), outputLength);
        }

        /// <summary>
        /// Converts a byte-array to a trit-array
        /// </summary>
        /// <param name="bytes">The byte-array</param>
        /// <returns>A trit-array</returns>
        public static int[] ConvertBytesToTrits(IEnumerable<byte> bytes)
        {
            return ConvertBigIntToTrits(new BigInteger(bytes.Reverse().ToArray()));
        }

        /// <summary>
        /// Converts a small trit-array into an integer
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <exception cref="ArgumentException">Thrown when the trits-array exceeds the specific limit to avoid an integeroverflow</exception>
        /// <returns>An integer</returns>
        public static int ConvertTritsToInteger(int[] trits)
        {
            const int TRITS_MAX_LENGTH = 9;

            if(ArrayUtils.EraseNullValuesFromEnd(trits).Length > TRITS_MAX_LENGTH)
            {
                throw new ArgumentException($"To avoid an integeroverflow the trit-array may only contain {TRITS_MAX_LENGTH} digests");
            }
            int value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value * 3 + trits[i];
            }
            return value;
        }

        /// <summary>
        /// Converts a small trit-array into a long
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <exception cref="ArgumentException">Thrown when the trits-array exceeds the specific limit to avoid an integeroverflow</exception>
        /// <returns>A long</returns>
        public static long ConvertTritsToLong(int[] trits)
        {
            const int TRITS_MAX_LENGTH = 9;

            if (ArrayUtils.EraseNullValuesFromEnd(trits).Length > TRITS_MAX_LENGTH)
            {
                throw new ArgumentException($"To avoid an integeroverflow the trit-array may only contain {TRITS_MAX_LENGTH} digests");
            }

            long value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value * 3 + trits[i];
            }
            return value;
        }

        /// <summary>
        /// Increments the value of a trit-array
        /// </summary>
        /// <param name="trits">The trit-array</param>
        /// <param name="summand">The summand (1 by default)</param>
        /// <returns>The incremented trit-array</returns>
        public static int[] Increment(int[] trits, int summand = 1)
        {
            BigInteger value = ConvertTritsToBigInt(trits);
            BigInteger incrementedValue = BigInteger.Add(value, summand);
            return ConvertBigIntToTrits(incrementedValue);
        }

        /// <summary>
        /// Increments the value of a tryte-string
        /// </summary>
        /// <param name="trits">The tryte-string</param>
        /// <param name="summand">The summand (1 by default)</param>
        /// <returns>The incremented tryte-string</returns>
        public static string Increment(string trytes, int summand = 1)
        {
            int[] value = ConvertTrytesToTrits(trytes);
            int[] incrementedValue = Increment(value, summand);
            return ConvertTritsToTrytes(ArrayUtils.EraseNullValuesFromEnd(incrementedValue));
        }

    }
}