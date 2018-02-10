using System.ComponentModel;

namespace Iota.Lib.Core
{
    /// <summary>
    /// This enumeration defines the core API call commands as specified on <see href="https://iota.readme.io/reference"/>
    /// </summary>
    public enum Command
    {
        /// <summary>
        /// Gets information about the node
        /// </summary>
        [Description("getNodeInfo")] GetNodeInfo,

        /// <summary>
        /// Gets the tips of the node
        /// </summary>
        [Description("getTips")] GetTips,

        /// <summary>
        /// Finds the transactions using different search criteria
        /// </summary>
        [Description("findTransactions")] FindTransactions,

        /// <summary>
        /// Gets two transactions to approve
        /// </summary>
        [Description("getTransactionsToApprove")] GetTransactionsToApprove,

        /// <summary>
        /// Attaches transactions to the tangle
        /// </summary>
        [Description("attachToTangle")] AttachToTangle,

        /// <summary>
        /// Gets the balances of a list of addresses
        /// </summary>
        [Description("getBalances")] GetBalances,

        /// <summary>
        /// Gets the inclusion states of a set of transactions
        /// </summary>
        [Description("getInclusionStates")] GetInclusionStates,

        /// <summary>
        /// Gets the raw transaction data of a specific transaction
        /// </summary>
        [Description("getTrytes")] GetTrytes,

        /// <summary>
        /// Gets the neighbours of the node
        /// </summary>
        [Description("getNeighbors")] GetNeighbors,

        /// <summary>
        /// Adds neighbours to the node
        /// </summary>
        [Description("addNeighbors")] AddNeighbors,

        /// <summary>
        /// Removes neighbours from the node
        /// </summary>
        [Description("removeNeighbors")] RemoveNeighbors,

        /// <summary>
        /// Interrupt attaching to the tangle
        /// </summary>
        [Description("interruptAttachingToTangle")] InterruptAttachingToTangle,

        /// <summary>
        /// Broadcasts transactions
        /// </summary>
        [Description("broadcastTransactions")] BroadcastTransactions,

        /// <summary>
        /// Stores transactions locally
        /// </summary>
        [Description("storeTransactions")] StoreTransactions
    }
}