using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iota.Lib.Model;
using static Iota.Lib.Utils.Constants;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// This class provides several methods to validate inputs
    /// </summary>
    public static class InputValidator
    {
        /// <summary>
        /// Determines whether the specified string is an address
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns>
        /// The state as boolean if the provided address is indeed an address
        /// </returns>
        public static bool IsValidAddress(string address)
        {
            if (address.Length == ADDRESSLENGTH_WITHOUT_CHECKSUM || address.Length == ADDRESSLENGTH_WITH_CHECKSUM)
            {
                return IsStringOfTrytes(address);
            }
            return false;
        }

        /// <summary>
        /// Determines whether an array only contains valid addresses
        /// </summary>
        /// <param name="addresses">The addresses</param>
        /// <returns>
        /// The state as boolean if the provided array only contains addresses
        /// </returns>
        public static bool IsArrayOfValidAddress(IEnumerable<string> addresses)
        {
            return addresses.ToList().TrueForAll(address => IsValidAddress(address));
        }

        /// <summary>
        /// Determines whether the specified array contains only contains valid transaction hashes
        /// </summary>
        /// <param name="hashes">The hashes</param>
        /// <returns>
        /// The state as boolean if the provided array only contains valid hashes
        /// </returns>
        public static bool IsArrayOfValidTransactionHashes(IEnumerable<string> hashes)
        {
            if (hashes == null)
            {
                return false;
            }

            foreach (string hash in hashes)
            {
                if (hash.Length != Constants.TRANSACTION_HASH_LENGTH || !IsStringOfTrytes(hash))
                {
                    return false; 
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string contains only characters from the trytes alphabet<see cref="Constants.TRYTE_ALPHABET"/>
        /// </summary>
        /// <param name="trytes">The trytes</param>
        /// <returns>
        /// The state as boolean if the provided string only contains valid trytes
        /// </returns>
        public static bool IsStringOfTrytes(string trytes)
        {
            string regex = "^([9A-Z])+$";
            var regexTrytes = new Regex(regex);
            return regexTrytes.IsMatch(trytes);
        }

        /// <summary>
        /// Determines whether the specified array only contains string of trytes
        /// </summary>
        /// <param name="trytes">The trytes</param>
        /// <returns>
        /// The state as boolean if the provided array only contains valid trytes
        /// </returns>
        public static bool IsArrayOfTrytes(IEnumerable<string> trytes)
        {
           return trytes.ToList().TrueForAll(element => IsStringOfTrytes(element));
        }

        /// <summary>
        /// Determines whether a list only contains valid transactions
        /// </summary>
        /// <param name="transfers">The transactions</param>
        /// <returns>
        /// The state as boolean if the provided array only contains valid transactions
        /// </returns>
        public static bool IsArrayOfValidTransactions(IEnumerable<Transaction> transactions)
        {
            return transactions.ToList().TrueForAll(element => IsValidTransaction(element));
        }

        /// <summary>
        /// Determines whether the specified transaction is valid.
        /// </summary>
        /// <param name="transaction">The transfer.</param>
        /// <returns>
        /// The state as boolean if the provided transaction is valid
        /// </returns>
        public static bool IsValidTransaction(Transaction transaction)
        {
            if (!IsValidAddress(transaction.Address))
            {
                return false;
            }

            if (!IsStringOfTrytes(transaction.SignatureMessageFragment))
            {
                return false;
            }

            if (transaction.CurrentIndex > transaction.LastIndex)
            {
                return false;
            }

            if (transaction.TrunkTransaction.Length != TRANSACTION_HASH_LENGTH)
            {
                return false;
            }

            if (transaction.BranchTransaction.Length != TRANSACTION_HASH_LENGTH)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines wether the seed is valid.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <returns>The state as boolean if the provided seed is valid</returns>
        public static bool IsValidSeed(string seed)
        {
            if (!IsStringOfTrytes(seed))
            {
                return false;
            }

            if (seed.Length > SEED_MAX_LENGTH)
            {
                return false;
            }
            return true;
        }
    }
}