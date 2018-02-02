using System;
using System.Collections.Generic;
using System.Numerics;
using Iota.Lib.Exception;
using Iota.Lib.Utils;

namespace Iota.Lib.Model
{
    /// <summary>
    /// Represents a bundle
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bundle"/>
        /// </summary>
        public Bundle()
        {
        }

        public List<Transaction> Transactions { get; set; }

        /// <summary>Adds a bundle entry</summary>
        /// <param name="input">The transaction</param>
        public void AddEntry(Transaction input)
        {
            if(!InputValidator.IsValidTransaction(input))
            {
                throw new InvalidTransactionException();
            }

            Transactions.Add(input);
        }

        /// <summary>
        /// Adds the trytes.
        /// </summary>
        /// <param name="signatureFragments">The signature fragments.</param>
        //public void AddTrytes(List<string> signatureFragments)
        //{
        //    string emptySignatureFragment = String.Empty;

        //    for (int j = 0; emptySignatureFragment.Length < Constants.SIGNATURE_MESSAGE_LENGTH; j++)
        //    {
        //        emptySignatureFragment += "9";
        //    }

        //    for (int i = 0; i < Transactions.Count; i++)
        //    {
        //        Transaction transaction = Transactions[i];

        //        // Fill empty signatureMessageFragment
        //        transaction.SignatureMessageFragment = ((signatureFragments.Count <= i || string.IsNullOrEmpty(signatureFragments[i]))
        //            ? emptySignatureFragment
        //            : signatureFragments[i]);
        //        // Fill empty trunkTransaction
        //        transaction.TrunkTransaction = Constants.EMPTY_HASH;

        //        // Fill empty branchTransaction
        //        transaction.BranchTransaction = Constants.EMPTY_HASH;

        //        // Fill empty nonce
        //        transaction.Nonce = Constants.EMPTY_HASH;
        //    }
        //}

        /// <summary>
        /// Normalizes the bundle
        /// </summary>
        /// <param name="bundleHash">The bundle hash</param>
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
            SetIndexes();
            CreateAndAssignBundleHash();
        }

        /// <summary>
        /// Loops through each transaction and adds a metatransaction if the length of the signature exceeds <see cref="Constants.SIGNATURE_MESSAGE_LENGTH"/>
        /// </summary>
        public void SliceSignatures(int securityLevel)
        {
            for (int c = 0; c <= Transactions.Count; c++)
            {
                if (Transactions[c].SignatureMessageFragment.Length % Constants.SIGNATURE_MESSAGE_LENGTH != 0
                    || Transactions[c].SignatureMessageFragment.Length / securityLevel != Constants.SIGNATURE_MESSAGE_LENGTH)
                {
                    throw new InvalidBundleException("Invalid signature message length");
                }

                switch (Transactions[c].SignatureMessageFragment.Length / Constants.SIGNATURE_MESSAGE_LENGTH)
                {
                    case 1:
                        break;
                    case 2:
                        {
                            Transaction metaTransaction = (Transaction)Transactions[0].Clone();
                            metaTransaction.SignatureMessageFragment = Transactions[c].SignatureMessageFragment.Substring(Constants.SIGNATURE_MESSAGE_LENGTH);
                            Transactions.Insert(c + 1, metaTransaction);
                            c++;
                            break;
                        }
                    case 3:
                        {
                            Transaction metaTransaction_01 = (Transaction)Transactions[0].Clone();
                            Transaction metaTransaction_02 = (Transaction)Transactions[0].Clone();
                            metaTransaction_01.SignatureMessageFragment = Transactions[c].SignatureMessageFragment.Substring(Constants.SIGNATURE_MESSAGE_LENGTH);
                            metaTransaction_02.SignatureMessageFragment = Transactions[c].SignatureMessageFragment.Substring(Constants.SIGNATURE_MESSAGE_LENGTH * 2);
                            Transactions.Insert(c + 1, metaTransaction_01);
                            c++;
                            Transactions.Insert(c + 1, metaTransaction_02);
                            c++;
                            break;
                        }
                    default:
                        throw new InvalidBundleException("Invalid signature message length");
                }
            }
        }

        /// <summary>
        /// Sets the branch- and trunktransaction for each transaction
        /// </summary>
        /// <param name="tip_01">The first tip</param>
        /// <param name="tip_02">The second tip</param>
        public void CreateTail(string tip_01, string tip_02)
        {
            for (int c = 0; c <= Transactions.Count; c++)
            {
                if (c == Transactions.Count)
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
        /// Calculates the bundle hash and assigns it to each transaction
        /// </summary>
        private void CreateAndAssignBundleHash()
        {
            Kerl kerl = new Kerl();

            foreach (Transaction transaction in Transactions)
            {
                kerl.Absorb(Converter.ConvertTrytesToTrits(transaction.Address));
                kerl.Absorb(Converter.ConvertBigIntToTrits(transaction.Value));
                kerl.Absorb(Converter.ConvertTrytesToTrits(transaction.ObsoleteTag));
                kerl.Absorb(Converter.ConvertLongToTrits(transaction.Timestamp));
                kerl.Absorb(Converter.ConvertIntegerToTrits(transaction.CurrentIndex));
                kerl.Absorb(Converter.ConvertIntegerToTrits(transaction.LastIndex));
            }

            string bundleHash = Converter.ConvertTritsToTrytes(kerl.Squeeze());
            Transactions.ForEach(tx => tx.Bundle = bundleHash);
        }
        
        /// <summary>
        /// Sets the Indexes of all transactions contained in this bundle
        /// </summary>
        private void SetIndexes()
        {
            for(int c = 0; c < Transactions.Count; c++)
            {
                Transactions[c].CurrentIndex = c;
                Transactions[c].LastIndex = Transactions.Count - 1;
            }
        }
        
    }
}