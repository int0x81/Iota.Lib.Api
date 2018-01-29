﻿using Iota.Lib.Exception;


namespace Iota.Lib.Utils
{
    /// <summary>
    /// This class defines utility methods to add/remove the checksum to/from an address
    /// </summary>
    public static class Checksum
    {
        /// <summary>
        /// Adds the checksum to the specified address
        /// </summary>
        /// <param name="address">An address without checksum</param>
        /// <returns>The address with the appended checksum </returns>
        /// <exception cref="InvalidAddressException">is thrown when an invalid address is provided</exception>
        public static string AddChecksum(string address)
        {
            InputValidator.CheckAddress(address);
            string addressWithChecksum = address;
            addressWithChecksum += CalculateChecksum(address);
            return addressWithChecksum;
        }


        /// <summary>
        /// Removes the checksum from the specified address with checksum
        /// </summary>
        /// <param name="addressWithChecksum">The address with checksum.</param>
        /// <returns>the specified address without checksum</returns>
        /// <exception cref="InvalidAddressException">is thrown when the specified address is not an address with checksum</exception>
        public static string RemoveChecksum(this string addressWithChecksum)
        {
            if (IsAddressWithChecksum(addressWithChecksum))
            {
                return GetAddress(addressWithChecksum);
            }
            throw new InvalidAddressException(addressWithChecksum);
        }


        internal static string GetAddress(string addressWithChecksum)
        {
            return addressWithChecksum.Substring(0, Constants.ADDRESSLENGTH_WITHOUT_CHECKSUM);
        }

        /// <summary>
        /// Determines whether the specified address with checksum has a valid checksum.
        /// </summary>
        /// <param name="addressWithChecksum">The address with checksum.</param>
        /// <returns>
        ///   <c>true</c> if the specified address with checksum has a valid checksum [the specified address with checksum]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidChecksum(string addressWithChecksum)
        {
            string addressWithoutChecksum = RemoveChecksum(addressWithChecksum);
            string adressWithRecalculateChecksum = addressWithoutChecksum + CalculateChecksum(addressWithoutChecksum);
            return adressWithRecalculateChecksum.Equals(addressWithChecksum);
        }


        private static bool IsAddressWithChecksum(string addressWithChecksum)
        {
            return InputValidator.IsAddress(addressWithChecksum) && addressWithChecksum.Length == Constants.ADDRESSLENGTH_WITH_CHECKSUM;
        }

        private static string CalculateChecksum(string address)
        {
            ISponge sponge = new Kerl();
            sponge.Reset();
            int[] paddedAddress = ArrayUtils.PadArrayWithZeros(Converter.ConvertTrytesToTrits(address), Kerl.HASH_LENGTH);
            sponge.Absorb(paddedAddress);
            string checksum = Converter.ConvertTritsToTrytes(sponge.Squeeze(Kerl.HASH_LENGTH));
            return checksum.Substring(72);
        }
    }
}