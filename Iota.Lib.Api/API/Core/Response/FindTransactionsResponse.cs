using System.Collections.Generic;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Response of <see cref="FindTransactionsRequest"/>
    /// </summary>
    /// <seealso cref="IotaResponse"/>
    public class FindTransactionsResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the hashes.
        /// </summary>
        /// <value>
        /// The hashes.
        /// </value>
        public List<string> Hashes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Hashes)}: {string.Join(",",Hashes)}";
        }
    }
}