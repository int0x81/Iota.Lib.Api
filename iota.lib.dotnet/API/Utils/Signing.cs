using System;
using System.Collections.Generic;
using System.Linq;
using Iota.Lib.CSharp.Api.Model;

namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// Ask cfb
    /// </summary>
    public class Signing
    {
        private ISponge curl;

        public Signing(ISponge curl)
        {
            this.curl = curl;
        }

        public Signing()
        {
            this.curl = new Curl();
        }

        public int[] Key(int[] seed, int index, int length)
        {
            int[] subseed = seed;

            for (int i = 0; i < index; i++)
            {
                for (int j = 0; j < 243; j++)
                {
                    if (++subseed[j] > 1)
                    {
                        subseed[j] = -1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            curl.Reset();
            curl.Absorb(subseed, 0, subseed.Length);
            subseed = curl.Squeeze();
            curl.Reset();
            curl.Absorb(subseed, 0, subseed.Length);

            IList<int> key = new List<int>();
            int[] buffer = new int[subseed.Length];

            while (length-- > 0)
            {
                for (int i = 0; i < 27; i++)
                {
                    buffer = curl.Squeeze();
                    for (int j = 0; j < 243; j++)
                    {
                        key.Add(buffer[j]);
                    }
                }
            }
            return ToIntArray(key);
        }

        private static int[] ToIntArray(IList<int> key)
        {
            int[] a = new int[key.Count];
            int i = 0;
            foreach (int v in key)
            {
                a[i++] = v;
            }
            return a;
        }

        public int[] Digests(int[] key)
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
                        curl.Reset();
                        curl.Absorb(buffer, 0, buffer.Length);
                        buffer = curl.Squeeze();
                    }
                    for (int k = 0; k < 243; k++)
                    {
                        keyFragment[j*243 + k] = buffer[k];
                    }
                }

                curl.Reset();
                curl.Absorb(keyFragment, 0, keyFragment.Length);
                buffer = curl.Squeeze();

                for (int j = 0; j < 243; j++)
                {
                    digests[i*243 + j] = buffer[j];
                }
            }
            return digests;
        }

        public int[] Digest(int[] normalizedBundleFragment, int[] signatureFragment)
        {
            curl.Reset();
            int[] buffer = new int[243];

            for (int i = 0; i < 27; i++)
            {
                buffer = ArrayUtils.SubArray(signatureFragment, i*243, 243);
                ;
                ISponge jCurl = curl.Clone();

                for (int j = normalizedBundleFragment[i] + 13; j-- > 0;)
                {
                    jCurl.Reset();
                    jCurl.Absorb(buffer);
                    buffer = jCurl.Squeeze();
                }
                curl.Absorb(buffer);
            }

            return curl.Squeeze();
        }

        public int[] Address(int[] digests)
        {
            int[] address = new int[243];
            address = curl.Reset()
                .Absorb(digests)
                .Squeeze();
            return address;
        }

        public int[] SignatureFragment(int[] normalizedBundleFragment, int[] keyFragment)
        {
            int[] hash = new int[243];

            for (int i = 0; i < 27; i++)
            {
                Array.Copy(keyFragment, i*243, hash, 0, 243);

                for (int j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    hash = curl.Reset()
                        .Absorb(hash)
                        .Squeeze();
                }

                for (int j = 0; j < 243; j++)
                {
                    Array.Copy(hash, j, keyFragment, i*243 + j, 1);
                }
            }

            return keyFragment;
        }

        public bool ValidateSignatures(string expectedAddress, string[] signatureFragments, string bundleHash)
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
                    Converter.ToTrits(signatureFragments[i]));

                for (int j = 0; j < 243; j++)
                {
                    Array.Copy(digestBuffer, j, digests, i*243 + j, 1);
                }
            }
            string address = Converter.ToTrytes(Address(digests));

            return (expectedAddress.Equals(address));
        }
    }
}