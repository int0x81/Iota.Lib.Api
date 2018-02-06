#region Acknowledgements
/**
 * The code on this class is heavily based on:
 * 
 * https://github.com/iotaledger/iri/blob/dev/src/main/java/com/iota/iri/hash/Curl.java
 * (c) 2016 Come-from-Beyond and Paul Handy
 */
#endregion

using System;

namespace Iota.Lib.Utils
{
    public class Curl : ISponge
    {
        /// <summary>
        /// The hash length
        /// </summary>
        public const int HASH_LENGTH = 243;
        private static readonly int StateLength = 3* HASH_LENGTH;
        private const int NumberOfRounds = 27;
        private static readonly int[] TruthTable = {1, 0, -1, 1, -1, 0, -1, 1, 0};

        public int[] State { get; set; } = new int[StateLength];

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>a new instance</returns>
        public ISponge Clone()
        {
            return new Curl();
        }

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ICurl instance (used for method chaining)</returns>
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
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>
        /// the ICurl instance (used for method chaining)
        /// </returns>
        public ISponge Absorb(int[] trits)
        {
            Absorb(trits, 0, trits.Length);
            return this;
        }

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// the squeezed trits
        /// </returns>
        public int[] Squeeze(int length)
        {
            int[] digest = new int[length];
            int offset = 0;

            do
            {
                Array.Copy(State, 0, digest, offset, length < HASH_LENGTH ? length : HASH_LENGTH);
                Transform();
                offset += HASH_LENGTH;
            } while ((length -= HASH_LENGTH) > 0);

            return digest;
        }

        /// <summary>
        /// Transforms this instance.
        /// </summary>
        /// <returns>
        /// the ICurl instance (used for method chaining)
        /// </returns>
        public Curl Transform()
        {
            int[] scratchpad = new int[StateLength];
            int scratchpadIndex = 0;
            for (int round = 0; round < NumberOfRounds; round++)
            {
                Array.Copy(State, 0, scratchpad, 0, StateLength);
                for (int stateIndex = 0; stateIndex < StateLength; stateIndex++)
                {
                    State[stateIndex] =
                        TruthTable[
                            scratchpad[scratchpadIndex] +
                            scratchpad[scratchpadIndex += (scratchpadIndex < 365 ? 364 : -365)]*3 + 4];
                }
            }

            return this;
        }

        /// <summary>
        /// Resets this state.
        /// </summary>
        /// <returns>
        /// the ICurl instance (used for method chaining)
        /// </returns>
        public ISponge Reset()
        {
            for (int stateIndex = 0; stateIndex < StateLength; stateIndex++)
            {
                State[stateIndex] = 0;
            }
            return this;
        }
    }
}