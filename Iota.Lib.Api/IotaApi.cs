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
        /// <param name="is_ssl">States if the connection you want to establish is using ssl-encryption (https)</param>
        public IotaApi(string host, int port, bool is_ssl) : base(host, port, is_ssl)
        {
        }

        /// <summary>
        /// Gets possible inputs of a seed
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="start">Starting key index</param>
        /// <param name="end">Ending key index</param>
        /// <param name="securityLevel">The security level. If no security level is specified the value will be 0 and the addresses of all security levels will be queried.</param>
        /// <returns>A list of inputs</returns>
        public IEnumerable<string> GetInputs(string seed, int start = 0, int end = MAX_KEY_INDEX, int securityLevel = 0)
        {
            if (!InputValidator.IsValidSeed(seed))
            {
                throw new InvalidTryteException();
            }

            seed = PadSeedWithNines(seed);

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
        /// Generates a correct bundle containing the desired ouputs and inputs, signs the transactions if needed and returns the transactions as tryte encoded strings
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="outputs">The transfers to prepare</param>
        /// <param name="inputs">List of inputs used for funding the transfer</param>
        /// <param name="remainderAddress">If defined, this address will be used for sending the remainder value to</param>
        /// <returns>A list of raw transaction data</returns>
        public List<string> PrepareTransfers(string seed, List<Transaction> outputs, List<Transaction> inputs = null, string remainderAddress = null)
        {
            if (!InputValidator.IsValidSeed(seed))
            {
                throw new InvalidTryteException();
            }

            seed = IotaApiUtils.PadSeedWithNines(seed);

            if (!InputValidator.IsArrayOfValidTransactions(outputs))
            {
                throw new InvalidTransactionException();
            }

            if(remainderAddress != null)
            {
                if (!InputValidator.IsValidAddress(remainderAddress))
                {
                    throw new InvalidAddressException($"{remainderAddress} is not a valid address");
                }
            }

            if (inputs != null)
            {
                if (!InputValidator.IsArrayOfValidTransactions(inputs))
                {
                    throw new InvalidTransactionException();
                }
            }

            var remainding = BigInteger.Subtract(GetTotalBalance(inputs), GetTotalBalance(outputs));

            if(remainding < 0)
            {
                throw new NotEnoughBalanceException();
            }
            if(remainding > 0 )
            {
                Transaction refund = new Transaction(GetNewAddresses(seed).ElementAt(0));
                outputs.Add(refund);
            }

            Bundle bundle = new Bundle(outputs);
            List<Transaction> confirmedInputs = ValidateInputs(inputs, bundle).ToList();
            confirmedInputs.ForEach(tx => bundle.AddEntry(tx));
             
            bundle.FinalizeBundle();

            List<String> bundleTrytes = new List<string>();
            bundle.Transactions.ForEach(tx => bundleTrytes.Add(tx.ToTransactionTrytes()));
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
        public IEnumerable<string> GetNewAddresses(string seed, int index = 0, int total = 0, int securityLevel = 2, bool checksum = false)
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
                string newAddress = IotaApiUtils.CreateNewAddress(seed, i, securityLevel, checksum);
                List<string> foundTransactions = FindTransactionsByAddress(newAddress);
                if(foundTransactions.Count == 0)
                {
                    yield return newAddress;
                    i++;
                }
            }  
        }

        /// <summary>
        /// Finds all transactions associated with a specified address and returns the hashes as list
        /// </summary>
        /// <param name="address">The address</param>
        /// <returns>The transaction hashes</returns>
        public List<string> FindTransactionsByAddress(string address)
        {
            List<string> singleAddress = new List<string>
            {
                address
            };
            return FindTransactionsAsync(null, singleAddress, null, null).Result.Hashes;
        }

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

            if (bundle.TotalValue > totalBalance)
            {
                throw new NotEnoughBalanceException();
            }
        }
    }
}