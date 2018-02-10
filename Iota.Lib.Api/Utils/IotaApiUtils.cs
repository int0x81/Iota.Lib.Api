using System;
using System.Collections.Generic;
using Iota.Lib.Model;
using System.Runtime.CompilerServices;
using System.Numerics;
using System.Threading.Tasks;
using static Iota.Lib.Utils.Constants;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// Provides several methods used in the Iota API
    /// </summary>
    public static class IotaApiUtils
    {
        /// <summary>
        /// Creates a new address
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="index">The address index</param>
        /// <param name="securityLevel">The security level</param>
        /// <param name="checksum">Indicates if the address shall contain a checksum</param>
        /// <returns>A tryte-string that represents an address</returns>
        public static string CreateNewAddress(string seed, int index, int securityLevel, bool checksum)
        {
            int[] privateKey = Signing.Key(Converter.ConvertTrytesToTrits(seed), index, securityLevel);
            int[] digests = Signing.Digests(privateKey);
            int[] addressInTrits = Signing.Address(digests);
            string address = Converter.ConvertTritsToTrytes(addressInTrits);

            return checksum ? Checksum.AddChecksum(address) : address;
        }

        /// <summary>
        /// Takes a bundle and signs all transaction with input (negative value). At this point the meta transactions already have to be generated into the bundle
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="bundle">The bundle containing all outputs, inputs and the needed meta transactions</param>
        /// <returns>The bundle with signed inputs</returns>
        public static Bundle SignInputsAndReturn(string seed, Bundle bundle)
        {
            Kerl kerl = new Kerl();

            bundle.FinalizeBundle();

            for (int i = 0; i < bundle.Transactions.Count; i++)
            {
                if (bundle.Transactions[i].Value < 0)
                {
                    string thisAddress = bundle.Transactions[i].Address;

                    string bundleHash = bundle.Transactions[i].Bundle;

                    int[] key = Signing.Key(Converter.ConvertTrytesToTrits(seed), bundle.Transactions[i].KeyIndex, bundle.Transactions[i].SecurityLevel);

                    int[] firstFragment = ArrayUtils.CreateSubArray(key, 0, Constants.KEY_LENGTH);

                    int[] normalizedBundleHash = bundle.NormalizeBundle(bundleHash);

                    int[] firstBundleFragment = ArrayUtils.CreateSubArray(normalizedBundleHash, 0, 27);

                    int[] firstSignedFragment = Signing.SignatureFragment(firstBundleFragment, firstFragment);

                    bundle.Transactions[i].SignatureMessageFragment = Converter.ConvertTritsToTrytes(firstSignedFragment);

                    for (int j = 1; j < bundle.Transactions[i].SecurityLevel; j++)
                    {
                        if (bundle.Transactions[i + j].Address.Equals(bundle.Transactions[i].Address) && bundle.Transactions[i + j].Value == 0)
                        {
                            int[] nextFragment = ArrayUtils.CreateSubArray(key, KEY_LENGTH * j, KEY_LENGTH);

                            int[] secondBundleFragment = ArrayUtils.CreateSubArray(normalizedBundleHash, 27 * j, 27);

                            int[] secondSignedFragment = Signing.SignatureFragment(secondBundleFragment, nextFragment);

                            bundle.Transactions[i + j].SignatureMessageFragment = Converter.ConvertTritsToTrytes(secondSignedFragment);
                        }
                    }
                }
            }

            return bundle;
        }
 
        /// <summary>
        /// Gets the balance of multiple transactions combined
        /// </summary>
        /// <param name="transactions">The transactions</param>
        /// <returns>The total balance</returns>
        public static BigInteger GetTotalBalance(List<Transaction> transactions)
        {
            BigInteger totalBalance = 0;
            foreach(Transaction transaction in transactions)
            {
                totalBalance += transaction.Value;
            }
            return totalBalance;
        }

        /// <summary>
        /// Gets the addresses from multiple transactions
        /// </summary>
        /// <param name="transactions">The transactions</param>
        /// <returns>An IEnumerable containing the addresses</returns>
        public static IEnumerable<string> GetAddressesFromTransactions(IEnumerable<Transaction> transactions)
        {
            foreach(Transaction transaction in transactions)
            {
                yield return transaction.Address;
            }
        }

        /// <summary>
        /// Creates a UNIX timestamp
        /// </summary>
        /// <returns>A long that represents a timestamp</returns>
        internal static long CreateTimeStampNow()
        {
            return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}