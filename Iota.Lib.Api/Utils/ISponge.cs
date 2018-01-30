namespace Iota.Lib.Utils
{
    /// <summary>
    /// This interface defines all methods a sponge-function has to implement
    /// </summary>
    public interface ISponge
    {
        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns></returns>
        ISponge Clone();

        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <param name="offset">The offset to start from</param>
        /// <param name="length">The length</param>
        /// <returns>An ISponge instance (used for method chaining)</returns>
        ISponge Absorb(int[] trits, int offset, int length);

        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <returns>An ISpongeFunction instance (used for method chaining)</returns>
        ISponge Absorb(int[] trits);

        /// <summary>
        /// Squeezes the specified trits
        /// </summary>
        /// <param name="length">The desired outputlength</param>
        /// <returns>The squeezed trits</returns>
        int[] Squeeze(int length);

        /// <summary>
        /// Transforms this instance
        /// </summary>
        /// <returns>An ISponge instance (used for method chaining)</returns>
        ISponge Transform();

        /// <summary>
        /// Resets this state
        /// </summary>
        /// <returns>the ISpongeFunction instance (used for method chaining)</returns>
        ISponge Reset();
    }
}