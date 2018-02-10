using Iota.Lib.Exception;


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
            if(!InputValidator.IsValidAddress(address))
            {
                throw new InvalidAddressException($"{address} is no valid address");
            }

             return address + CalculateChecksum(address);
        }


        /// <summary>
        /// Removes the checksum from the specified address
        /// </summary>
        /// <param name="addressWithChecksum">The address with checksum</param>
        /// <exception cref="InvalidAddressException"> is thrown when the specified address is not an address with checksum</exception>
        public static string RemoveChecksum(string addressWithChecksum)
        {
            if (IsAddressWithChecksum(addressWithChecksum))
            {
                return addressWithChecksum.Substring(0, Constants.ADDRESSLENGTH_WITHOUT_CHECKSUM);
            }
            throw new InvalidAddressException(addressWithChecksum);
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
            string addressWithoutChecksum = addressWithChecksum.Substring(0, Constants.ADDRESSLENGTH_WITHOUT_CHECKSUM);

            return addressWithChecksum.Equals(addressWithoutChecksum + CalculateChecksum(addressWithoutChecksum));
        }


        private static bool IsAddressWithChecksum(string addressWithChecksum)
        {
            return InputValidator.IsValidAddress(addressWithChecksum) && addressWithChecksum.Length == Constants.ADDRESSLENGTH_WITH_CHECKSUM;
        }

        private static string CalculateChecksum(string address)
        {
            if(!InputValidator.IsValidAddress(address) || address.Length != Constants.ADDRESSLENGTH_WITHOUT_CHECKSUM)
            {
                throw new InvalidAddressException($"{address} is no valid address");
            }
            Kerl kerl = new Kerl();
            kerl.Reset();
            kerl.Absorb(Converter.ConvertTrytesToTrits(address));
            int[] hashInTrits = new int[Kerl.HASH_LENGTH];
            kerl.Squeeze(ref hashInTrits, 0 ,hashInTrits.Length);
            string checksum = Converter.ConvertTritsToTrytes(hashInTrits);
            return checksum.Substring(72);
        }
    }
}