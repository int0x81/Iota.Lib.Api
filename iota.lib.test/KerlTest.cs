using System.IO;
using Iota.Lib.CSharp.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Iota.Lib.CSharpTests.Api
{
    [TestClass]
    public class KerlTest
    {
        private Kerl kerl;
        private string testSeed = "SMRUKAKOPAKXQSIKVZWQGQNKZZWL9BGEFJCIEBRJDIAGWFHUKAOSWACNC9JFDU9WHAPZBEIGWBU9VTNZS";
        private string[] expResults_secLevel_01_no_checksum =
        {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9D", //KeyIndex 0
            "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9"  //KeyIndex 1
        };

        private string[] expResults_level_01_with_checksum =
        {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY",
            "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9UBQYOQXCD"
        };

        [TestInitialize]
        public void InitializeKerl()
        {
            this.kerl = new Kerl();
        }

        
    }
}