using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Iota.Lib.Utils
{
    
    /// <summary>
    /// This class allows to convert between ASCII and tryte encoded strings 
    /// </summary>
    public class ASCIIConverter
    {
        /// <summary>
        /// Converts the ASCII encoded string to trytes
        /// </summary>
        /// <param name="inputString">ASCII encoded string</param>
        /// <returns>tryte encoded string</returns>
        public static string ToTrytes(string inputString)
        {
            StringBuilder trytes = new StringBuilder();

            for (int i = 0; i < inputString.Length; i++)
            {
                char asciiValue = inputString.ElementAt(i);

                // If not recognizable ASCII character, replace with space
                if (asciiValue > 255)
                {
                    asciiValue = (char) 32;
                }

                int firstValue = asciiValue%27;
                int secondValue = (asciiValue - firstValue)/27;

                string trytesValue = Constants.TRYTE_ALPHABET.ElementAt(firstValue).Key.ToString() + Constants.TRYTE_ALPHABET.ElementAt(secondValue).Key.ToString();

                trytes.Append(trytesValue);
            }

            return trytes.ToString();
        }



        /// <summary>
        /// Converts the specified tryte encoded String to ASCII
        /// </summary>
        /// <param name="inputTrytes">tryte encoded string</param>
        /// <returns>an ASCII encoded string</returns>
        public static string ToString(string inputTrytes)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < inputTrytes.Length; i += 2)
            {
                int firstValue = FindDictionaryIndex(Constants.TRYTE_ALPHABET, inputTrytes[i]);
                int secondValue = FindDictionaryIndex(Constants.TRYTE_ALPHABET, inputTrytes[i+1]);

                int decimalValue = firstValue + secondValue*27;

                string character = ((char) decimalValue).ToString();

                builder.Append(character);
            }

            return builder.ToString();
        }

        private static int FindDictionaryIndex(Dictionary<char, int[]> dict, char keyValue)
        {
            int index = 0;

            while(index < dict.Count)
            {
                if (dict.ElementAt(index).Key.Equals(keyValue))
                {
                    return index;
                }
                index++;
            }
            return index;
        }
    }
}