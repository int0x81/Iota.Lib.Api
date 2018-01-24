using System;
using System.Collections.Generic;
using System.Linq;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Utils
{
    public static class Signing
    {
        static ISponge kerl;

        static Signing()
        {
            kerl = new Kerl();
        }

        public static int[] Key(int[] seed, int index, int securityLevel)
        {
            int[] filledSeed = FillSeed(seed);

            int[] subseed = Converter.Increment(filledSeed, index);
            
            kerl.Reset();
            kerl.Absorb(subseed);
            subseed = kerl.Squeeze(subseed.Length);

            kerl.Reset();
            kerl.Absorb(subseed);

            IList<int> privateKey = new List<int>();
            int loopCounter = securityLevel;
            int[] buffer = new int[subseed.Length];

            while (loopCounter > 0)
            {
                for (int i = 0; i < 27; i++)
                {
                    buffer = kerl.Squeeze(subseed.Length);

                    foreach(int trit in buffer)
                    {
                        privateKey.Add(trit);
                    }
                }
                loopCounter--;
            }
            return privateKey.ToArray();
        }

        public static int[] Digests(int[] key)
        {
            int[] digests = new int[(int) Math.Floor((decimal) key.Length/6561)*243];
            int[] buffer = new int[243];

            for (int i = 0; i < Math.Floor((decimal) key.Length/6561); i++)
            {
                int[] keyFragment = new int[6561];
                Array.Copy(key, i*6561, keyFragment, 0, 6561);

                for (int j = 0; j < 27; j++)
                {
                    Array.Copy(keyFragment, j*243, buffer, 0, 243);
                    for (int k = 0; k < 26; k++)
                    {
                        kerl.Reset();
                        kerl.Absorb(buffer);
                        buffer = kerl.Squeeze(Kerl.HASH_LENGTH);
                    }
                    for (int k = 0; k < 243; k++)
                    {
                        keyFragment[j*243 + k] = buffer[k];
                    }
                }

                kerl.Reset();
                kerl.Absorb(keyFragment);
                buffer = kerl.Squeeze(Kerl.HASH_LENGTH);

                for (int j = 0; j < 243; j++)
                {
                    digests[i * 243 + j] = buffer[j];
                }
            }
            return digests;
        }

        public static int[] Digest(int[] normalizedBundleFragment, int[] signatureFragment)
        {
            kerl.Reset();
            int[] buffer = new int[243];

            for (int i = 0; i < 27; i++)
            {
                buffer = ArrayUtils.SubArray(signatureFragment, i*243, 243);

                ISponge jKerl = new Kerl();

                for (int j = normalizedBundleFragment[i] + 13; j-- > 0;)
                {
                    jKerl.Reset();
                    jKerl.Absorb(buffer);
                    buffer = jKerl.Squeeze(Kerl.HASH_LENGTH);
                }
                kerl.Absorb(buffer);
            }

            return kerl.Squeeze(Kerl.HASH_LENGTH);
        }

        public static int[] Address(int[] digests)
        {
            kerl.Reset();
            kerl.Absorb(digests);

            return kerl.Squeeze(Kerl.HASH_LENGTH);
        }

        public static int[] SignatureFragment(int[] normalizedBundleFragment, int[] keyFragment)
        {
            int[] hash = new int[243];

            for (int i = 0; i < 27; i++)
            {
                Array.Copy(keyFragment, i*243, hash, 0, 243);

                for (int j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    kerl.Reset();
                    kerl.Absorb(hash);
                    hash = kerl.Squeeze(Kerl.HASH_LENGTH);
                }

                for (int j = 0; j < 243; j++)
                {
                    Array.Copy(hash, j, keyFragment, i*243 + j, 1);
                }
            }

            return keyFragment;
        }

        public static bool ValidateSignatures(string expectedAddress, string[] signatureFragments, string bundleHash)
        {
            Bundle bundle = new Bundle();

            var normalizedBundleFragments = new int[3, 27];
            int[] normalizedBundleHash = bundle.NormalizedBundle(bundleHash);

            // Split hash into 3 fragments
            for (int i = 0; i < 3; i++)
            {
                // normalizedBundleFragments[i] = Arrays.copyOfRange(normalizedBundleHash, i*27, (i + 1)*27);
                Array.Copy(normalizedBundleHash, i*27, normalizedBundleFragments, 0, 27);
            }

            // Get digests
            int[] digests = new int[signatureFragments.Length*243];

            for (int i = 0; i < signatureFragments.Length; i++)
            {
                int[] digestBuffer = Digest(ArrayUtils.SliceRow(normalizedBundleFragments, i%3).ToArray(),
                    Converter.ConvertTrytesToTrits(signatureFragments[i]));

                for (int j = 0; j < 243; j++)
                {
                    Array.Copy(digestBuffer, j, digests, i*243 + j, 1);
                }
            }
            string address = Converter.ConvertTritsToTrytes(Address(digests));

            return (expectedAddress.Equals(address));
        }

        /// <summary>
        /// Fills a seed with null values until the maximum seed length is reached
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <returns>The filled seed</returns>
        private static int[] FillSeed(int[] seed)
        {
            List<int> seedAsList = new List<int>(seed);

            while (seedAsList.Count % (Constants.SEED_MAX_LENGTH * 3) != 0)
            {
                seedAsList.Add(0);
            }

            return seedAsList.ToArray();
        }
    }
}