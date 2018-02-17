using Iota.Lib.Exception;
using Iota.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iota.Lib.Utils
{
    class PowService
    {
        Bundle _bundle;
        IPoWComputer _powComputer;
        int threadsAvail;
        string _branchTip;
        string _trunkTip;

        public PowService()
        {
            if(POW_GPU_Model.IsSystemSupported())
            {
                _powComputer = new POW_GPU_Model();
            }
            else if(PearlDiver.IsSystemSupported())
            {
                _powComputer = new PearlDiver();
            }
            else
            {
                throw new SystemException("Your machine does not support local proof-of-work");
            }

            threadsAvail = (Environment.ProcessorCount / 4) * 3;
            if (threadsAvail == 0)
            {
                threadsAvail++;
            }
            if(threadsAvail > 9)
            {
                threadsAvail = 9;
            }
        }

        public void Load(Bundle bundle, string branchTip, string trunkTip)
        {
            if(!InputValidator.IsValidBundle(bundle))
            {
                throw new ArgumentException();
            }
            if(!InputValidator.IsStringOfTrytes(branchTip) || !InputValidator.IsStringOfTrytes(trunkTip) || branchTip.Length != Constants.TRANSACTION_HASH_LENGTH || trunkTip.Length != Constants.TRANSACTION_HASH_LENGTH)
            {
                throw new InvalidTryteException();
            }

            _bundle = bundle;
            _branchTip = branchTip;
            _trunkTip = trunkTip;
        }

        public Bundle Execute()
        {
            Curl curl = new Curl(81);

            for (int c = _bundle.Transactions.Count - 1; c >= 0; c--)
            {
                _bundle.Transactions[c].AttachmentTimestamp = IotaApiUtils.CreateTimeStampNow() * 1000;
                _bundle.Transactions[c].AttachmentTimestampLowerBound = 0;
                _bundle.Transactions[c].AttachmentTimestampUpperBound = 12;

                if (c == _bundle.Transactions.Count - 1)
                {
                    _bundle.Transactions[c].BranchTransaction = _branchTip;
                    _bundle.Transactions[c].TrunkTransaction = _trunkTip;
                }
                else
                {
                    _bundle.Transactions[c].BranchTransaction = _trunkTip;
                    _bundle.Transactions[c].TrunkTransaction = _bundle.Transactions[c + 1].Hash;
                }
                
                string transWithPOW = _powComputer.Search(_bundle.Transactions[c].ToTransactionTrytes(), DetermineAvailThreads(), Constants.MIN_WEIGHT_MAGNITUDE);
                _bundle.Transactions[c] = new Transaction(transWithPOW);
                var testme = _bundle.Transactions[c].Hash.Length;
            }
            return _bundle;
        }

        private int DetermineAvailThreads()
        {
            int threads = (Environment.ProcessorCount / 4) * 3;
            if(threads <=0)
            {
                threads = 1;
            }
            return threads;
        }
    }
}