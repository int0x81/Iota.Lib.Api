namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This class defines different constants that are used accros the library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// This String contains all possible characters of the tryte alphabet
        /// </summary>                              
        public static readonly string TRYTE_ALPHABET = "9ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// The maximum seed length
        /// </summary>
        public static readonly int SEED_MAX_LENGTH = 81;

        /// <summary>
        /// This String represents the empty hash consisting of '9'
        /// </summary>
        public static readonly string EMPTY_HASH =
            "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        /// <summary>
        /// The length of an address without checksum
        /// </summary>
        public static readonly int ADDRESSLENGTH_WITHOUT_CHECKSUM = 81;

        /// <summary>
        /// The address length with checksum
        /// </summary>
        public static readonly int ADDRESSLENGTH_WITH_CHECKSUM = 90;
    }
}