using System.Collections.Generic;

namespace Iota.Lib.Core
{
    /// <summary>
    /// Response of <see cref="AttachToTangleRequest"/>
    /// </summary>
    /// <seealso cref="IotaResponse"/>
    public class AttachToTangleResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the trytes.
        /// </summary>
        /// <value>
        /// The trytes.
        /// </value>
        public List<string> Trytes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Trytes)}: {string.Join(",", Trytes)}";
        }
    }
}