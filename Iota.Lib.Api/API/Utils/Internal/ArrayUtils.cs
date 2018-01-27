using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Iota.Lib.Test")]

namespace Iota.Lib.Api.Utils
{
    internal class ArrayUtils
    {
        public static IEnumerable<T> SliceRow<T>(T[,] array, int row)
        {
            for (var i = array.GetLowerBound(1); i <= array.GetUpperBound(1); i++)
            {
                yield return array[row, i];
            }
        }

        public static T[] SubArray<T>(T[] data, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            T[] result = new T[endIndex - startIndex];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }

        public static T[] SubArray2<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

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
