using System;
using System.Collections.Generic;
using Iota.Lib.Model;
using System.Runtime.CompilerServices;
using System.Numerics;

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

        public static List<string> SignInputsAndReturn(string seed, List<Transaction> inputs, Bundle bundle, List<string> signatureFragments)
        {
            bundle.FinalizeBundle();
            bundle.AddTrytes(signatureFragments);

            //  SIGNING OF INPUTS
            //
            //  Here we do the actual signing of the inputs
            //  Iterate over all bundle transactions, find the inputs
            //  Get the corresponding private key and calculate the signatureFragment
            for (int i = 0; i < bundle.Transactions.Count; i++)
            {
                if (bundle.Transactions[i].Value < 0)
                {
                    string thisAddress = bundle.Transactions[i].Address;

                    // Get the corresponding keyIndex of the address
                    int keyIndex = 0;
                    foreach (Transaction input in inputs)
                    {
                        if (input.Address.Equals(thisAddress))
                        {
                            keyIndex = input.CurrentIndex;
                            break;
                        }
                    }

                    string bundleHash = bundle.Transactions[i].Bundle;

                    // Get corresponding private key of address
                    int[] key = Signing.Key(Converter.ConvertTrytesToTrits(seed), keyIndex, 2);

                    //  First 6561 trits for the firstFragment
                    int[] firstFragment = ArrayUtils.CreateSubArray(key, 0, 6561);

                    //  Get the normalized bundle hash
                    int[] normalizedBundleHash = bundle.NormalizedBundle(bundleHash);

                    //  First bundle fragment uses 27 trytes
                    int[] firstBundleFragment = ArrayUtils.CreateSubArray(normalizedBundleHash, 0, 27);

                    //  Calculate the new signatureFragment with the first bundle fragment
                    int[] firstSignedFragment = Signing.SignatureFragment(firstBundleFragment, firstFragment);

                    //  Convert signature to trytes and assign the new signatureFragment
                    bundle.Transactions[i].SignatureMessageFragment = Converter.ConvertTritsToTrytes(firstSignedFragment);

                    //  Because the signature is > 2187 trytes, we need to
                    //  find the second transaction to add the remainder of the signature
                    for (int j = 0; j < bundle.Transactions.Count; j++)
                    {
                        //  Same address as well as value = 0 (as we already spent the input)
                        if (bundle.Transactions[j].Address.Equals(thisAddress) &&
                            bundle.Transactions[j].Value == 0)
                        {
                            // Use the second 6562 trits
                            int[] secondFragment = ArrayUtils.CreateSubArray(key, 6561, 6561);

                            // The second 27 to 54 trytes of the bundle hash
                            int[] secondBundleFragment = ArrayUtils.CreateSubArray(normalizedBundleHash, 27, 27);

                            //  Calculate the new signature
                            int[] secondSignedFragment = Signing.SignatureFragment(secondBundleFragment,
                                secondFragment);

                            //  Convert signature to trytes and assign it again to this bundle entry
                            bundle.Transactions[j].SignatureMessageFragment = (Converter.ConvertTritsToTrytes(secondSignedFragment));
                        }
                    }
                }
            }

            List<string> bundleTrytes = new List<string>();

            // Convert all bundle entries into trytes
            foreach (Transaction transaction in bundle.Transactions)
            {
                bundleTrytes.Add(transaction.ToTransactionTrytes());
            }
            bundleTrytes.Reverse();
            return bundleTrytes;
        }

        /// <summary>
        /// Gets the balance of multiple transactions combined.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <returns>The total balance.</returns>
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
        /// Creates a unix timestamp.
        /// </summary>
        /// <returns>An integer that represents a timestamp.</returns>
        internal static long CreateTimeStampNow()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Milliseconds;
        }
    }
}