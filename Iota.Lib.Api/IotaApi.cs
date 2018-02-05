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
using static Iota.Lib.Utils.IotaApiUtils;

namespace Iota.Lib
{
    /// <summary>
    /// Provides all proposed calls and inherits the core calls
    /// </summary>
    public class IotaApi : IotaCoreApi
    {
        Kerl kerl = new Kerl();
        
        /// <summary>
        /// Creates an api object that can interact with a specific node
        /// </summary>
        /// <param name="host">The host address</param>
        /// <param name="port">The port</param>
        /// <param name="is_ssl">States if the connection you want to establish is using ssl-encryption (https)</param>
        public IotaApi(string host, int port, bool is_ssl) : base(host, port, is_ssl)
        {
        }

        /// <summary>
        /// Gets possible inputs of a seed
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="start">Starting key index</param>
        /// <param name="end">Ending key index. Choosing a high value will result in an io-intense operation</param>
        /// <param name="securityLevel">The security level. If no security level is specified the value will be 0 and the addresses of all security levels will be queried.</param>
        /// <returns>A list of inputs</returns>
        public IEnumerable<string> GetInputs(string seed, int start = 0, int end = 10, int securityLevel = 0)
        {
            if (!InputValidator.IsValidSeed(seed))
            {
                throw new InvalidTryteException();
            }

            seed = ArrayUtils.AdjustTryteString(seed, SEED_MAX_LENGTH);

            if (start > end)
            {
                throw new ArgumentException("Start index must be smaller than end index");
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
                        yield return CreateNewAddress(seed, keyIndex, seclvlIndex, false);
                    }
                }
                else
                {
                    yield return CreateNewAddress(seed, keyIndex, securityLevel, false);
                }
            }
        }

        /// <summary>
        /// Generates a correct bundle containing the desired ouputs and inputs, signs the transactionsand creates the tail. In order to succesfully
        /// send the bundle you still need to perform proof of work
        /// <see href="https://medium.com/@louielu/in-depth-explanation-of-how-iota-making-a-transaction-with-picture-8a638805f905"/>
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="outputs">The transfers to prepare</param>
        /// <param name="securityLevel">The security level of the private key hwich is used to sign the outputs</param>
        /// <param name="inputs">List of inputs used for funding the transfer</param>
        /// <param name="remainderAddress">
        /// If defined, this address will be used for sending the remainder value to.
        /// </param>
        /// <returns>A list of raw transaction data</returns>
        public IEnumerable<string> PrepareTransfers(string seed, List<Transaction> outputs, int securityLevel, List<Transaction> inputs = null, string remainderAddress = null)
        {
            if (!InputValidator.IsValidSeed(seed))
            {
                throw new InvalidTryteException();
            }

            seed = ArrayUtils.AdjustTryteString(seed, SEED_MAX_LENGTH);

            if (!InputValidator.IsArrayOfValidTransactions(outputs))
            {
                throw new InvalidTransactionException();
            }

            if (inputs != null)
            {
                if(!InputValidator.IsArrayOfValidTransactions(inputs))
                {
                    throw new InvalidTransactionException();
                }

                if (string.IsNullOrEmpty(remainderAddress))
                {
                    remainderAddress = GetNewAddresses(seed, inputs[inputs.Count - 1].KeyIndex + 1, 1, securityLevel).ElementAt(0);
                }
            }

            BigInteger remainding = BigInteger.Add(GetTotalBalance(inputs), GetTotalBalance(outputs));


            if(remainding > 0)
            {
                throw new NotEnoughBalanceException();
            }

            var dummy = outputs[0].ToTransactionTrytes();
            var di = dummy.Length;

            Bundle bundle = new Bundle();
            bundle.AddEntries(outputs);
            bundle.AddEntries(inputs);
            bundle.SliceSignatures(securityLevel);
            bundle.AddEntry(new Transaction(remainderAddress, -remainding));
            bundle.FinalizeBundle();
            var tmp = Task.Run(() => SignInputsAndReturn(seed, bundle)).Result;
            var response = GetTransactionsToApproveAsync(SAVE_DEPTH).Result;
            bundle.CreateTail(response.BranchTransaction, response.TrunkTransaction);
            IEnumerable<String> bundleTrytes = bundle.GetRawTransactions();
            bundleTrytes.Reverse();
            return bundleTrytes;  
        }

        /// <summary>
        /// Generates new, unused addresses from a seed 
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="index">Key index to start search from</param>
        /// <param name="total">Total number of addresses to generate</param>
        /// <param name="securityLevel">The security level of the generated addresses</param>
        /// <param name="checksum">States if the returning addresses shall contain a valid checksum</param>
        /// <returns>An IEnumerable containing the addresses</returns>
        public IEnumerable<string> GetNewAddresses(string seed, int index = 0, int total = 1, int securityLevel = 2, bool checksum = false)
        {
            if (!InputValidator.IsValidSeed(seed))
            {
                throw new InvalidTryteException();
            }
            if (securityLevel < 1 || securityLevel > 3)
            {
                throw new ArgumentException("Invalid security level parameter", "securityLevel");
            }
            if (index < 0)
            {
                throw new ArgumentException("Invalid index parameter", "index");
            }

            for(int i = index; i < index + total;)
            {
                string newAddress = CreateNewAddress(seed, i, securityLevel, checksum);
                IEnumerable<Transaction> foundTransactions = FindTransactionsByAddresses(newAddress);
                if(foundTransactions.Count() == 0)
                {
                    yield return newAddress;
                    i++;
                }
            }  
        }

        /// <summary>
        /// This function returns the bundle which is associated with a transaction
        /// </summary>
        /// <param name="rawTransaction">The raw transaction</param>
        /// <returns>The bundle</returns>
        public Bundle GetBundle(string rawTransaction)
        {
            Bundle bundle = TraverseBundle(rawTransaction, null, new Bundle());

            if (bundle == null)
            {
                throw new InvalidBundleException("Bundle can not be null");
            }
            
            BigInteger totalValue = 0;
            string bundleHash = bundle.Transactions[0].Bundle;

            kerl.Reset();

            List<Signature> signaturesToValidate = new List<Signature>();

            for(int index = 0; index < bundle.Transactions.Count; index++)
            {
                Transaction bundleTransaction = bundle.Transactions[index];
                totalValue += bundleTransaction.Value;

                if(bundleTransaction.CurrentIndex != index)
                {
                    throw new InvalidBundleException("The index of the bundle " + bundleTransaction.CurrentIndex + " did not match the expected index " + index);
                }

                kerl.Absorb(Converter.ConvertTrytesToTrits(bundleTransaction.ToTransactionTrytes().Substring(2187, 162)));

                if (bundleTransaction.Value < 0)
                {
                    Signature sig = new Signature
                    {
                        Address = bundleTransaction.Address
                    };
                    sig.SignatureFragments.Add(bundleTransaction.SignatureMessageFragment);

                    for (int i = index; i < bundle.Transactions.Count - 1; i++)
                    {
                        var newBundleTx = bundle.Transactions[i + 1];

                        if (newBundleTx.Address == bundleTransaction.Address && newBundleTx.Value == 0)
                        {
                            sig.SignatureFragments.Add(newBundleTx.SignatureMessageFragment);
                        }
                    }

                    signaturesToValidate.Add(sig);
                }
            }

            string bundleFromTxString = Converter.ConvertTritsToTrytes(kerl.Squeeze());

            if (totalValue != 0 || !bundle.Transactions[bundle.Transactions.Count - 1].CurrentIndex.Equals(bundle.Transactions[bundle.Transactions.Count - 1].LastIndex) || !bundleFromTxString.Equals(bundleHash))
            {
                throw new InvalidBundleException("Invalid Bundle");
            } 

            foreach (Signature signature in signaturesToValidate)
            {
                String[] signatureFragments = signature.SignatureFragments.ToArray();
                string address = signature.Address;

                if (Signing.ValidateSignatures(address, signatureFragments, bundleHash))
                {
                    throw new InvalidSignatureException();
                }     
            }

            return bundle;
        }

        public List<Bundle> GetTransfers(string seed, int start, int end, int securityLevel, bool inclusionStates = false)
        {
            if(!InputValidator.IsValidSeed(seed))
            {
                throw new InvalidTryteException();
            }
            seed = ArrayUtils.AdjustTryteString(seed, SEED_MAX_LENGTH);

            if (start > end || end > (start + 500))
            {
                throw new System.Exception("Invalid inputs provided: start, end");
            }

            IEnumerable<string> addresses = GetNewAddresses(seed, start, end, securityLevel, false);


            return BundlesFromAddresses(addresses, inclusionStates);
        }

        /// <summary>
        /// Takes a tail transaction hash as input, gets the bundle associated with the transaction and then replays the bundle by attaching it to the tangle
        /// </summary>
        public void ReplayTransfer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs the 'prepareTransfers' request, does the Proof-of-Work, broadcasts the transactions to the network and stores them locally
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="depth"></param>
        /// <param name="outputs"></param>
        /// <param name="securityLevel"></param>
        /// <param name="inputs"></param>
        /// <param name="remainderAddress"></param>
        /// <param name="localPOW"></param>
        /// <returns></returns>
        public bool[] SendTransfer(string seed, int depth, List<Transaction> outputs, int securityLevel, List<Transaction> inputs = null, string remainderAddress = null, bool localPOW = true)
        {
            IEnumerable<string> rawTransactions = PrepareTransfers(seed, outputs, securityLevel, inputs, remainderAddress);
            IEnumerable<Transaction> sentTransactions = SendTrytes(rawTransactions.ToArray(), depth);

            bool[] successful = new bool[sentTransactions.Count()];

            for (int i = 0; i < successful.Length; i++)
            {
                successful[i] = FindTransactionsByBundleHash(sentTransactions.ElementAt(i).Bundle).Count() != 0;
            }

            return successful;
        }

        /// <summary>
        /// Broadcasts and stores a list of transactions
        /// </summary>
        /// <param name="rawTransactions">The raw transactions</param>
        public void BroadcastAndStore(List<string> trytes)
        {
            var response = BroadcastTransactionsAsync(trytes).Result;
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                StoreTransactions(trytes);
            }
        }

        /// <summary>
        /// Performs the Proof-of-Work, broadcasts the transactions to the network and stores them locally
        /// </summary>
        /// <param name="rawTransactions"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public IEnumerable<Transaction> SendTrytes(IEnumerable<string> rawTransactions, int depth)
        {
            GetTransactionsToApproveResponse transactionsToApproveResponse = GetTransactionsToApprove(depth);

            AttachToTangleResponse attachToTangleResponse = AttachToTangleAsync(transactionsToApproveResponse.TrunkTransaction, transactionsToApproveResponse.BranchTransaction, rawTransactions.ToList()).Result;

            BroadcastAndStore(attachToTangleResponse.Trytes);


            foreach(string rawTransaction in attachToTangleResponse.Trytes)
            {
                yield return new Transaction(rawTransaction);
            }
        }

        /// <summary>
        /// Finds all transactions associated with a specified address and returns the hashes as list
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns>The transaction hashes</returns>
        public IEnumerable<Transaction> FindTransactionsByAddresses(params string[] addresses)
        {
            List<string> rawTransactions= FindTransactionsAsync(null, addresses.ToList(), null, null).Result.Hashes;

            foreach(string rawTransaction in rawTransactions)
            {
                yield return new Transaction(rawTransaction);
            }
        }

        /// <summary>
        /// Finds all transactions with a specific bundle hash
        /// </summary>
        /// <param name="bundleHashes">The hash</param>
        /// <returns>An array of transactions</returns>
        public IEnumerable<Transaction> FindTransactionsByBundleHash(params string[] bundleHashes)
        {
            var rawTransactions = FindTransactionsAsync(bundleHashes.ToList()).Result.Hashes;
            foreach (string rawTransaction in rawTransactions)
            {
                yield return new Transaction(rawTransaction);
            }
        }

        #region private methods
        /// <summary>
        /// Lets the network verify if the total balance of the addresses of the input list will cover the bundles total balance
        /// </summary>
        /// <param name="inputs">The inputs</param>
        /// <param name="bundle">The bundle</param>
        /// <returns>All valid inputs.</returns>
        private IEnumerable<Transaction> ValidateInputs(IEnumerable<Transaction> inputs, Bundle bundle)
        {
            List<string> inputAddresses = new List<string>();
            foreach (Transaction input in inputs)
            {
                inputAddresses.Add(input.Address);
            }

            GetBalancesResponse response = GetBalancesAsync(GetAddressesFromTransactions(inputs).ToList()).Result;

            if (inputs.Count() != response.Balances.Count())
            {
                throw new IotaApiException();
            }

            BigInteger totalBalance = 0;
            for (var i = 0; i < response.Balances.Count; i++)
            {
                var thisBalance = response.Balances[i];
                totalBalance += thisBalance;

                if (thisBalance > 0)
                {
                    yield return inputs.ElementAt(i);
                }
            }

            if (GetTotalBalance(bundle.Transactions) > totalBalance)
            {
                throw new NotEnoughBalanceException();
            }
        }

        private List<Bundle> BundlesFromAddresses(IEnumerable<string> addresses, bool inclusionStates)
        {
            IEnumerable<Transaction> foundTransactions = FindTransactionsByAddresses(addresses.ToArray());

            List<string> tailTransactions = new List<string>();
            List<string> nonTailBundleHashes = new List<string>();

            foreach (Transaction transaction in foundTransactions)
            {
                // Sort tail and nonTails
                if (transaction.CurrentIndex == 0)
                {
                    tailTransactions.Add(transaction.Hash);
                }
                else
                {
                    if (nonTailBundleHashes.IndexOf(transaction.Bundle) == -1)
                    {
                        nonTailBundleHashes.Add(transaction.Bundle);
                    }
                }
            }

            foundTransactions = FindTransactionsByBundleHash(nonTailBundleHashes.ToArray());
            foreach (Transaction transaction in foundTransactions)
            {
                // Sort tail and nonTails
                if (transaction.CurrentIndex == 0)
                {
                    if (tailTransactions.IndexOf(transaction.Hash) == -1)
                    {
                        tailTransactions.Add(transaction.Hash);
                    }
                }
            }

            List<Bundle> finalBundles = new List<Bundle>();
            string[] tailTxArray = tailTransactions.ToArray();

            GetInclusionStatesResponse gisr = null;
            if(inclusionStates)
            {
                try
                {
                    gisr = GetLatestInclusion(tailTxArray);
                }
                catch(SystemException)
                {}
                if (gisr == null || gisr.States == null || gisr.States.Count == 0)
                {
                    throw new ArgumentException("Inclusion states not found");
                }    
            }


            GetInclusionStatesResponse finalInclusionStates = gisr;

            Parallel.ForEach(tailTransactions, (param) =>
            {
                try
                {
                    Bundle bundle = GetBundle(param);

                    if (inclusionStates)
                    {
                        bool thisInclusion = finalInclusionStates.States[tailTxArray.ToList().IndexOf(param)];
                        foreach(Transaction transaction in bundle.Transactions)
                        {
                            transaction.Persistance = thisInclusion;
                        }
                    }
                    finalBundles.Add(bundle);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Bundle error: " + ex.Message);
                }
            });

            finalBundles.Sort();
            return finalBundles;
        }

        private Bundle TraverseBundle(string trunkTransaction, string bundleHash, Bundle bundle)
        {
            GetTrytesResponse gtr = GetTrytesAsync(trunkTransaction).Result;

            if (gtr.Trytes.Count == 0)
            {
                throw new InvisibleBundleTransactionException();
            }
                
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
            if(transaction.LastIndex == 0 && transaction.CurrentIndex == 0)
            {
                return new Bundle(new List<Transaction>() { transaction });
            }

            // Define new trunkTransaction for search
            var trunkTx = transaction.TrunkTransaction;

            // Add transaction object to bundle
            bundle.AddEntry(transaction);

            // Continue traversing with new trunkTx
            return TraverseBundle(trunkTx, bundleHash, bundle);
        }

        /// <summary>
        /// Gets the latest inclusion
        /// </summary>
        /// <param name="hashes">The hashes</param>
        /// <returns>a GetInclusionStatesResponse cotaining the inclusion state of the specified hashes</returns>
        private GetInclusionStatesResponse GetLatestInclusion(string[] hashes)
        {
            string[] latestMilestone = { GetNodeInfo().LatestSolidSubtangleMilestone };
            return GetInclusionStates(hashes, latestMilestone);
        }
        #endregion
    }
}