using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Crypto.Digests;
using static Iota.Lib.Utils.Converter;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// Kerl is the hashfunction Iota uses for multiple scenarios such as address generation, signature generation etc.
    /// It is sponge-function based on Keccak-384
    /// </summary>
    /// <seealso href="https://github.com/iotaledger/kerl"/>
    /// <seealso href="https://keccak.team/keccak.html"/>
    public class Kerl : ISponge
    {
        private readonly KeccakDigest keccak;
        private const int BIT_HASH_LENGTH = 384;
        private const int BYTE_HASH_LENGTH = BIT_HASH_LENGTH / 8;
        private byte[] byteState;
        private int[] tritState;
        
        /// <summary>
        /// The array length of one 'squeeze' in trits 
        /// </summary>
        public const int HASH_LENGTH = 243;

        /// <summary>
        /// Creates a new <see cref="Kerl"/> instance
        /// </summary>
        public Kerl() 
        {
            keccak = new KeccakDigest(BIT_HASH_LENGTH);
            byteState = new byte[BYTE_HASH_LENGTH];
            tritState = new int[HASH_LENGTH];
        }

        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <param name="offset">The offset to start from</param>
        /// <param name="length">The length</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
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

        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        public ISponge Absorb(int[] trits)
        {
            return Absorb(trits, 0, trits.Length);
        }

        /// <summary>
        /// Squeezes the absorbed trits
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="offset">The offset</param>
        /// <param name="length">The desired outputlength</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        public ISponge Squeeze(ref int[] array, int offset, int length)
        {
            var tritsList = array.ToList();
            while(tritsList.Count % HASH_LENGTH != 0)
            {
                tritsList.Add(0);
            }
            array = tritsList.ToArray();

            do
            {
                keccak.DoFinal(byteState, 0);
                tritState = Converter.ConvertBytesToTrits(byteState);
                tritState = ArrayUtils.PadArrayWithZeros(tritState, HASH_LENGTH);
                tritState[HASH_LENGTH - 1] = 0;
                Array.Copy(tritState, 0, array, offset, HASH_LENGTH);

                for (int i = byteState.Length; i-- > 0;)
                {

                    byteState[i] = (byte)(byteState[i] ^ 0xFF);
                }
                keccak.BlockUpdate(byteState, 0, byteState.Length);
                offset += HASH_LENGTH;

            } while ((length -= HASH_LENGTH) > 0);

            return this;
        }

        /// <summary>
        /// Squeezes the absorbed trits
        /// </summary>
        /// <param name="array">The array</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        public ISponge Squeeze(ref int[] array)
        {
            return Squeeze(ref array, 0, array.Length);
        }

        /// <summary>
        /// Resets the state
        /// </summary>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        public ISponge Reset()
        {
            keccak.Reset();
            return this;
        }
    }
}