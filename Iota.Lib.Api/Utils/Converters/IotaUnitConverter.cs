using System;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// Provides methods to convert Iota to different units
    /// </summary>
    public class IotaUnitConverter
    {
        /// <summary>
        /// Convert the iota amount
        /// </summary>
        /// <param name="amount">amount</param>
        /// <param name="fromUnit">the source unit e.g. the unit of amount</param>
        /// <param name="toUnit">the target unit</param>
        /// <returns>the specified amount in the target unit</returns>
        public static double ConvertUnits(long amount, IotaUnit fromUnit, IotaUnit toUnit)
        {
            long amountInSource = (long) (amount*Math.Pow(10, (int) fromUnit));
            return ConvertUnits(amountInSource, toUnit);
        }

        private static double ConvertUnits(long amount, IotaUnit toUnit)
        {
            int base10NormalizationExponent = (int) toUnit;
            return (amount/Math.Pow(10, base10NormalizationExponent));
        }

        /// <summary>
        /// Finds the optimal unit to display the specified amount in
        /// </summary>
        /// <param name="amount">amount </param>
        /// <returns>the optimal IotaUnit</returns>
        public static IotaUnit FindOptimalIotaUnitToDisplay(long amount)
        {
            int length = (amount).ToString().Length;

            if (amount < 0)
            {
                length -= 1;
            }

            IotaUnit units = IotaUnit.Iota;

            if (length >= 1 && length <= 3)
            {
                units = IotaUnit.Iota;
            }
            else if (length > 3 && length <= 6)
            {
                units = IotaUnit.Kilo;
            }
            else if (length > 6 && length <= 9)
            {
                units = IotaUnit.Mega;
            }
            else if (length > 9 && length <= 12)
            {
                units = IotaUnit.Giga;
            }
            else if (length > 12 && length <= 15)
            {
                units = IotaUnit.Terra;
            }
            else if (length > 15 && length <= 18)
            {
                units = IotaUnit.Peta;
            }
            return units;
        }
    }
}