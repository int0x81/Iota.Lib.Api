namespace Iota.Lib.CSharp.Api.Utils
{
    /// <summary>
    /// This interface abstracts the curl hashing algorithm
    /// </summary>
    public interface ISponge
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        ISponge Clone();

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="length">The length.</param>
        /// <returns>the ISpongeFunction instance (used for method chaining)</returns>
        ISponge Absorb(int[] trits, int offset, int length);

        /// <summary>
        /// Absorbs the specified trits.
        /// </summary>
        /// <param name="trits">The trits.</param>
        /// <returns>the ISpongeFunction instance (used for method chaining)</returns>
        ISponge Absorb(int[] trits);

        /// <summary>
        /// Squeezes the specified trits.
        /// </summary>
        /// <param name="length">The desired outputlength.</param>
        /// <returns>the squeezed trits</returns>
        int[] Squeeze(int length);

        /// <summary>
        /// Transforms this instance.
        /// </summary>
        /// <returns>the ISpongeFunction instance (used for method chaining)</returns>
        ISponge Transform();

        /// <summary>
        /// Resets this state.
        /// </summary>
        /// <returns>the ISpongeFunction instance (used for method chaining)</returns>
        ISponge Reset();
    }
}