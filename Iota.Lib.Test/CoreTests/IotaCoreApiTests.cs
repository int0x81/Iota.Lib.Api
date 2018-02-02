using Iota.Lib.Core;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using static System.Net.HttpStatusCode;

namespace Iota.Lib.Test
{
    [TestClass]
    public class IotaCoreApiTests
    {
        const string NODE = "nodes.thetangle.org"; //Your test node; Please note that not all nodes accept the all requests
        const int  PORT = 443;                     //Your test nodes's port
        const bool IS_SSL = true;                  //Your test node's encryption state
        const int TIMEOUT = 10000;                 //Limits the time a request should take in milliseconds

        List<string> ADDRESSES = new List<string>()
        {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY",
            "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9UBQYOQXCD",
            "JIY9BUVVUCTQHGYNZQPZQAMDHPLZMTYPJSEFCZKJ9VCWN9LZWCWROUCTNOOOMSSFUENASJCBJRBULHLBXLLJUWXCHA"
        };

        readonly List<string> RAW_TRANSACTIONS = new List<string>()
        {
            "BYSWEAUTWXHXZ9YBZISEK9LUHWGMHXCGEVNZHRLUWQFCUSDXZHOFHWHL9MQPVJXXZLIXPXPXF9KYEREFSKCPKYIIKPZVLHUTDFQKKVVBBN9ATTLPCNPJDWDEVIYYLGPZGCWXOBDXMLJC9VO9QXTTBLAXTTBFUAROYEGQIVB9MJWJKXJMCUPTWAUGFZBTZCSJVRBGMYXTVBDDS9MYUJCPZ9YDWWQNIPUAIJXXSNLKUBSCOIJPCLEFPOXFJREXQCUVUMKSDOVQGGHRNILCO9GNCLWFM9APMNMWYASHXQAYBEXF9QRIHIBHYEJOYHRQJAOKAQ9AJJFQ9WEIWIJOTZATIBOXQLBMIJU9PCGBLVDDVFP9CFFSXTDUXMEGOOFXWRTLFGV9XXMYWEMGQEEEDBTIJ9OJOXFAPFQXCDAXOUDMLVYRMRLUDBETOLRJQAEDDLNVIRQJUBZBO9CCFDHIX9MSQCWYAXJVWHCUPTRSXJDESISQPRKZAFKFRULCGVRSBLVFOPEYLEE99JD9SEBALQINPDAZHFAB9RNBH9AZWIJOTLBZVIEJIAYGMC9AZGNFWGRSWAXTYSXVROVNKCOQQIWGPNQZKHUNODGYADPYLZZZUQRTJRTODOUKAOITNOMWNGHJBBA99QUMBHRENGBHTH9KHUAOXBVIVDVYYZMSEYSJWIOGGXZVRGN999EEGQMCOYVJQRIRROMPCQBLDYIGQO9AMORPYFSSUGACOJXGAQSPDY9YWRRPESNXXBDQ9OZOXVIOMLGTSWAMKMTDRSPGJKGBXQIVNRJRFRYEZ9VJDLHIKPSKMYC9YEGHFDS9SGVDHRIXBEMLFIINOHVPXIFAZCJKBHVMQZEVWCOSNWQRDYWVAIBLSCBGESJUIBWZECPUCAYAWMTQKRMCHONIPKJYYTEGZCJYCT9ABRWTJLRQXKMWY9GWZMHYZNWPXULNZAPVQLPMYQZCYNEPOCGOHBJUZLZDPIXVHLDMQYJUUBEDXXPXFLNRGIPWBRNQQZJSGSJTTYHIGGFAWJVXWL9THTPWOOHTNQWCNYOYZXALHAZXVMIZE9WMQUDCHDJMIBWKTYH9AC9AFOT9DPCADCV9ZWUTE9QNOMSZPTZDJLJZCJGHXUNBJFUBJWQUEZDMHXGBPTNSPZBR9TGSKVOHMOQSWPGFLSWNESFKSAZY9HHERAXALZCABFYPOVLAHMIHVDBGKUMDXC9WHHTIRYHZVWNXSVQUWCR9M9RAGMFEZZKZ9XEOQGOSLFQCHHOKLDSA9QCMDGCGMRYJZLBVIFOLBIJPROKMHOYTBTJIWUZWJMCTKCJKKTR9LCVYPVJI9AHGI9JOWMIWZAGMLDFJA9WU9QAMEFGABIBEZNNAL9OXSBFLOEHKDGHWFQSHMPLYFCNXAAZYJLMQDEYRGL9QKCEUEJ9LLVUOINVSZZQHCIKPAGMT9CAYIIMTTBCPKWTYHOJIIY9GYNPAJNUJ9BKYYXSV9JSPEXYMCFAIKTGNRSQGUNIYZCRT9FOWENSZQPD9ALUPYYAVICHVYELYFPUYDTWUSWNIYFXPX9MICCCOOZIWRNJIDALWGWRATGLJXNAYTNIZWQ9YTVDBOFZRKO9CFWRPAQQRXTPACOWCPRLYRYSJARRKSQPR9TCFXDVIXLP9XVL99ERRDSOHBFJDJQQGGGCZNDQ9NYCTQJWVZIAELCRBJJFDMCNZU9FIZRPGNURTXOCDSQGXTQHKHUECGWFUUYS9J9NYQ9U9P9UUP9YMZHWWWCIASCFLCMSKTELZWUGCDE9YOKVOVKTAYPHDF9ZCCQAYPJIJNGSHUIHHCOSSOOBUDOKE9CJZGYSSGNCQJVBEFTZFJ9SQUHOASKRRGBSHWKBCBWBTJHOGQ9WOMQFHWJVEG9NYX9KWBTCAIXNXHEBDIOFO9ALYMFGRICLCKKLG9FOBOX9PDWNQRGHBKHGKKRLWTBEQMCWQRLHAVYYZDIIPKVQTHYTWQMTOACXZOQCDTJTBAAUWXSGJF9PNQIJ9AJRUMUVCPWYVYVARKR9RKGOUHHNKNVGGPDDLGKPQNOYHNKAVVKCXWXOQPZNSLATUJT9AUWRMPPSWHSTTYDFAQDXOCYTZHOYYGAIM9CELMZ9AZPWB9MJXGHOKDNNSZVUDAGXTJJSSZCPZVPZBYNNTUQABSXQWZCHDQSLGK9UOHCFKBIBNETK999999999999999999999999999999999999999999999999999999999999999999999999999999999NOXDXXKUDWLOFJLIPQIBRBMGDYCPGDNLQOLQS99EQYKBIU9VHCJVIPFUYCQDNY9APGEVYLCENJIOBLWNB999999999XKBRHUD99C99999999NKZKEKWLDKMJCI9N9XQOLWEPAYWSH9999999999999999999999999KDDTGZLIPBNZKMLTOLOXQVNGLASESDQVPTXALEKRMIOHQLUHD9ELQDBQETS9QFGTYOYWLNTSKKMVJAUXSIROUICDOXKSYZTDPEDKOQENTJOWJONDEWROCEJIEWFWLUAACVSJFTMCHHXJBJRKAAPUDXXVXFWP9X9999IROUICDOXKSYZTDPEDKOQENTJOWJONDEWROCEJIEWFWLUAACVSJFTMCHHXJBJRKAAPUDXXVXFWP9X9999"
        };

        IotaCoreApi api = new IotaCoreApi(NODE, PORT, IS_SSL);

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

        //[TestMethod, Timeout(TIMEOUT)]
        //public void TestGetNeighbors()
        //{
        //    var response = api.GetNeighbors();
        //    Assert.IsTrue(response.StatusCode == OK);

        //    var asyncResponse = api.GetNeighborsAsync().GetAwaiter().GetResult();
        //    Assert.IsTrue(asyncResponse.StatusCode == OK);
        //}

        //[TestMethod, Timeout(TIMEOUT)]
        //public void TestAddNeighbors()
        //{
        //    string[] neighbors =
        //    {
        //        "udp://8.8.8.8:14265",
        //        "udp://8.8.8.5:14265"
        //    };

        //    var response = api.AddNeighbors(neighbors);
        //    Assert.IsTrue(response.StatusCode == OK);
        //    Assert.IsTrue(response.AddedNeighbors >= 0);

        //    var asyncResponse = api.AddNeighborsAsync(neighbors).GetAwaiter().GetResult();
        //    Assert.IsTrue(response.StatusCode == OK);
        //    Assert.IsTrue(response.AddedNeighbors >= 0);
        //}

        //[TestMethod, Timeout(TIMEOUT)]
        //public void TestRemoveNeighbors()
        //{
        //    string[] neighbors =
        //    {
        //        "udp://8.8.8.8:14265",
        //        "udp://8.8.8.5:14265"
        //    };

        //    var response = api.RemoveNeighbors(neighbors);
        //    Assert.IsTrue(response.StatusCode == OK);
        //    Assert.IsTrue(response.RemovedNeighbors >= 0);

        //    var asyncResponse = api.RemoveNeighborsAsync(neighbors).GetAwaiter().GetResult();
        //    Assert.IsTrue(response.StatusCode == OK);
        //    Assert.IsTrue(response.RemovedNeighbors >= 0);
        //}

        [TestMethod, Timeout(TIMEOUT)]
        public void TestFindTransactions()
        {
            foreach(string address in ADDRESSES)
            {
                if(!InputValidator.IsValidAddress(address))
                {
                    throw new ArgumentException();
                }
            }
            var response = api.FindTransactions(null, ADDRESSES);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Hashes.Count >= 0);

            response = api.FindTransactionsAsync(null, ADDRESSES).Result;
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Hashes.Count >= 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetTrytes()
        {
            List<string> hashes = new List<string>()
            {
                "OAATQS9VQLSXCLDJVJJVYUGONXAXOFMJOZNSYWRZSWECMXAQQURHQBJNLD9IOFEPGZEPEMPXCIVRX9999"
            };

            var response = api.GetTrytes(hashes.ToArray());
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Trytes.Count >= 0);

            response = api.GetTrytesAsync().GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Trytes.Count >= 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetInclusionStates()
        {
            string[] transactions =
            {
                "QHBYXQWRAHQJZEIARWSQGZJTAIITOZRMBFICIPAVD9YRJMXFXBDPFDTRAHHHP9YPDUVTNOFWZGFGWMYHEKNAGNJHMW"
            };

            string[] tips =
            {
                "ZIJGAJ9AADLRPWNCYNNHUHRRAC9QOUDATEDQUMTNOTABUVRPTSTFQDGZKFYUUIE9ZEBIVCCXXXLKX9999"
            };

            var response = api.GetInclusionStates(transactions, tips);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.States.Count > 0);

            response = api.GetInclusionStatesAsync(transactions, tips).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.States.Count > 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetBalances()
        {
            List<string> addresses = new List<string>
            {
                "HBBYKAKTILIPVUKFOTSLHGENPTXYBNKXZFQFR9VQFWNBMTQNRVOUKPVPRNBSZVVILMAFBKOTBLGLWLOHQ"
            };

            

            var response = api.GetBalances(addresses);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Balances[0] >= 0);

            response = api.GetBalancesAsync(addresses).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(response.Balances[0] >= 0);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestGetTransactionsToApprove()
        {
            var response = api.GetTransactionsToApprove(27);
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(InputValidator.IsStringOfTrytes(response.TrunkTransaction));

            response = api.GetTransactionsToApproveAsync(27).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
            Assert.IsTrue(InputValidator.IsStringOfTrytes(response.BranchTransaction));
        }

        //[TestMethod, Timeout(TIMEOUT)]
        //public void TestAttachToTangle()
        //{
        //    var transactionsToApprove = api.GetTransactionsToApprove(27);

        //    var response = api.AttachToTangle(transactionsToApprove.TrunkTransaction, transactionsToApprove.BranchTransaction, RAW_TRANSACTIONS, Constants.MIN_WEIGHT_MAGNITUDE);
        //    Assert.IsTrue(response.StatusCode == OK);
        //    Assert.IsTrue(response.Trytes.Count >= 0);

        //    response = api.AttachToTangleAsync(transactionsToApprove.TrunkTransaction, transactionsToApprove.BranchTransaction, RAW_TRANSACTIONS, Constants.MIN_WEIGHT_MAGNITUDE).GetAwaiter().GetResult();
        //    Assert.IsTrue(response.StatusCode == OK);
        //    Assert.IsTrue(response.Trytes.Count >= 0);
        //}

        //[TestMethod, Timeout(TIMEOUT)]
        //public void TestInterruptAttachingToTangle()
        //{
        //    var response = api.InterruptAttachingToTangle();
        //    Assert.IsTrue(response.StatusCode == OK);

        //    response = api.InterruptAttachingToTangleAsync().GetAwaiter().GetResult();
        //    Assert.IsTrue(response.StatusCode == OK);
        //}

        [TestMethod, Timeout(TIMEOUT)]
        public void TestBroadcastTransactions()
        {
            var response = api.BroadcastTransactions(RAW_TRANSACTIONS);
            Assert.IsTrue(response.StatusCode == OK);

            response = api.BroadcastTransactionsAsync(RAW_TRANSACTIONS).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
        }

        [TestMethod, Timeout(TIMEOUT)]
        public void TestStoreTransactions()
        {
            var response = api.StoreTransactions(RAW_TRANSACTIONS);
            Assert.IsTrue(response.StatusCode == OK);

            response = api.StoreTransactionsAsync(RAW_TRANSACTIONS).GetAwaiter().GetResult();
            Assert.IsTrue(response.StatusCode == OK);
        }
    }
}
