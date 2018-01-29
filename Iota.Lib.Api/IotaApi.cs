using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Iota.Lib.Core;
using Iota.Lib.Exception;
using Iota.Lib.Model;
using Iota.Lib.Utils;

using static Iota.Lib.Utils.Constants;

namespace Iota.Lib
{
    /// <summary>
    /// This class provides all proposed calls and inherits the core calls
    /// </summary>
    public class IotaApi : IotaCoreApi
    {
        Kerl kerl = new Kerl();
        const int MAX_KEY_INDEX = 10; //The default maximum for generating addresses; Increasing this value can decrease performance because more addresses have to be queried

        /// <summary>
        /// Creates an api object that can interact with a specific Node
        /// </summary>
        /// <param name="host">The host address</param>
        /// <param name="port">The port</param>
        public IotaApi(string host, int port, bool ssl) : base(host, port, ssl)
        {
            kerl = new Kerl();
        }

        /// <summary>
        /// Gets possible inputs of a seed
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="start">Starting key index</param>
        /// <param name="end">Ending key index</param>
        /// <param name="securityLevel">The security level. If no security level is specified the value will be 0 and the addresses of all security levels will be queried.</param>
        /// <returns>A list of inputs</returns>
        public List<Transaction> GetInputs(string seed, int start = 0, int end = MAX_KEY_INDEX, int securityLevel = 0)
        {
            InputValidator.CheckIfValidSeed(seed);

            seed = InputValidator.PadSeedIfNecessary(seed);

            if (start > end)
            {
                throw new ArgumentException("Start index must be smaller than end index");
            }

            if (end - start > MAX_KEY_INDEX)
            {
                throw new ArgumentException($"Can't generate more than {MAX_KEY_INDEX} addresses");
            }

            if (securityLevel < 0 || securityLevel > 3)
            {
                throw new ArgumentException("SecurityLevel may only be 0, 1, 2 or 3", "securityLevel");
            }

            List<string> allAddresses = new List<string>();

            for (int keyIndex = start; keyIndex < end; keyIndex++)
            {
                if (securityLevel == 0)
                {
                    for (int seclvlIndex = 0; seclvlIndex < 3; seclvlIndex++)
                    {
                        string address = IotaApiUtils.CreateNewAddress(seed, keyIndex, seclvlIndex, false);
                        allAddresses.Add(address);
                    }
                }
                else
                {
                    string address = IotaApiUtils.CreateNewAddress(seed, keyIndex, securityLevel, false);
                    allAddresses.Add(address);
                }

            }

            return GetBalanceAndFormat(allAddresses, start);
        }

        /// <summary>
        /// Main purpose of this function is to get a list of transfer objects as input, and then prepare the transfer by generating the correct bundle,
        /// as well as choosing and signing the inputs if necessary (if it's a value transfer). The output of this function is list of the raw transaction data (trytes)
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="outputs">The transfers to prepare.</param>
        /// <param name="inputs">List of inputs used for funding the transfer.</param>
        /// <param name="remainderAddress">If defined, this address will be used for sending the remainder value (of the inputs) to.</param>
        /// <returns>A list containing the trytes of the new bundle.</returns>
        public List<string> PrepareTransfers(string seed, List<Transaction> outputs, List<Transaction> inputs = null, string remainderAddress = null)
        {
            InputValidator.CheckTransferArray(outputs);

            Bundle bundle = new Bundle();
            List<string> signatureFragments = new List<string>();
            int signatureMessageLength = 1;

            //  Iterate over all transfers, get totalValue
            //  and prepare the signatureFragments, message and tag
            foreach(Transaction transaction in outputs)
            {
                signatureFragments = GetSignatureFragments(transaction, ref signatureMessageLength);

                // Add first entries to the bundle
                // Slice the address in case the user provided a checksummed one
                bundle.AddEntry(signatureMessageLength, transaction);
            }

            if (bundle.TotalValue != 0)
            {
                if (inputs != null && inputs.Count > 0)
                {
                    List<Transaction> confirmedInputs = ValidateInputs(inputs, bundle);
                    
                    return AddRemainder(seed, confirmedInputs, bundle, String.Empty, bundle.TotalValue, remainderAddress, signatureFragments);
                }
                else
                {
                    List<Transaction> transactionList = GetInputs(seed);
                    return AddRemainder(seed, transactionList, bundle, String.Empty, bundle.TotalValue, remainderAddress, signatureFragments);
                }
            }
            else
            {
                bundle.FinalizeBundle();
                bundle.AddTrytes(signatureFragments);

                List<String> bundleTrytes = new List<string>();
                bundle.Transactions.ForEach(tx => bundleTrytes.Add(tx.ToTransactionTrytes()));

                bundleTrytes.Reverse();
                return bundleTrytes;
            }
        }

        /// <summary>
        /// Generates a new address from a seed and returns the remainderAddress. This is either done deterministically, or by providing the index of the new remainderAddress 
        /// </summary>
        /// <param name="seed">Tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="index">Optional (default null). Key index to start search from. If the index is provided, the generation of the address is not deterministic.</param>
        /// <param name="checksum">Optional (default false). Adds 9-tryte address checksum</param>
        /// <param name="total">Optional (default 1)Total number of addresses to generate.</param>
        /// <param name="returnAll">If true, it returns all addresses which were deterministically generated (until findTransactions returns null)</param>
        /// <returns>an array of strings with the specifed number of addresses</returns>
        public string[] GetNewAddress(string seed, int index = 0, int securityLevel = 0, bool checksum = false, int total = 0, bool returnAll = false)
        {
            //Validate all parameters

            List<string> allAdresses = new List<string>();

            // Case 1: total
            //
            // If total number of addresses to generate is supplied, simply generate
            // and return the list of all addresses
            if (total > 0)
            {
                // Increase index with each iteration
                for (int i = index; i < index + total; i++)
                {
                    allAdresses.Add(IotaApiUtils.CreateNewAddress(seed, i, securityLevel, checksum));
                }

                return allAdresses.ToArray();
            }

            //  Case 2: no total provided
            //
            //  Continue calling findTransactions to see if address was already created
            //  if null, return list of addresses
            //
            else
            {
                List<string> addresses = new List<string>();

                for (int i = index; ; i++)
                {
                    string newAddress = IotaApiUtils.CreateNewAddress(seed, i, 2, checksum);
                    FindTransactionsResponse response = FindTransactionsByAddresses(newAddress);

                    if (returnAll)
                    {
                        addresses.Add(newAddress);
                    }

                    if (response.Hashes.Count == 0)
                        break;
                }

                return addresses.ToArray();
            }
        }

        /// <summary>
        /// Gets the transfers which are associated with a seed. 
        /// The transfers are determined by either calculating deterministically which addresses were already used, 
        /// or by providing a list of indexes to get the transfers from.
        /// </summary>
        /// <param name="seed">tryte-encoded seed. It should be noted that this seed is not transferred</param>
        /// <param name="inclusionStates">If True, it gets the inclusion states of the transfers.</param>
        /// <param name="start">the address start index</param>
        /// <param name="end">the address end index</param>
        /// <returns>An Array of Bundle object that represent the transfers</returns>
        public Bundle[] GetTransfers(string seed, int? start, int? end, bool inclusionStates = false)
        {
            InputValidator.CheckIfValidSeed(seed);
            seed = InputValidator.PadSeedIfNecessary(seed);

            if (!start.HasValue)
                start = 0;
            if (!end.HasValue)
                end = 0;

            // If start value bigger than end, return error
            // or if difference between end and start is bigger than 500 keys
            if (start.Value > end.Value || end.Value > (start + 500))
            {
                throw new System.Exception("Invalid inputs provided: start, end");
            }

            // first call findTransactions
            // If a transaction is non tail, get the tail transactions associated with it
            // add it to the list of tail transactions

            string[] addresses = GetNewAddress(seed, start.Value, 2, false,
                end.HasValue ? end.Value : end.Value - start.Value, true);


            Bundle[] bundles = BundlesFromAddresses(addresses, inclusionStates);
            return bundles;
        }

        /// <summary>
        /// Finds the transaction objects.
        /// </summary>
        /// <param name="adresses">The adresses.</param>
        /// <returns>a list of transactions</returns>
        public List<Transaction> FindTransactionObjects(string[] adresses)
        {
            FindTransactionsResponse ftr = FindTransactions(adresses.ToList(), null, null, null);
            if (ftr == null || ftr.Hashes == null)
                return null;

            // get the transaction objects of the transactions
            return GetTransactionsObjects(ftr.Hashes.ToArray());
        }

        /// <summary>
        /// Gets the transactions objects.
        /// </summary>
        /// <param name="hashes">The hashes in trytes</param>
        /// <returns>a list of transactions</returns>
        public List<Transaction> GetTransactionsObjects(string[] hashes)
        {
            if (!InputValidator.IsArrayOfHashes(hashes))
            {
                throw new IllegalStateException("Not an Array of Hashes: " + hashes.ToString());
            }

            GetTrytesResponse trytesResponse = GetTrytesAsync(hashes).Result;

            List<Transaction> transactions = new List<Transaction>();

            foreach (string tryte in trytesResponse.Trytes)
            {
                transactions.Add(new Transaction(tryte));
            }
            return transactions;
        }

        /// <summary>
        /// Finds the transaction objects by bundle.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a list of Transaction objects</returns>
        public List<Transaction> FindTransactionObjectsByBundle(string[] bundles)
        {
            FindTransactionsResponse ftr = FindTransactions(null, null, null, bundles.ToList());
            if (ftr == null || ftr.Hashes == null)
                return null;

            // get the transaction objects of the transactions
            return GetTransactionsObjects(ftr.Hashes.ToArray());
        }

        /// <summary>
        /// Replays the bundle.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude.</param>
        /// <returns>an array of boolean that indicate which transactions have been replayed successfully</returns>
        public bool[] ReplayBundle(string transaction, int depth, int minWeightMagnitude)
        {
            //StopWatch stopWatch = new StopWatch();

            List<string> bundleTrytes = new List<string>();

            Bundle bundle = GetBundle(transaction)[0];

            bundle.Transactions.ForEach((t) => bundleTrytes.Add(t.ToTransactionTrytes()));

            List<Transaction> transactions = SendTrytes(bundleTrytes, depth).ToList();

            bool[] successful = new bool[transactions.Count];

            for (int i = 0; i < transactions.Count; i++)
            {
                FindTransactionsResponse response = FindTransactionsByBundles(transactions[i].Bundle);
                successful[i] = response.Hashes.Count != 0;
            }

            return successful;
        }

        /// <summary>
        /// Finds the transactions by bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByBundles(params string[] bundles)
        {
            return FindTransactions(null, null, null, bundles.ToList());
        }

        /// <summary>
        /// Finds the transactions by approvees.
        /// </summary>
        /// <param name="approvees">The approvees.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByApprovees(params string[] approvees)
        {
            return FindTransactions(null, null, approvees.ToList(), null);
        }

        /// <summary>
        /// Finds the transactions by digests.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByDigests(params string[] bundles)
        {
            return FindTransactions(null, bundles.ToList(), null, null);
        }

        /// <summary>
        /// Finds the transactions by addresses.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <returns>a FindTransactionsResponse containing the transactions, see <see cref="FindTransactionsResponse"/></returns>
        public FindTransactionsResponse FindTransactionsByAddresses(params string[] addresses)
        {
            return FindTransactions(addresses.ToList(), null, null, null);
        }

        /// <summary>
        /// Gets the latest inclusion.
        /// </summary>
        /// <param name="hashes">The hashes.</param>
        /// <returns>a GetInclusionStatesResponse cotaining the inclusion state of the specified hashes</returns>
        public GetInclusionStatesResponse GetLatestInclusion(string[] hashes)
        {
            string[] latestMilestone = { GetNodeInfo().LatestSolidSubtangleMilestone };
            return GetInclusionStates(hashes, latestMilestone);
        }

        /// <summary>
        /// Wrapper function that basically does prepareTransfers, as well as attachToTangle and finally, it broadcasts and stores the transactions locally.
        /// </summary>
        /// <param name="seed">tryte-encoded seed</param>
        /// <param name="depth">depth</param>
        /// <param name="minWeightMagnitude">The minimum weight magnitude</param>
        /// <param name="transactions">Array of transfer objects</param>
        /// <param name="inputs">Optional (default null). List of inputs used for funding the transfer</param>
        /// <param name="address">Optional (default null). If defined, this address will be used for sending the remainder value (of the inputs) to</param>
        /// <returns> an array of the boolean that indicates which Transactions where sent successully</returns>
        public List<bool> SendTransfer(string seed, int depth, int minWeightMagnitude, List<Transaction> outputs, List<Transaction> inputs = null, string address = null)
        {
            List<string> trytes = PrepareTransfers(seed, outputs, inputs, address);
            List<Transaction> finalizedTransactions = SendTrytes(trytes, depth);

            List<bool> successful = new List<bool>();

            for (int i = 0; i < finalizedTransactions.Count; i++)
            {
                FindTransactionsResponse response = FindTransactionsByBundles(finalizedTransactions[i].Bundle);

                successful[i] = response.Hashes.Count != 0;
            }

            return successful;
        }

        /// <summary>
        /// Sends the trytes.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        /// <param name="depth">The depth.</param>
        /// <returns>an Array of Transactions</returns>
        public List<Transaction> SendTrytes(List<string> trytes, int depth)
        {
            GetTransactionsToApproveResponse transactionsToApproveResponse = GetTransactionsToApprove(depth);

            AttachToTangleResponse attachToTangleResponse = AttachToTangle(transactionsToApproveResponse.TrunkTransaction, transactionsToApproveResponse.BranchTransaction, trytes);
            try
            {
                BroadcastAndStore(attachToTangleResponse.Trytes);
            }
            catch (System.Exception)
            {
                return new List<Transaction>();
            }

            List<Transaction> transactions = new List<Transaction>();

            foreach (string tx in attachToTangleResponse.Trytes)
            {
                transactions.Add(new Transaction(tx));
            }
            return transactions;
        }

        /// <summary>
        /// This function returns the bundle which is associated with a transaction. Input can by any type of transaction (tail and non-tail). 
        /// If there are conflicting bundles (because of a replay for example) it will return multiple bundles. 
        /// It also does important validation checking (signatures, sum, order) to ensure that the correct bundle is returned.
        /// </summary>
        /// <param name="transaction">The transaction encoded in trytes.</param>
        /// <returns>An list of bundles. If there are multiple bundles it means that there are conflicting bundles.</returns>
        public List<Bundle> GetBundle(string transaction)
        {
            Bundle bundle = TraverseBundle(transaction, null, new Bundle());

            if (bundle == null)
                throw new ArgumentException("Unknown Bundle");

            BigInteger totalSum = 0;
            string bundleHash = bundle.Transactions[0].Bundle;

            kerl.Reset();

            List<Signature> signaturesToValidate = new List<Signature>();

            for (int index = 0; index < bundle.Transactions.Count; index++)
            {
                Transaction bundleTransaction = bundle.Transactions[index];
                totalSum += bundleTransaction.Value;

                if (bundleTransaction.CurrentIndex != index)
                    throw new InvalidBundleException("The index of the bundle " + bundleTransaction.CurrentIndex + " did not match the expected index " + index);

                // Get the transaction trytes
                string thisTxTrytes = bundleTransaction.ToTransactionTrytes().Substring(2187, 162);

                // Absorb bundle hash + value + timestamp + lastIndex + currentIndex trytes.
                kerl.Absorb(Converter.ConvertTrytesToTrits(thisTxTrytes));

                // Check if input transaction
                if (bundleTransaction.Value < 0)
                {
                    string address = bundleTransaction.Address;
                    Signature sig = new Signature();
                    sig.Address = address;
                    sig.SignatureFragments.Add(bundleTransaction.SignatureMessageFragment);

                    // Find the subsequent txs with the remaining signature fragment
                    for (int i = index; i < bundle.Length - 1; i++)
                    {
                        var newBundleTx = bundle.Transactions[i + 1];

                        // Check if new tx is part of the signature fragment
                        if (newBundleTx.Address == address && newBundleTx.Value == 0)
                        {
                            sig.SignatureFragments.Add(newBundleTx.SignatureMessageFragment);
                        }
                    }

                    signaturesToValidate.Add(sig);
                }
            }

            // Check for total sum, if not equal 0 return error
            if (totalSum != 0)
                throw new InvalidBundleException("Invalid Bundle Sum");

            int[] bundleFromTrxs = new int[243];
            bundleFromTrxs = kerl.Squeeze(243);
            string bundleFromTxString = Converter.ConvertTritsToTrytes(bundleFromTrxs);

            // Check if bundle hash is the same as returned by tx object
            if (!bundleFromTxString.Equals(bundleHash))
            {
                throw new InvalidBundleException("Invalid Bundle Hash");
            }
                
            if (!bundle.Transactions[bundle.Length - 1].CurrentIndex.Equals(bundle.Transactions[bundle.Length - 1].LastIndex))
            {
                throw new InvalidBundleException("Invalid Bundle");
            }
             
            // Validate the signatures
            foreach (Signature aSignaturesToValidate in signaturesToValidate)
            {
                String[] signatureFragments = aSignaturesToValidate.SignatureFragments.ToArray();
                string address = aSignaturesToValidate.Address;
                bool isValidSignature = Signing.ValidateSignatures(address, signatureFragments, bundleHash);

                if (!isValidSignature)
                    throw new InvalidSignatureException();
            }

            return new List<Bundle>();
        }

        /// <summary>
        /// Wrapper function that broadcasts and stores the specified trytes
        /// </summary>
        /// <param name="trytes">trytes</param>
        public void BroadcastAndStore(List<string> trytes)
        {
            BroadcastTransactions(trytes);
            StoreTransactions(trytes);
        }

        private Bundle TraverseBundle(string trunkTransaction, string bundleHash, Bundle bundle)
        {
            GetTrytesResponse gtr = GetTrytes(trunkTransaction);

            if (gtr.Trytes.Count == 0)
                throw new InvisibleBundleTransactionException();

            string trytes = gtr.Trytes[0];

            Transaction transaction = new Transaction(trytes);

            // If first transaction to search is not a tail, return error
            if (bundleHash == null && transaction.CurrentIndex != 0)
            {
                throw new InvalidTailTransactionException();
            }

            // If no bundle hash, define it
            if (bundleHash == null)
            {
                bundleHash = transaction.Bundle;
            }

            // If different bundle hash, return with bundle
            if (bundleHash != transaction.Bundle)
            {
                return bundle;
            }

            // If only one bundle element, return
            if (transaction.LastIndex == 0 && transaction.CurrentIndex == 0)
            {
                return new Bundle();
            }

            // Define new trunkTransaction for search
            var trunkTx = transaction.TrunkTransaction;

            // Add transaction object to bundle
            bundle.Transactions.Add(transaction);

            // Continue traversing with new trunkTx
            return TraverseBundle(trunkTx, bundleHash, bundle);
        }

        private Bundle[] BundlesFromAddresses(string[] addresses, bool inclusionStates)
        {
            List<Transaction> trxs = FindTransactionObjects(addresses);
            // set of tail transactions
            List<string> tailTransactions = new List<string>();
            List<string> nonTailBundleHashes = new List<string>();

            foreach (Transaction trx in trxs)
            {
                // Sort tail and nonTails
                if (trx.CurrentIndex == 0)
                {
                    tailTransactions.Add(trx.Hash);
                }
                else
                {
                    if (nonTailBundleHashes.IndexOf(trx.Bundle) == -1)
                    {
                        nonTailBundleHashes.Add(trx.Bundle);
                    }
                }
            }

            List<Transaction> bundleObjects = FindTransactionObjectsByBundle(nonTailBundleHashes.ToArray());
            foreach (Transaction trx in bundleObjects)
            {
                // Sort tail and nonTails
                if (trx.CurrentIndex == 0)
                {
                    if (tailTransactions.IndexOf(trx.Hash) == -1)
                    {
                        tailTransactions.Add(trx.Hash);
                    }
                }
            }

            List<Bundle> finalBundles = new List<Bundle>();
            string[] tailTxArray = tailTransactions.ToArray();

            // If inclusionStates, get the confirmation status
            // of the tail transactions, and thus the bundles
            GetInclusionStatesResponse gisr = null;
            if (inclusionStates)
            {
                try
                {
                    gisr = GetLatestInclusion(tailTxArray);
                }
                catch (IllegalAccessException)
                {
                    // suppress exception (the check is done below)
                }
                if (gisr == null || gisr.States == null || gisr.States.Count == 0)
                    throw new ArgumentException("Inclusion states not found");
            }


            GetInclusionStatesResponse finalInclusionStates = gisr;

            Parallel.ForEach(tailTransactions, (param) =>
            {
                try
                {
                    Bundle b = GetBundle(param)[0];

                    if (inclusionStates)
                    {
                        bool thisInclusion = finalInclusionStates.States[tailTxArray.ToList().IndexOf(param)];
                        foreach (Transaction t in b.Transactions)
                        {
                            //t.Persistance = thisInclusion;
                        }
                    }
                    finalBundles.Add(b);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Bundle error: " + ex.Message);
                }
            });

            finalBundles.Sort();
            Bundle[] returnValue = new Bundle[finalBundles.Count];
            for (int i = 0; i < finalBundles.Count; i++)
            {
                returnValue[i] = new Bundle(finalBundles[i].Transactions);
            }
            return returnValue;
        }

        /// <summary>
        /// Adds an input with the provided remainder address to the bundle.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="inputs">The inputs.</param>
        /// <param name="bundle">The bundle.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="totalValue">The total value.</param>
        /// <param name="remainderAddress">The address which the remainding balance shall be transfered to.</param>
        /// <param name="signatureFragments">The signature fragments.</param>
        /// <returns>The new list of raw transaction the updated bundle contains.</returns>
        private List<string> AddRemainder(string seed, List<Transaction> inputs, Bundle bundle, string tag, BigInteger totalValue, string remainderAddress, List<string> signatureFragments)
        {
            BigInteger totalTransferValue = totalValue;

            foreach (Transaction input in inputs)
            {
                bundle.AddEntry(2, new Transaction(input.Address));
                // If there is a remainder value
                // Add extra output to send remaining funds to

                if (input.Value >= totalTransferValue)
                {
                    var remainder = input.Value - totalTransferValue;

                    // If user has provided remainder address
                    // Use it to send remaining funds to
                    if (remainder > 0 && remainderAddress != null)
                    {
                        // Remainder bundle entry
                        bundle.AddEntry(1, new Transaction(remainderAddress, remainder, tag));

                        // function for signing inputs
                        IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments);
                    }
                    else if (remainder > 0)
                    {
                        // Generate a new Address by calling getNewAddress
                        string address = GetNewAddress(seed)[0];

                        // Remainder bundle entry
                        bundle.AddEntry(1, new Transaction(address, remainder, tag));

                        // function for signing inputs
                        return IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments);
                    }
                    else
                    {
                        // If there is no remainder, do not add transaction to bundle
                        // simply sign and return
                        return IotaApiUtils.SignInputsAndReturn(seed, inputs, bundle, signatureFragments);
                    }
                }
                // If multiple inputs provided, subtract the totalTransferValue by
                // the inputs balance
                else
                {
                    totalTransferValue -= input.Value;
                }
            }

            throw new NotEnoughBalanceException(totalValue);
        }

        /// <summary>
        /// Gets the balances of the specified addresses and calculates the total balance till the threshold is reached.
        /// </summary>
        /// <param name="addresses">The addresses</param>
        /// <param name="start">The start index.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns>A list of transactions</returns>
        /// <exception cref="NotEnoughBalanceException">Thrown if threshold exceeds the sum of balance of the specified addresses</exception>
        private List<Transaction> GetBalanceAndFormat(List<string> addresses, int start, int threshold = 100)
        {
            GetBalancesResponse getBalancesResponse = GetBalancesAsync(addresses, threshold).Result;

            List<long> balances = getBalancesResponse.Balances;

            List<Transaction> transactions = new List<Transaction>(); 

            for (int i = 0; i < addresses.Count; i++)
            {
                if (balances[i] > 0)
                {
                    transactions.Add(new Transaction()
                    {
                        Address = addresses[i],
                        Value = balances[i],
                        CurrentIndex = start + i
                    });

                    if (IotaApiUtils.GetTotalBalance(transactions) >= threshold)
                    {
                        return transactions;
                    }
                }
            }

            throw new NotEnoughBalanceException();
        }

        private List<string> GetSignatureFragments(Transaction transaction, ref int signatureMessageLength)
        {
            List<string> signatureFragments = new List<string>();

            // If message longer than 2187 trytes, increase signatureMessageLength (add 2nd transaction)
            if (transaction.SignatureMessageFragment.Length > SIGNATURE_MESSAGE_LENGTH)
            {
                // Get total length, message / maxLength (2187 trytes)
                signatureMessageLength += (int)Math.Floor(((decimal)transaction.SignatureMessageFragment.Length / SIGNATURE_MESSAGE_LENGTH));

                var msgCopy = transaction.SignatureMessageFragment;

                // While there is still a message, copy it
                while (msgCopy != null)
                {
                    var fragment = msgCopy.Substring(0, SIGNATURE_MESSAGE_LENGTH > msgCopy.Length ? msgCopy.Length : SIGNATURE_MESSAGE_LENGTH);
                    msgCopy = msgCopy.Substring(SIGNATURE_MESSAGE_LENGTH, msgCopy.Length - SIGNATURE_MESSAGE_LENGTH);

                    // Pad remainder of fragment
                    for (var j = 0; fragment.Length < SIGNATURE_MESSAGE_LENGTH; j++)
                    {
                        fragment += '9';
                    }

                    signatureFragments.Add(fragment);
                }
            }
            else
            {
                // Else, get single fragment with 2187 of 9's trytes
                string fragment = String.Empty;

                if (transaction.SignatureMessageFragment != null)
                {
                    fragment = transaction.SignatureMessageFragment.Substring(0, transaction.SignatureMessageFragment.Length < SIGNATURE_MESSAGE_LENGTH ? transaction.SignatureMessageFragment.Length : SIGNATURE_MESSAGE_LENGTH);
                }

                for (var j = 0; fragment.Length < SIGNATURE_MESSAGE_LENGTH; j++)
                {
                    fragment += '9';
                }

                signatureFragments.Add(fragment);
            }

            return signatureFragments;
        }

        /// <summary>
        /// Checks if the inputs combined balance is not larger than the bundels total balance.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="bundle">The bundle.</param>
        /// <returns>All valid inputs.</returns>
        private List<Transaction> ValidateInputs(List<Transaction> inputs, Bundle bundle)
        {
            List<string> inputAddresses = new List<string>();
            foreach(Transaction input in inputs)
            {
                inputAddresses.Add(input.Address);
            }

            GetBalancesResponse balances = GetBalancesAsync(inputAddresses).GetAwaiter().GetResult();

            List<Transaction> confirmedInputs = new List<Transaction>();

            BigInteger totalBalance = 0;
            for (var i = 0; i < balances.Balances.Count; i++)
            {
                var thisBalance = balances.Balances[i];
                totalBalance += thisBalance;
                
                if (thisBalance > 0)
                {
                    var inputEl = inputs[i];
                    inputEl.Value = thisBalance;

                    confirmedInputs.Add(inputEl);
                }
            }
            
            if (bundle.TotalValue > totalBalance)
            {
                throw new NotEnoughBalanceException();
            }

            return confirmedInputs;
        }
    }
}