using System;
using System.Collections.Generic;
using System.Linq;
using Iota.Lib.Model;
using static Iota.Lib.Utils.Constants;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// Provides several methods needed to sign transactions
    /// </summary>
    public static class Signing
    {
        static Kerl kerl = new Kerl();

        /// <summary>
        /// Creates a private key
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="index">The index</param>
        /// <param name="securityLevel">The security level</param>
        /// <returns>The private key as trit-array</returns>
        public static int[] Key(int[] seed, int index, int securityLevel)
        {
            int[] filledSeed = ArrayUtils.PadArrayWithZeros(seed, SEED_MAX_LENGTH * 3);

            int[] subseed = Converter.Increment(filledSeed, index);
            
            kerl.Reset();
            kerl.Absorb(subseed);
            kerl.Squeeze(ref subseed, 0, subseed.Length);

            kerl.Reset();
            kerl.Absorb(subseed);

            IList<int> privateKey = new List<int>();
            int loopCounter = securityLevel;
            int[] buffer = new int[subseed.Length];

            while (loopCounter > 0)
            {
                for (int i = 0; i < 27; i++)
                {
                    kerl.Squeeze(ref buffer, 0, seed.Length);

                    foreach(int trit in buffer)
                    {
                        privateKey.Add(trit);
                    }
                }
                loopCounter--;
            }
            return privateKey.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">The privat key</param>
        /// <returns></returns>
        public static int[] Digests(int[] key)
        {
            int securityLevel = (int)Math.Floor((decimal)key.Length / KEY_LENGTH);
            int[] digests = new int[243 * securityLevel];
            int[] buffer = new int[243];

            for(int i = 0; i < securityLevel; i++)
            {
                int[] keyFragment = new int[KEY_LENGTH];
                Array.Copy(key, i * KEY_LENGTH, keyFragment, 0, KEY_LENGTH);

                for(int j = 0; j < 27; j++)
                {
                    Array.Copy(keyFragment, j * 243, buffer, 0, 243);

                    for(int k = 0; k < 26; k++)
                    {
                        Kerl kerl_02 = new Kerl();
                        kerl_02.Absorb(buffer, 0, buffer.Length);
                        kerl_02.Squeeze(ref buffer, 0, Kerl.HASH_LENGTH);
                    }

                    Array.Copy(buffer, 0, keyFragment, j * 243, 243);
                }

                Kerl kerl = new Kerl();
                kerl.Absorb(keyFragment, 0, keyFragment.Length);
                kerl.Squeeze(ref buffer, 0, Kerl.HASH_LENGTH);

                Array.Copy(buffer, 0, digests, i * 243, 243);
                
            }
            return digests;
        }

        //public static int[] Digest(int[] normalizedBundleFragment, int[] signatureFragment)
        //{
        //    kerl.Reset();
        //    int[] buffer = new int[Kerl.HASH_LENGTH];
        //    Kerl kerl_02 = new Kerl();

        //    for (int i = 0; i < 27; i++)
        //    {
        //        buffer = ArrayUtils.CreateSubArray(signatureFragment, i * Kerl.HASH_LENGTH, Kerl.HASH_LENGTH);


        //        for (int j = normalizedBundleFragment[i] + 13; j-- > 0;)
        //        {
        //            kerl_02.Reset();
        //            kerl_02.Absorb(buffer);
        //            kerl_02.Squeeze(ref buffer, 0, buffer.Length);
        //        }
        //        kerl.Absorb(buffer);
        //    }

        //    kerl.Squeeze(ref buffer, 0, buffer.Length);

        //    return buffer;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digests"></param>
        /// <returns></returns>
        public static int[] Address(int[] digests)
        {
            int[] address = new int[Kerl.HASH_LENGTH];
            kerl.Reset();
            kerl.Absorb(digests);
            kerl.Squeeze(ref address, 0 , address.Length);

            return address;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalizedBundleFragment"></param>
        /// <param name="keyFragment"></param>
        /// <returns></returns>
        public static int[] SignatureFragment(int[] normalizedBundleFragment, int[] keyFragment)
        {
            int[] signatureFragment = (int[])keyFragment.Clone();

            for (int i = 0; i < 27; i++)
            {
                int[] hash = new int[243];
                Array.Copy(signatureFragment, i * 243, hash, 0, 243);

                for (int j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    kerl.Reset();
                    kerl.Absorb(hash);
                    kerl.Squeeze(ref hash, 0, Kerl.HASH_LENGTH);
                }

                Array.Copy(hash, 0, signatureFragment, i * 243, 243);
            }

            return signatureFragment;
        }

        //public static bool ValidateSignatures(string expectedAddress, string[] signatureFragments, string bundleHash)
        //{
        //    Bundle bundle = new Bundle();

        //    var normalizedBundleFragments = new int[3, 27];
        //    int[] normalizedBundleHash = bundle.NormalizeBundle(bundleHash);

        //    // Split hash into 3 fragments
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Array.Copy(normalizedBundleHash, i*27, normalizedBundleFragments, 0, 27);
        //    }

        //    // Get digests
        //    int[] digests = new int[signatureFragments.Length*243];

        //    for (int i = 0; i < signatureFragments.Length; i++)
        //    {
        //        int[] digestBuffer = Digest(ArrayUtils.SliceRow(normalizedBundleFragments, i%3).ToArray(), Converter.ConvertTrytesToTrits(signatureFragments[i]));

        //        for (int j = 0; j < 243; j++)
        //        {
        //            Array.Copy(digestBuffer, j, digests, i*243 + j, 1);
        //        }
        //    }
        //    string address = Converter.ConvertTritsToTrytes(Address(digests));

        //    return (expectedAddress.Equals(address));
        //}
    }
}