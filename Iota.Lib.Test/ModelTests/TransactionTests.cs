using Iota.Lib.Core;
using Iota.Lib.Model;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Iota.Lib.Utils.Constants;

namespace Iota.Lib.Test.ModelTests
{
    [TestClass]
    public class TransactionTests
    {
        const string NODE = "nodes.thetangle.org"; //Your test node; Please note that not all nodes accept the all requests
        const int PORT = 443;                      //Your test nodes's port
        const bool SSL = true;                     //Your test node's encryption state

        readonly IotaCoreApi api = new IotaCoreApi(NODE, PORT, SSL);

        [TestMethod]
        public void TestTransactionConstructor()
        {
            var response = api.GetTransactionsToApproveAsync(8).Result;
            string raw_transaction = api.GetTrytesAsync(response.BranchTransaction).Result.Trytes[0];
            Assert.IsTrue(raw_transaction.Length == RAW_TRANSACTION_LENGTH);
            Transaction transaction = new Transaction(raw_transaction);
            Assert.IsTrue(transaction.SignatureMessageFragment.Length == SIGNATURE_MESSAGE_LENGTH);
            Assert.IsTrue(InputValidator.IsTrytes(transaction.SignatureMessageFragment, transaction.SignatureMessageFragment.Length));
            Assert.IsTrue(transaction.Address.Length == ADDRESSLENGTH_WITHOUT_CHECKSUM);
            Assert.IsTrue(InputValidator.IsAddress(transaction.Address));
            Assert.IsTrue(transaction.AttachmentTimestamp >= 0);
            Assert.IsTrue(transaction.AttachmentTimestampLowerBound >= 0);
            Assert.IsTrue(transaction.AttachmentTimestampUpperBound >= 0);
            Assert.IsTrue(transaction.BranchTransaction.Length == TRANSACTION_HASH_LENGTH);
            Assert.IsTrue(transaction.Bundle.Length == BUNDLE_HASH_LENGTH);
            Assert.IsTrue(transaction.CurrentIndex >= 0);
            Assert.IsTrue(transaction.Hash.Length == TRANSACTION_HASH_LENGTH);
            Assert.IsTrue(transaction.LastIndex >= 0);
            Assert.IsTrue(transaction.LastIndex >= transaction.CurrentIndex);
            Assert.IsTrue(transaction.Nonce.Length == NONCE_LENGTH);
            Assert.IsTrue(transaction.ObsoleteTag.Length == TAG_LENGTH);
            Assert.IsTrue(transaction.Tag.Length == TAG_LENGTH);
            Assert.IsTrue(transaction.Timestamp >= 0);
            Assert.IsTrue(transaction.TrunkTransaction.Length == TRANSACTION_HASH_LENGTH);
            Assert.IsTrue(transaction.Value >= 0);
        }
    }
}
