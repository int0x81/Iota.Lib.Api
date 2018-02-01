using System;
using System.Collections.Generic;
using System.Numerics;
using Iota.Lib.Exception;
using Iota.Lib.Utils;

namespace Iota.Lib.Model
{
    /// <summary>
    /// Represents a bundle.
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/> class without transactions.
        /// </summary>
        public Bundle() : this(new List<Transaction>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/> class
        /// </summary>
        /// <param name="transactions">The transactions</param>
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

        /// <summary>Adds a bundle entry</summary>
        /// <param name="transaction">The transaction</param>
        public void AddEntry(Transaction transaction)
        {
            if(!InputValidator.IsValidAddress(transaction.Address) || transaction.Value == null || !InputValidator.IsStringOfTrytes(transaction.Tag) || transaction.Timestamp == 0)
            {
                throw new InvalidTransactionException();
            }

            Transactions.Add(transaction);
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
        /// Calculates the bundle hash using <see cref="Kerl"/> and fills it into all transactions
        /// </summary>
        public void FinalizeBundle()
        {

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

        /// <summary>
        /// Calculates the bundle hash
        /// </summary>
        /// <returns>The bundle hash</returns>
        public string GetBundleHash()
        {
            Kerl kerl = new Kerl();

            foreach(Transaction transaction in Transactions)
            {
                kerl.Absorb(Converter.ConvertTrytesToTrits(transaction.Address));
                kerl.Absorb(Converter.ConvertBigIntToTrits(transaction.Value));
                kerl.Absorb(Converter.ConvertTrytesToTrits(transaction.ObsoleteTag));
                kerl.Absorb(Converter.ConvertLongToTrits(transaction.Timestamp));
                kerl.Absorb(Converter.ConvertIntegerToTrits(transaction.CurrentIndex));
                kerl.Absorb(Converter.ConvertIntegerToTrits(transaction.LastIndex));
            }

            return Converter.ConvertTritsToTrytes(kerl.Squeeze());
        }

        /// <summary>
        /// Sets the branch- and trunktransaction for each transaction
        /// </summary>
        /// <param name="tip_01">The first tip</param>
        /// <param name="tip_02">The second tip</param>
        private void CreateTail(string tip_01, string tip_02)
        {
            for(int c = 0; c <= Transactions.Count; c++)
            {
                if(c == Transactions.Count)
                {
                    Transactions[c].BranchTransaction = tip_02;
                    Transactions[c].TrunkTransaction = tip_01;
                }
                else
                {
                    Transactions[c].BranchTransaction = tip_01;
                    Transactions[c].TrunkTransaction = Transactions[c + 1].Hash;
                }
            }
        }
    }
}