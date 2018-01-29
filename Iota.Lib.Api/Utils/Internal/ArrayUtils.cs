using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Iota.Lib.Test")]

namespace Iota.Lib.Utils
{
    internal class ArrayUtils
    {
        public static IEnumerable<T> SliceRow<T>(T[,] array, int row)
        {
            for(int i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        /// <summary>
        /// Creates a sub array of a given array.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="sourceArray">The source array.</param>
        /// <param name="index">The begin of the new array.</param>
        /// <param name="length">The length of the new array.</param>
        /// <returns>A new array with the given length.</returns>
        public static T[] CreateSubArray<T>(T[] sourceArray, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(sourceArray, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Takes a int-array and erase all 0's from the end.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The 0-erased array.</returns>
        public static int[] EraseNullValuesFromEnd(int[] array)
        {
            List<int> list = new List<int>(array);
            
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i] == 0)
                {
                    list.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Pads an array of type int with zeros until a given size is reached.
        /// </summary>
        /// <param name="oldArray">The array.</param>
        /// <param name="newLength">The new length.</param>
        /// <returns>An int-array of the new size.</returns>
        public static int[] PadArrayWithZeros(int[]oldArray, int newLength)
        {
            if(oldArray.Length > newLength)
            {
                throw new ArgumentException("The desired length must be larger then the size of the array");
            }
            int[] newArray = new int[newLength];
            Array.Copy(oldArray, newArray, oldArray.Length);

            int index = oldArray.Length;
            while(index < newLength)
            {
                newArray[index] = 0;
                index++;
            }
            return newArray;
        }

        /// <summary>
        /// Compares each element of two arrays and checks if all elements contain the same value.
        /// </summary>
        /// <param name="firstArray">The first array.</param>
        /// <param name="secondArray">The second array.</param>
        /// <returns>A boolean that determines if both arrays are equal.</returns>
        public static bool CompareEachElement(int[] firstArray, int[] secondArray)
        {
            if (firstArray == null || firstArray.Length != secondArray.Length)
            {
                return false;
            }

            for (int i = 0; i < firstArray.Length; i++)
            {
                if (firstArray[i] != secondArray[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
