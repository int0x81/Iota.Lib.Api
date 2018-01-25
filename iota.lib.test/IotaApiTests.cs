using Iota.Lib.CSharp.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iota.Lib.CSharpTests
{
    [TestClass]
    public class IotaApiTests
    {
        IotaApi testMe;

        public IotaApiTests()
        {
            testMe = new IotaApi("nodes.thetangle.org", 443);
        }
    }
}
