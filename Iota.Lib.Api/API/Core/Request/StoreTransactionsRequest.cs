using System.Collections.Generic;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Represents the core API request 'StoreTransactions'.
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class StoreTransactionsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreTransactionsRequest"/> class.
        /// </summary>
        /// <param name="trytes">The trytes.</param>
        public StoreTransactionsRequest(List<string> trytes) : base(Core.Command.StoreTransactions)
        {
            this.Trytes = trytes;
        }

        /// <summary>
        /// Gets or sets the trytes.
        /// </summary>
        /// <value>
        /// List of raw data of transactions to be rebroadcast.
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
            return $"{nameof(Trytes)}: {Trytes}";
        }
    }
}