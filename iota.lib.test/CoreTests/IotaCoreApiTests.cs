using Iota.Lib.Api.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using static System.Net.HttpStatusCode;

namespace Iota.Lib.Test
{
    [TestClass]
    public class IotaCoreApiTests
    {
        const string NODE = "nodes.thetangle.org"; //Your test node; Please note that not all nodes accept the 'Neighbor'-actions.
        const int  PORT = 443;                     //Your test port
        const int TIMEOUT = 10000;                 //Limits the time a request should take in milliseconds

        IotaCoreApi api = new IotaCoreApi(NODE, PORT);

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetTips()
        {
            var response = api.GetTips();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Hashes.Count > 0);

            var asyncResponse = api.GetTipsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(asyncResponse.StatusCode == OK);
            Assert.IsTrue(asyncResponse.Hashes.Count > 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetNodeInfo()
        {
            var response = api.GetNodeInfo();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.JreVersion.Length != 0);

            var asyncResponse = api.GetNodeInfoAsync().GetAwaiter().GetResult();
            Assert.IsTrue(asyncResponse.StatusCode == OK);
            Assert.IsTrue(response.JreVersion.Length != 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetNeighbors()
        {
            var response = api.GetNeighbors();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Neighbors.Count >= 0);

            var asyncResponse = api.GetNeighborsAsync().GetAwaiter().GetResult();
            Assert.IsTrue(asyncResponse.StatusCode == OK);
            Assert.IsTrue(response.Neighbors.Count >= 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestAddNeighbors()
        {
            string[] neighbors =
            {
                "udp://8.8.8.8:14265",
                "udp://8.8.8.5:14265"
            };

            var response = api.AddNeighbors(neighbors);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.AddedNeighbors >= 0);

            var asyncResponse = api.AddNeighborsAsync(neighbors).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.AddedNeighbors >= 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestRemoveNeighbors()
        {
            string[] neighbors =
            {
                "udp://8.8.8.8:14265",
                "udp://8.8.8.5:14265"
            };

            var response = api.RemoveNeighbors(neighbors);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.RemovedNeighbors >= 0);

            var asyncResponse = api.RemoveNeighborsAsync(neighbors).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.RemovedNeighbors >= 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestFindTransactions()
        {
            List<string> addresses = new List<string>()
            {
                "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY",
                "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9UBQYOQXCD",
                "JIY9BUVVUCTQHGYNZQPZQAMDHPLZMTYPJSEFCZKJ9VCWN9LZWCWROUCTNOOOMSSFUENASJCBJRBULHLBXLLJUWXCHA"
            };

            var response = api.FindTransactions(null, addresses);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Hashes.Count >= 0);

            response = api.FindTransactions();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Hashes.Count >= 0);
        }
    }
}
