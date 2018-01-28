using System.Collections.Generic;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Represents the core api request 'GetBalances'
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class GetBalancesRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetBalancesRequest"/> class.
        /// </summary>
        /// <param name="addresses">List of addresses you want to get the confirmed balance from.</param>
        /// <param name="threshold">Confirmation treshold</param>
        public GetBalancesRequest(List<string> addresses, int threshold = 100): base(Core.Command.GetBalances)
        {
            Addresses = addresses;
            Threshold = threshold;
        }

        /// <summary>
        /// Gets the threshold.
        /// </summary>
        /// <value>
        /// Confirmation treshold
        /// </value>
        public int Threshold { get; }

        /// <summary>
        /// Gets the addresses.
        /// </summary>
        /// <value>
        /// List of addresses you want to get the confirmed balance from.
        /// </value>
        public List<string> Addresses { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Threshold)}: {Threshold}, {nameof(Addresses)}: {string.Join(",",Addresses)}";
        }
    }
}