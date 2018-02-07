using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iota.Lib.Utils;

namespace Iota.Lib.Test
{
    [TestClass]
    public class PearlDiverTest
    {
        const string NODE = "nodes.thetangle.org"; //Your test node; Please note that not all nodes accept the all requests
        const int PORT = 443;                      //Your test nodes's port
        const bool IS_SSL = true;                  //Your test node's encryption state

        //PearlDiver pearldiver = new PearlDiver();
        IotaApi api = new IotaApi(NODE, PORT, IS_SSL);
        
        [TestMethod]
        public void TestPearDiver_01()
        {
            
        }

    }
}
