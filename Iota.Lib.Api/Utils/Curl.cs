#region Acknowledgements
/*
 * The code on this class is heavily based on:
 * 
 * https://github.com/iotaledger/iri/blob/dev/src/main/java/com/iota/iri/hash/Curl.java
 * (c) 2016 Come-from-Beyond and Paul Handy
 */
#endregion

using System;

namespace Iota.Lib.Utils
{
    /// <summary>
    /// A sponge function used for transaction hashing, proof of work and milestone verification
    /// </summary>
    public class Curl : ISponge
    {
        private const int HASH_LENGTH = 243;
        private const int STATE_LENGTH = 3* HASH_LENGTH;
        private const int NUMBER_OF_ROUNDSP27 = 27; //used only for milestone verification
        private const int NUMBER_OF_ROUNDSP81 = 81; //used for transaction-hash generation and the proof of work
        private readonly int numberOfRounds;
        private readonly int[] TRUTH_TABLE = { 1, 0, -1, 2, 1, -1, 0, 2, -1, 1, 0 };
        private readonly long[] stateLow;
        private readonly long[] stateHigh;
        private readonly int[] scratchpad = new int[STATE_LENGTH];
        private int[] State;

        /// <summary>
        /// Creates a new <see cref="Curl"/> instance on either 27 or 81 mode. 27 is only used for milestone verification
        /// </summary>
        /// <param name="mode">The mode</param>
        public Curl(int mode)
        {
            if (mode == NUMBER_OF_ROUNDSP27 || mode == NUMBER_OF_ROUNDSP81)
            {
                numberOfRounds = mode; 
            }
            else
            {
                throw new ArgumentException("Mode can only be 27 or 81");
            }

            State = new int[STATE_LENGTH];
            stateHigh = null;
            stateLow = null;
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
            do
            {
                Array.Copy(trits, offset, State, 0, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
                offset += HASH_LENGTH;
            } while ((length -= HASH_LENGTH) > 0);
            return this;
        }

        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        public ISponge Absorb(int[] trits)
        {
            Absorb(trits, 0, trits.Length);
            return this;
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
            do
            {
                Array.Copy(State, 0, array, offset, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
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
            for (int stateIndex = 0; stateIndex < STATE_LENGTH; stateIndex++)
            {
                State[stateIndex] = 0;
            }
            return this;
        }

        /// <summary>
        /// Transforms this instance
        /// </summary>
        /// <returns>A <see cref="Curl"/> instance (used for method chaining)</returns>
        private Curl Transform()
        {
            int scratchpadIndex = 0;
            int prev_scratchpadIndex = 0;
            for (int round = 0; round < numberOfRounds; round++)
            {
                Array.Copy(State, 0, scratchpad, 0, STATE_LENGTH);
                for (int stateIndex = 0; stateIndex < STATE_LENGTH; stateIndex++)
                {
                    prev_scratchpadIndex = scratchpadIndex;
                    if (scratchpadIndex < 365)
                    {
                        scratchpadIndex += 364;
                    }
                    else
                    {
                        scratchpadIndex += -365;
                    }

                    var testme = scratchpad[prev_scratchpadIndex];
                    var testme02 = scratchpad[scratchpadIndex];
                    var testme03 = scratchpad[prev_scratchpadIndex] + (scratchpad[scratchpadIndex] << 2) + 5;

                    State[stateIndex] = TRUTH_TABLE[scratchpad[prev_scratchpadIndex] + (scratchpad[scratchpadIndex] << 2) + 5];
                }
            }

            return this;
        }
    }
}