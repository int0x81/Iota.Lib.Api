using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Digests;

using static Iota.Lib.Utils.Converter;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// <see href="https://github.com/iotaledger/kerl">Kerl</see> is the hashfunction Iota uses for multiple scenarios such as address generation, signature generation etc.
    /// It is sponge-function based on <see href="https://keccak.team/keccak.html">Keccak-384</see>.
    /// </summary>
    public class Kerl : ISponge
    {
        private readonly KeccakDigest keccak;

        public const int HASH_LENGTH = 243;
        public const int BIT_HASH_LENGTH = 384;
        public const int BYTE_HASH_LENGTH = BIT_HASH_LENGTH / 8;

        private byte[] byteState;
        private int[] tritState;

        public Kerl() 
        {
            keccak = new KeccakDigest(BIT_HASH_LENGTH);
            byteState = new byte[BYTE_HASH_LENGTH];
            tritState = new int[HASH_LENGTH];
        }

        public ISponge Clone()
        {
            return new Kerl();
        }

        public ISponge Absorb(int[] trits, int offset, int length)
        {
            List<int> tritsAsList = new List<int>(trits);
            while(tritsAsList.Count % HASH_LENGTH != 0)
            {
                tritsAsList.Add(0);
            }
            trits = tritsAsList.ToArray();

            while (offset < length)
            {
                Array.Copy(trits, offset, tritState, 0, HASH_LENGTH);
                tritState[HASH_LENGTH - 1] = 0;
                
                byte[] bytes = ConvertTritsToBytes(tritState, BYTE_HASH_LENGTH);
                
                keccak.BlockUpdate(bytes, 0, bytes.Length);

                offset += HASH_LENGTH;
            }

            return this;
        }

        public ISponge Absorb(int[] trits)
        {
            Absorb(trits, 0, trits.Length);
            return this;
        }

        public int[] Squeeze(int length = HASH_LENGTH)
        {
            int[] digest = new int[length];
            int offset = 0;

            while (offset < length)
            {
                keccak.DoFinal(byteState, 0);
                tritState = ConvertBytesToTrits(byteState);
                tritState = ArrayUtils.PadArrayWithZeros(tritState, HASH_LENGTH);
                tritState[HASH_LENGTH - 1] = 0;
                Array.Copy(tritState, 0, digest, offset, HASH_LENGTH);

                keccak.Reset();

                for (int i = this.byteState.Length; i-- > 0;)
                {
                    byteState[i] = (byte)(byteState[i] ^ 0xFF);
                }

                keccak.BlockUpdate(this.byteState, 0, this.byteState.Length);
                offset += HASH_LENGTH;
            }

            return digest;
        }

        public ISponge Reset()
        {
            keccak.Reset();
            return this;
        }

        public ISponge Transform()
        {
            throw new NotImplementedException();
        }
    }
}