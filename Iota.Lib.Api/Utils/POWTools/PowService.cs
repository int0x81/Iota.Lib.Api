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
            Curl curl = new Curl();
            _bundle.Transactions.Reverse();

            for (int c = _bundle.Transactions.Count - 1; c >= 0; c--)
            {
                

                _bundle.Transactions[c].AttachmentTimestamp = IotaApiUtils.CreateTimeStampNow() * 1000;
                _bundle.Transactions[c].AttachmentTimestampLowerBound = 0;
                _bundle.Transactions[c].AttachmentTimestampUpperBound = 3812798742493;

                if (c == _bundle.Transactions.Count - 1)
                {
                    _bundle.Transactions[c].BranchTransaction = _branchTip;
                    _bundle.Transactions[c].TrunkTransaction = _trunkTip;
                }
                else
                {
                    var rawTransaction = _bundle.Transactions[c + 1].ToTransactionTrytes();
                    var trits = curl.Absorb(Converter.ConvertTrytesToTrits(rawTransaction));
                    _bundle.Transactions[c].BranchTransaction = _trunkTip;
                    _bundle.Transactions[c + 1].Hash = Converter.ConvertTritsToTrytes(curl.Squeeze(Curl.HASH_LENGTH));
                    _bundle.Transactions[c].TrunkTransaction = _bundle.Transactions[c + 1].Hash;
                }
                
                string transWithPOW = _powComputer.Search(_bundle.Transactions[c].ToTransactionTrytes(), 10, Constants.MIN_WEIGHT_MAGNITUDE);
                _bundle.Transactions[c] = new Transaction(transWithPOW);
                var testme = _bundle.Transactions[c].ToTransactionTrytes();
            }
            return _bundle;

        }
    }
}