using Iota.Lib.Model;
using Iota.Lib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Iota.Lib.Utils.IotaApiUtils;

namespace Iota.Lib.Test
{
    [TestClass]
    public class AddressGenerationTest
    {
        readonly string seed = "SMRUKAKOPAKXQSIKVZWQGQNKZZWL9BGEFJCIEBRJDIAGWFHUKAOSWACNC9JFDU9WHAPZBEIGWBU9VTNZS";
        readonly string shortSeed = "SMRUKAKOPAKXQSIEIGWBU9VTNZS";

        readonly string[] addresses_security_01_with_checksum =
        {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9DNVCJURQMY",
            "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9UBQYOQXCD",
            "JIY9BUVVUCTQHGYNZQPZQAMDHPLZMTYPJSEFCZKJ9VCWN9LZWCWROUCTNOOOMSSFUENASJCBJRBULHLBXLLJUWXCHA"
        };

        readonly string[] addresses_security_01_without_checksum =
        {
            "ADJGOINLDBYXPTHJTZCAMABXLNOUFVPRROSRT99RYCNMIBIVKLEKCBVMRWZCKMVLUIMZVHEUXZAJRGA9D",
            "NZ9CNGUKCHUQPRIYLV9OXN9KMDGAEFLDOWENIH9KHPVJGYIWUKKBFDZSZDPAOOFEECVITDXQXPEUAGOT9",
            "JIY9BUVVUCTQHGYNZQPZQAMDHPLZMTYPJSEFCZKJ9VCWN9LZWCWROUCTNOOOMSSFUENASJCBJRBULHLBX"
        };

        readonly string[] addresses_security_02_with_checksum =
        {
            "IXJWSCZNZDMYLGRZZAXWJBAIRNCMMGZMXEWRHMPYVWT9CHGIKEMVRLZLDAJPAFQNUDMRFYSSPMTCSTFU9ORKDY9UFC",
            "CAVX9MCNWZQECHBZGIETEAXDNGFLUMYMKTHVMOVC9PLDJYAMSFYWGPFNVXKGY9MPUXOQSHANNDYFSCUKXJVNIPNZEW",
            "9BJKBADAQV9BYAQHZPNERADGKVWDDTYIKHSATDR9VJLRYHYWRQDUKKBAINYDVMHUDYYCIADIVRNLZNGW9HUF9SNRND"
        };

        readonly string[] addresses_security_02_without_checksum =
        {
            "IXJWSCZNZDMYLGRZZAXWJBAIRNCMMGZMXEWRHMPYVWT9CHGIKEMVRLZLDAJPAFQNUDMRFYSSPMTCSTFU9",
            "CAVX9MCNWZQECHBZGIETEAXDNGFLUMYMKTHVMOVC9PLDJYAMSFYWGPFNVXKGY9MPUXOQSHANNDYFSCUKX",
            "9BJKBADAQV9BYAQHZPNERADGKVWDDTYIKHSATDR9VJLRYHYWRQDUKKBAINYDVMHUDYYCIADIVRNLZNGW9"
        };

        readonly string[] addresses_security_03_with_checksum =
        {
            "YBKSGTFAFSZZYACIQ9T9GVRQ9KMJHIAIVQJWADIFMVFATV9OBALBZTYZJLBSIYMVDWKQVLC9VBBXJNKQZRXGYOFFKZ",
            "IRJYUZ9HTOOUB9XYNARUMXALMOVTOCZECZKCOSRIDCODAKQDRLZZBGSVTKUHQTLJVQJFDHNNFXSAEOEZWVKENFFIYC",
            "SDXOHRLRJEAYTANFL9PKMOOIJMAJDBJDHNHMHDCPUHTMKUJOETSBOV9OYYPUSPQRFPTXVPCADHVRCJGUYIGOXXLNSD"
        };

        readonly string[] addresses_security_03_without_checksum =
        {
            "YBKSGTFAFSZZYACIQ9T9GVRQ9KMJHIAIVQJWADIFMVFATV9OBALBZTYZJLBSIYMVDWKQVLC9VBBXJNKQZ",
            "IRJYUZ9HTOOUB9XYNARUMXALMOVTOCZECZKCOSRIDCODAKQDRLZZBGSVTKUHQTLJVQJFDHNNFXSAEOEZW",
            "SDXOHRLRJEAYTANFL9PKMOOIJMAJDBJDHNHMHDCPUHTMKUJOETSBOV9OYYPUSPQRFPTXVPCADHVRCJGUY"
        };

        [TestMethod]
        public void TestCreateNewAddress()
        {
            string address_01 = CreateNewAddress(seed, 0, 1, true);
            string address_02 = CreateNewAddress(seed, 1, 2, true);
            string address_03 = CreateNewAddress(seed, 0, 3, true);
            string address_04 = CreateNewAddress(seed, 2, 3, false);

            Assert.AreEqual(address_01, addresses_security_01_with_checksum[0]);
            Assert.AreEqual(address_02, addresses_security_02_with_checksum[1]);
            Assert.AreEqual(address_03, addresses_security_03_with_checksum[0]);
            Assert.AreEqual(address_04, addresses_security_03_without_checksum[2]);
        }

        [TestMethod]
        public void TestCreateTimestampNow()
        {
            long timestamp = CreateTimeStampNow();
            Assert.IsTrue(timestamp > 0);
        }
    }
}
