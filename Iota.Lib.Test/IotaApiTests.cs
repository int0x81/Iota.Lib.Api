using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Iota.Lib.Model;
using Iota.Lib.Exception;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.Test
{
    [TestClass]
    public class IotaApiTests
    {
        const string NODE = "03.nl.nodes.iota.cafe"; //Your test node; Please note that not all nodes accept the all requests
        const int PORT = 14265;                      //Your test nodes's port
        const bool IS_SSL = false;                   //Your test node's encryption state
        const int TIMEOUT = 10000;                   //Limits the time a request should take in milliseconds

        IotaApi api = new IotaApi(NODE, PORT, IS_SSL);

        /// <summary>
        /// This test performs an actuall transfer including local proof-of-work
        /// </summary>
        [TestMethod]
        public void PROOF_OF_CONCEPT()
        {
            PowService powService = new PowService();
            const string SEED = "YOUR TESTSEED";
            const string OUTGOING_ADDRESS = "YOUR OUTGOING ADDRESS";
            const string INPUT_ADDRESS = "YOUR INPUT ADDRESS";
            const string REMAINDING_ADDRESS = "THE ADDRESS TO SENT THE REMAINDING VALUE TO";
            Transaction output = new Transaction(OUTGOING_ADDRESS, 2);
            Transaction input = new Transaction(INPUT_ADDRESS, -10, null, "TESTTAG", 0, 2);

            Bundle transfer = api.PrepareTransfers(SEED, new List<Transaction> { output }, 2, new List<Transaction> { input }, REMAINDING_ADDRESS);

            var response = api.GetTransactionsToApproveAsync(2).Result;
            powService.Load(transfer, response.BranchTransaction, response.TrunkTransaction);
            transfer = powService.Execute();
            var result = api.BroadcastTransactions(transfer.GetRawTransactions().ToList());
            Assert.IsTrue(result.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}