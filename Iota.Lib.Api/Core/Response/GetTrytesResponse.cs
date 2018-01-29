using System.Collections.Generic;

namespace Iota.Lib.Core
{
    /// <summary>
    /// This class represents the response of <see cref="GetTrytesRequest"/>
    /// </summary>
    /// <seealso cref="IotaResponse"/>
    public class GetTrytesResponse : IotaResponse
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