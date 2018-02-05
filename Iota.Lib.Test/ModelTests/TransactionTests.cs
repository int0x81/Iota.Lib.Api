using Iota.Lib.Core;
using Iota.Lib.Model;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Iota.Lib.Utils.Constants;

namespace Iota.Lib.Test
{
    [TestClass]
    public class TransactionTests
    {
        const string NODE = "nodes.thetangle.org"; //Your test node; Please note that not all nodes accept the all requests
        const int PORT = 443;                      //Your test nodes's port
        const bool IS_SSL = true;                  //Your test node's encryption state

        readonly IotaCoreApi api = new IotaCoreApi(NODE, PORT, IS_SSL);

        [TestMethod]
        public void TestTransactionConstructor()
        {
            var response = api.GetTransactionsToApproveAsync(8).Result;
            string raw_transaction_01 = api.GetTrytesAsync(response.BranchTransaction).Result.Trytes[0];
            Assert.IsTrue(raw_transaction_01.Length == RAW_TRANSACTION_LENGTH);
            Transaction transaction_01 = new Transaction(raw_transaction_01);
            Assert.IsTrue(InputValidator.IsValidTransaction(transaction_01));
            string raw_transaction_02 = transaction_01.ToTransactionTrytes();
            Assert.IsTrue(raw_transaction_01.Length == raw_transaction_02.Length);
            Transaction transaction_02 = new Transaction(raw_transaction_02);
            Assert.AreEqual(transaction_01.Address, transaction_02.Address);
            Assert.AreEqual(transaction_01.AttachmentTimestamp, transaction_02.AttachmentTimestamp);
            Assert.AreEqual(transaction_01.AttachmentTimestampLowerBound, transaction_02.AttachmentTimestampLowerBound);
            Assert.AreEqual(transaction_01.AttachmentTimestampUpperBound, transaction_02.AttachmentTimestampUpperBound);
            Assert.AreEqual(transaction_01.BranchTransaction, transaction_02.BranchTransaction);
            Assert.AreEqual(transaction_01.Bundle, transaction_02.Bundle);
            Assert.AreEqual(transaction_01.CurrentIndex, transaction_02.CurrentIndex);
            Assert.AreEqual(transaction_01.Hash, transaction_02.Hash);
            Assert.AreEqual(transaction_01.LastIndex, transaction_02.LastIndex);
            Assert.AreEqual(transaction_01.Nonce, transaction_02.Nonce);
            Assert.AreEqual(transaction_01.ObsoleteTag, transaction_02.ObsoleteTag);
            Assert.AreEqual(transaction_01.SignatureMessageFragment, transaction_02.SignatureMessageFragment);
            Assert.AreEqual(transaction_01.Tag, transaction_02.Tag);
            Assert.AreEqual(transaction_01.Timestamp, transaction_02.Timestamp);
            Assert.AreEqual(transaction_01.TrunkTransaction, transaction_02.TrunkTransaction);
            Assert.AreEqual(transaction_01.Value, transaction_02.Value);
            Assert.IsTrue(raw_transaction_01.Length == raw_transaction_02.Length);
        }
    }
}
