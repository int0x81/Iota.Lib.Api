using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This class provides a set of utility methods to are used to convert between different formats
    /// </summary>
    public class Converter
    {
        /// <summary>
        /// The radix
        /// </summary>
        public static readonly int RADIX = 3;

        /// <summary>
        /// The maximum trit value
        /// </summary>
        public static readonly int MAX_TRIT_VALUE = (RADIX - 1)/2;

        /// <summary>
        /// The minimum trit value
        /// </summary>
        public static readonly int MIN_TRIT_VALUE = -MAX_TRIT_VALUE;

        /// <summary>
        /// The number of trits in a byte
        /// </summary>
        public static readonly int TRITS_IN_BYTE = 5;

        /// <summary>
        /// The number of trits in a tryte
        /// </summary>
        public static readonly int NumberOfTritsInATryte = 3;

        static readonly int[][] ByteToTritsMappings = new int[243][];
        static readonly int[][] TryteToTritsMappings = new int[27][];

        static Converter()
        {
            int[] trits = new int[TRITS_IN_BYTE];

            for (int i = 0; i < 243; i++)
            {
                ByteToTritsMappings[i] = new int[TRITS_IN_BYTE];
                ByteToTritsMappings[i] = new int[TRITS_IN_BYTE];
                Array.Copy(trits, ByteToTritsMappings[i], TRITS_IN_BYTE);
                Increment(trits, TRITS_IN_BYTE);
            }

            for (int i = 0; i < 27; i++)
            {
                TryteToTritsMappings[i] = new int[NumberOfTritsInATryte];
                Array.Copy(trits, TryteToTritsMappings[i], NumberOfTritsInATryte);
                Increment(trits, NumberOfTritsInATryte);
            }
        }

        /// <summary>
        /// Converts the specified trits array to bytes
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static byte[] ToBytes(int[] trits, int offset, int size)
        {
            byte[] bytes = new byte[(size + TRITS_IN_BYTE - 1)/TRITS_IN_BYTE];
            for (int i = 0; i < bytes.Length; i++)
            {
                int value = 0;
                for (
                    int j = (size - i*TRITS_IN_BYTE) < 5
                        ? (size - i*TRITS_IN_BYTE)
                        : TRITS_IN_BYTE;
                    j-- > 0;)
                {
                    value = value*RADIX + trits[offset + i*TRITS_IN_BYTE + j];
                }
                bytes[i] = (byte) value;
            }

            return bytes;
        }

        /// <summary>
        /// Converts the specified trits to trytes
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns></returns>
        public static byte[] ToBytes(int[] trits)
        {
            return ToBytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// Gets the trits from the specified bytes and stores it into the provided trits array
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="trits">The trits.</param>
        public static void GetTrits(sbyte[] bytes, int[] trits)
        {
            int offset = 0;
            for (int i = 0; i < bytes.Length && offset < trits.Length; i++)
            {
                Array.Copy(
                    ByteToTritsMappings[bytes[i] < 0 ? (bytes[i] + ByteToTritsMappings.Length) : bytes[i]], 0,
                    trits, offset,
                    trits.Length - offset < TRITS_IN_BYTE
                        ? (trits.Length - offset)
                        : TRITS_IN_BYTE);

                offset += TRITS_IN_BYTE;
            }
            while (offset < trits.Length)
            {
                trits[offset++] = 0;
            }
        }

        /// <summary>
        /// Converts the specified trinary encoded string into trits
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <returns></returns>
        public static int[] ToTritsString(string trytes)
        {
            int[] d = new int[3*trytes.Length];
            for (int i = 0; i < trytes.Length; i++)
            {
                Array.Copy(TryteToTritsMappings[Constants.TRYTE_ALPHABET.IndexOf(trytes[i])], 0, d,
                    i*NumberOfTritsInATryte, NumberOfTritsInATryte);
            }
            return d;
        }

        /// <summary>
        /// Converts the specified trinary encoded string into a trits array of the specified length.
        /// If the trytes string results in a shorter then specified trits array, then the remainder is padded we zeroes
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="length">The length.</param>
        /// <returns>a trits array</returns>
        public static int[] ToTrits(string trytes, int length)
        {
            int[] trits = ToTrits(trytes);

            List<int> tritsList = new List<int>();

            foreach (int i in trits)
                tritsList.Add(i);

            while (tritsList.Count < length)
                tritsList.Add(0);

            return tritsList.ToArray();
        }


        /// <summary>
        /// Converts the specified trinary encoded trytes string to trits
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <returns>An array of trits</returns>
        public static int[] ToTrits(string trytes)
        {
            List<int> trits = new List<int>();
            if (InputValidator.IsValue(trytes))
            {
                long value = long.Parse(trytes);

                long absoluteValue = value < 0 ? -value : value;

                int position = 0;

                while (absoluteValue > 0)
                {
                    int remainder = (int) (absoluteValue%RADIX);
                    absoluteValue /= RADIX;

                    if (remainder > MAX_TRIT_VALUE)
                    {
                        remainder = MIN_TRIT_VALUE;
                        absoluteValue++;
                    }

                    trits.Insert(position++, remainder);
                }
                if (value < 0)
                {
                    for (int i = 0; i < trits.Count; i++)
                    {
                        trits[i] = -trits[i];
                    }
                }
            }
            else
            {
                int[] d = new int[3*trytes.Length];
                for (int i = 0; i < trytes.Length; i++)
                {
                    Array.Copy(TryteToTritsMappings[Constants.TRYTE_ALPHABET.IndexOf(trytes[i])], 0, d,
                        i*NumberOfTritsInATryte, NumberOfTritsInATryte);
                }
                return d;
            }
            return trits.ToArray();
        }

        /// <summary>
        /// Copies the trits from the input string into the destination array
        /// </summary>
        /// <param name="input">The input string</param>
        /// <param name="destination">The destination array.</param>
        /// <returns></returns>
        public static int[] CopyTrits(string input, int[] destination)
        {
            for (int i = 0; i < input.Length; i++)
            {
                int index = Constants.TRYTE_ALPHABET.IndexOf(input[i]);
                if(index < 0)
                {
                    throw new ArgumentException($"{input} is no valid input!");
                }
                destination[i*3] = TryteToTritsMappings[index][0];
                destination[i*3 + 1] = TryteToTritsMappings[index][1];
                destination[i*3 + 2] = TryteToTritsMappings[index][2];
            }
            return destination;
        }

        /// <summary>
        /// Copies the trits in long representation into the destination array
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="destination">The destination array.</param>
        /// <param name="offset">The offset from which copying is started.</param>
        /// <param name="size">The size.</param>
        public static void CopyTrits(long value, int[] destination, int offset, int size)
        {
            long absoluteValue = value < 0 ? -value : value;
            for (int i = 0; i < size; i++)
            {
                int remainder = (int) (absoluteValue%RADIX);
                absoluteValue /= RADIX;
                if (remainder > MAX_TRIT_VALUE)
                {
                    remainder = MIN_TRIT_VALUE;
                    absoluteValue++;
                }
                destination[offset + i] = remainder;
            }

            if (value < 0)
            {
                for (int i = 0; i < size; i++)
                {
                    destination[offset + i] = -destination[offset + i];
                }
            }
        }

        /// <summary>
        /// Converts the trits array to a trytes string
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset from which copying is started.</param>
        /// <param name="size">The size.</param>
        /// <returns>a trytes string</returns>
        public static string ToTrytes(int[] trits, int offset, int size)
        {
            StringBuilder trytes = new StringBuilder();
            for (int i = 0; i < (size + NumberOfTritsInATryte - 1)/NumberOfTritsInATryte; i++)
            {
                int j = trits[offset + i*3] + trits[offset + i*3 + 1]*3 + trits[offset + i*3 + 2]*9;
                if (j < 0)
                {
                    j += Constants.TRYTE_ALPHABET.Length;
                }
                trytes.Append(Constants.TRYTE_ALPHABET[j]);
            }
            return trytes.ToString();
        }

        /// <summary>
        /// Converts the trits array to a trytes string
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>a trytes string</returns>
        public static string ToTrytes(int[] trits)
        {
            return ToTrytes(trits, 0, trits.Length);
        }

        /// <summary>
        /// Converts the specified trits array to trytes in integer representation
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>trytes in integer representation</returns>
        public static int ToTryteValue(int[] trits, int offset)
        {
            return trits[offset] + trits[offset + 1]*3 + trits[offset + 2]*9;
        }

        /// <summary>
        /// Converts the specified trits to its corresponding integer value
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>an integer value representing the corresponding trits</returns>
        public static int ToValue(int[] trits)
        {
            int value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value*3 + trits[i];
            }
            return value;
        }

        /// <summary>
        ///  Converts the specified trits to its corresponding integer value
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns></returns>
        public static long ToLongValue(int[] trits)
        {
            long value = 0;

            for (int i = trits.Length; i-- > 0;)
            {
                value = value*3 + trits[i];
            }
            return value;
        }

        /// <summary>
        /// Increments the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="size">The size.</param>
        public static void Increment(int[] trits, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (++trits[i] > MAX_TRIT_VALUE)
                {
                    trits[i] = MIN_TRIT_VALUE;
                }
                else
                {
                    break;
                }
            }
        }

        public static BigInteger ConvertTritsToBigInt(int[] trits, int offset, int size)
        {
            var value = BigInteger.Zero;

            for (var i = size; i-- > 0;)
            {
                value = BigInteger.Multiply(value, new BigInteger(RADIX));
                value = BigInteger.Add(value, new BigInteger(trits[offset + i]));
            }

            return value;
        }

        public static int[] ConvertBigIntToTrits(BigInteger value)
        {
            int[] trits = new int[Kerl.HASH_LENGTH];
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
                trits[counter] = remainder;
                counter++;
            }

            if (value < 0)
            {
                for(int i = 0; i < trits.Length; i++)
                {
                    trits[i] = -trits[i];
                }
            }

            return trits.ToArray();
        }
        public static byte[] ConvertBigIntToBytes(BigInteger value)
        {
            byte[] result = new byte[Kerl.BYTE_HASH_LENGTH];
            byte[] bytes = value.ToByteArray();              // In .NET Standard the 'ToByteArray()'-function fills the array in reverse order compared to .NET Framework 4.6
            bytes = bytes.Reverse().ToArray();


            var i = 0;
            while (i + bytes.Length < Kerl.BYTE_HASH_LENGTH)
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

        public static byte[] ConvertTritsToBytes(int[] trits)
        {
            return ConvertBigIntToBytes(ConvertTritsToBigInt(trits, 0, Kerl.HASH_LENGTH));
        }

        public static int[] ConvertBytesToTrits(IEnumerable<byte> bytes)
        {
            return ConvertBigIntToTrits(new BigInteger(bytes.Reverse().ToArray()));
        }

        public static BigInteger ConvertBytesToBigInt(IEnumerable<byte> bytes)
        {
            return new BigInteger(bytes.ToArray());
        }

        private static int FindMaxPowValue(BigInteger value)
        {
            int index = 0;
            
            while(true)
            {
                BigInteger quotient = BigInteger.Divide(value, BigInteger.Pow(RADIX, index));
                if(quotient == 0)
                {
                    return index;
                }
                index++;
            }
        }
    }
}