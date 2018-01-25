namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Represents the core API request 'GetInclusionStates'
    /// </summary>
    public class GetInclusionStatesRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetInclusionStatesRequest"/> class.
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="tips">The tips.</param>
        public GetInclusionStatesRequest(string[] transactions, string[] tips) : base(Command.GetInclusionStates)
        {
            Transactions = transactions;
            Tips = tips;
        }

        /// <summary>
        /// Gets the transactions.
        /// </summary>
        /// <value>
        /// The transactions.
        /// </value>
        public string[] Transactions { get; }

        /// <summary>
        /// Gets the tips.
        /// </summary>
        /// <value>
        /// The tips.
        /// </value>
        public string[] Tips { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Transactions)}: {Transactions}, {nameof(Tips)}: {Tips}";
        }
    }
}