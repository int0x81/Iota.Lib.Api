using Iota.Lib.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iota.Lib.Test.ModelTests
{
    [TestClass]
    class BundleTests
    {


        [TestMethod]
        public void TestGetBundleHash()
        {
            List<Transaction> transactions = new List<Transaction>
            {
            new Transaction
            {
                Address = "JHYLDJCBBTSFGVTBONTIVOWURCWMWBGGVRTOAMTKKFHWJAJHKKPWEYTAVDXMUSJBIUYEVZMO9LXBWHTUZ",
                Value = 3,
                ObsoleteTag = "999999999999999999999999999",
                Timestamp = 1515494426,
                CurrentIndex = 0,
                LastIndex = 2
            },
            new Transaction
            {
                Address = "HRKDPLUQEEVPBOWRFBLULSAZBEQRRZMDTKHJYISUFXFJIRFGYISBEYSJ9LKOINHAC9UCELAEQZJJZXKHX",
                Value = -3,
                ObsoleteTag = "999999999999999999999999999",
                Timestamp = 1515494426,
                CurrentIndex = 1,
                LastIndex = 2
            },
            new Transaction
            {
                Address = "HRKDPLUQEEVPBOWRFBLULSAZBEQRRZMDTKHJYISUFXFJIRFGYISBEYSJ9LKOINHAC9UCELAEQZJJZXKHX",
                Value = 0,
                ObsoleteTag = "999999999999999999999999999",
                Timestamp = 1515494426,
                CurrentIndex = 2,
                LastIndex = 2
            }
            };

            //string expectedHash_01 = "UMGXJ9JCUDQIYFYQLWDSLCVJBKRQIF9AEFBJBCRRCQAZGQNGTLHH9YZKJABACRJCFGZAFHQPGYULOLQVB";

            //string realHash = Bundle.GetBundleHash(transactions);
        }
    }
}
