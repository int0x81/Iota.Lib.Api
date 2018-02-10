namespace Iota.Lib.Utils
{
    /// <summary>
    /// This interface defines all methods a sponge-function has to implement
    /// </summary>
    public interface ISponge
    {
        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <param name="offset">The offset to start from</param>
        /// <param name="length">The length</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        ISponge Absorb(int[] trits, int offset, int length);

        /// <summary>
        /// Absorbs the specified trits
        /// </summary>
        /// <param name="trits">The trits</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        ISponge Absorb(int[] trits);

        /// <summary>
        /// Squeezes the absorbed trits
        /// </summary>
        /// <param name="array">The array</param>
        /// <param name="offset">The offset</param>
        /// <param name="length">The desired outputlength</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        ISponge Squeeze(ref int[] array, int offset, int length);

        /// <summary>
        /// Squeezes the absorbed trits
        /// </summary>
        /// <param name="array">The array</param>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        ISponge Squeeze(ref int[] array);

        /// <summary>
        /// Resets the state
        /// </summary>
        /// <returns>An <see cref="ISponge"/> instance (used for method chaining)</returns>
        ISponge Reset();
    }
}