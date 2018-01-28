using System.Collections.Generic;

namespace Iota.Lib.Api.Utils
{
    public static class Constants
    { 
        /// <summary>
        /// Contains all the trytes with the corresponding trits-value
        /// </summary>
        public static readonly Dictionary<char, int[]> TRYTE_ALPHABET = new Dictionary<char, int[]>
                                                                     {
                                                                       { '9', new[] { 0, 0, 0 } },
                                                                       { 'A', new[] { 1, 0, 0 } },
                                                                       { 'B', new[] { -1, 1, 0 } },
                                                                       { 'C', new[] { 0, 1, 0 } },
                                                                       { 'D', new[] { 1, 1, 0 } },
                                                                       { 'E', new[] { -1, -1, 1 } },
                                                                       { 'F', new[] { 0, -1, 1 } },
                                                                       { 'G', new[] { 1, -1, 1 } },
                                                                       { 'H', new[] { -1, 0, 1 } },
                                                                       { 'I', new[] { 0, 0, 1 } },
                                                                       { 'J', new[] { 1, 0, 1 } },
                                                                       { 'K', new[] { -1, 1, 1 } },
                                                                       { 'L', new[] { 0, 1, 1 } },
                                                                       { 'M', new[] { 1, 1, 1 } },
                                                                       { 'N', new[] { -1, -1, -1 } },
                                                                       { 'O', new[] { 0, -1, -1 } },
                                                                       { 'P', new[] { 1, -1, -1 } },
                                                                       { 'Q', new[] { -1, 0, -1 } },
                                                                       { 'R', new[] { 0, 0, -1 } },
                                                                       { 'S', new[] { 1, 0, -1 } },
                                                                       { 'T', new[] { -1, 1, -1 } },
                                                                       { 'U', new[] { 0, 1, -1 } },
                                                                       { 'V', new[] { 1, 1, -1 } },
                                                                       { 'W', new[] { -1, -1, 0 } },
                                                                       { 'X', new[] { 0, -1, 0 } },
                                                                       { 'Y', new[] { 1, -1, 0 } },
                                                                       { 'Z', new[] { -1, 0, 0 } }
                                                                     };

        /// <summary>
        /// The maximum seed length
        /// </summary>
        public const int SEED_MAX_LENGTH = 81;

        /// <summary>
        /// This string represents an empty hash consisting of '9'
        /// </summary>
        public const string EMPTY_HASH = "999999999999999999999999999999999999999999999999999999999999999999999999999999999";

        /// <summary>
        /// The length of an address without checksum
        /// </summary>
        public const int ADDRESSLENGTH_WITHOUT_CHECKSUM = 81;

        /// <summary>
        /// The length of an address with checksum
        /// </summary>
        public const int ADDRESSLENGTH_WITH_CHECKSUM = 90;

        /// <summary>
        /// The radix of Iotas trinary system
        /// </summary>
        public const int RADIX = 3;

        /// <summary>
        /// The maximum value a trit can have
        /// </summary>
        public const int MAX_TRIT_VALUE = (RADIX - 1) / 2;

        /// <summary>
        /// The minimum value a trit can have
        /// </summary>
        public const int MIN_TRIT_VALUE = -MAX_TRIT_VALUE;

        /// <summary>
        /// The number of trits in a byte
        /// </summary>
        public const int NUMBER_OF_TRITS_IN_A_BYTE = 5;

        /// <summary>
        /// The number of trits in a tryte
        /// </summary>
        public const int NUMBER_OF_TRITS_IN_A_TRYTE = 3;

        /// <summary>
        /// The minimum weight magnitutde that is considered as save
        /// </summary>
        public const int MIN_WEIGHT_MAGNITUDE = 18;
    }
}