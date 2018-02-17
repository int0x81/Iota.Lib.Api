using System.Collections.Generic;

namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents the core api request 'FindTransactions'
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class FindTransactionsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindTransactionsRequest"/> class
        /// </summary>
        /// <param name="bundles">The bundles</param>
        /// <param name="addresses">The addresses</param>
        /// <param name="tags">The tags</param>
        /// <param name="approves">The approvees</param>
        public FindTransactionsRequest(List<string> bundles, List<string> addresses, List<string> tags, List<string> approves) : base(Core.Command.FindTransactions)
        {
            Bundles = bundles;
            Addresses = addresses;
            Tags = tags;
            Approves = approves;

            if (Bundles == null)
                Bundles = new List<string>();
            if (Addresses == null)
                Addresses = new List<string>();
            if (Tags == null)
                Tags = new List<string>();
            if (Approves == null)
                Approves = new List<string>();
        }

        /// <summary>
        /// The bundles
        /// </summary>
        public List<string> Bundles { get; set; }

        /// <summary>
        /// The addresses
        /// </summary>
        public List<string> Addresses { get; set; }

        /// <summary>
        /// The tags
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// The approvees
        /// </summary>
        public List<string> Approves { get; set; }
    }
}