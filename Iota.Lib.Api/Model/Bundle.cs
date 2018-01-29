using System;
using System.Collections.Generic;
using System.Numerics;
using Iota.Lib.Utils;

namespace Iota.Lib.Model
{
    /// <summary>
    /// Represents a bundle.
    /// </summary>
    public class Bundle : IComparable<Bundle>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/> class without transactions.
        /// </summary>
        public Bundle() : this(new List<Transaction>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/> class.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        public Bundle(List<Transaction> transactions)
        {
            Transactions = transactions;
        }

        /// <summary>
        /// Gets or sets the total value
        /// </summary>
        /// <value>The value of all transactions combined</value>
        public BigInteger TotalValue { get; set; } 

        /// <summary>
        /// Gets or sets the transactions.
        /// </summary>
        /// <value>
        /// The transactions.
        /// </value>
        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// Gets the length of the bundle
        /// </summary>
        /// <value>The length</value>
        public int Length
        {
            get
            {
                return Transactions.Count;
            }
        }

        /// <summary>
        /// Adds a bundle entry
        /// </summary>
        /// <param name="signatureMessageLength">Length of the signature message.</param>
        /// <param name="address">The address.</param>
        /// <param name="value">The value.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="timestamp">The timestamp.</param>
        public void AddEntry(int signatureMessageLength, Transaction transaction)
        {
            for (int i = 0; i < signatureMessageLength; i++)
            {
                TotalValue += transaction.Value;
                Transactions.Add(transaction);
            }
        }

        /// <summary>
        /// Adds the trytes.
        /// </summary>
        /// <param name="signatureFragments">The signature fragments.</param>
        public void AddTrytes(List<string> signatureFragments)
        {
            string emptySignatureFragment = String.Empty;

            for (int j = 0; emptySignatureFragment.Length < Constants.SIGNATURE_MESSAGE_LENGTH; j++)
            {
                emptySignatureFragment += "9";
            }

            for (int i = 0; i < Transactions.Count; i++)
            {
                Transaction transaction = Transactions[i];

                // Fill empty signatureMessageFragment
                transaction.SignatureMessageFragment = ((signatureFragments.Count <= i || string.IsNullOrEmpty(signatureFragments[i]))
                    ? emptySignatureFragment
                    : signatureFragments[i]);
                // Fill empty trunkTransaction
                transaction.TrunkTransaction = Constants.EMPTY_HASH;

                // Fill empty branchTransaction
                transaction.BranchTransaction = Constants.EMPTY_HASH;

                // Fill empty nonce
                transaction.Nonce = Constants.EMPTY_HASH;
            }
        }

        /// <summary>
        /// Normalizeds the bundle.
        /// </summary>
        /// <param name="bundleHash">The bundle hash.</param>
        /// <returns></returns>
        public int[] NormalizedBundle(string bundleHash)
        {
            int[] normalizedBundle = new int[81];

            for (int i = 0; i < 3; i++)
            {
                long sum = 0;
                for (int j = 0; j < 27; j++)
                {
                    sum += (normalizedBundle[i*27 + j] = Converter.ConvertTritsToInteger(Converter.ConvertTrytesToTrits("" + bundleHash[i*27 + j])));
                }

                if (sum >= 0)
                {
                    while (sum-- > 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i*27 + j] > -13)
                            {
                                normalizedBundle[i*27 + j]--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (sum++ < 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i*27 + j] < 13)
                            {
                                normalizedBundle[i*27 + j]++;
                                break;
                            }
                        }
                    }
                }
            }

            return normalizedBundle;
        }

        /// <summary>
        /// Calculates the bundle hash using <see cref="Kerl"/> and fills it into all transactions this bundle has.
        /// </summary>
        public void FinalizeBundle()
        {
            Kerl kerl = new Kerl();
            kerl.Reset();

            for (int i = 0; i < Transactions.Count; i++)
            {
                int[] valueTrits = Converter.ConvertBigIntToTrits(Transactions[i].Value);
                valueTrits = ArrayUtils.PadArrayWithZeros(valueTrits, 81);

                int[] timestampTrits = Converter.ConvertLongToTrits(Transactions[i].Timestamp);
                timestampTrits = ArrayUtils.PadArrayWithZeros(timestampTrits, 27);

                int[] currentIndexTrits = Converter.ConvertIntegerToTrits(Transactions[i].CurrentIndex);
                currentIndexTrits = ArrayUtils.PadArrayWithZeros(currentIndexTrits, 27);

                int[] lastIndexTrits = Converter.ConvertIntegerToTrits(Transactions[i].LastIndex);
                lastIndexTrits = ArrayUtils.PadArrayWithZeros(lastIndexTrits, 27);

                string stringToConvert = Transactions[i].Address
                                         + Converter.ConvertTritsToTrytes(valueTrits)
                                         + Converter.ConvertTritsToTrytes(timestampTrits)
                                         + Converter.ConvertTritsToTrytes(currentIndexTrits)
                                         + Converter.ConvertTritsToTrytes(lastIndexTrits);

                int[] t = Converter.ConvertTrytesToTrits(stringToConvert);
                kerl.Absorb(t, 0, t.Length);
            }

            string hashInTrytes = Converter.ConvertTritsToTrytes(kerl.Squeeze());

            foreach(Transaction transaction in Transactions)
            {
                transaction.Bundle = hashInTrytes;
            }
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
        /// </returns>
        public int CompareTo(Bundle other)
        {
            long timeStamp1 = Transactions[0].Timestamp;
            long timeStamp2 = other.Transactions[0].Timestamp;

            if (timeStamp1 < timeStamp2)
                return -1;
            if (timeStamp1 > timeStamp2)
                return 1;
            return 0;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Transactions)}: {string.Join(",", Transactions)}";
        }
    }
}