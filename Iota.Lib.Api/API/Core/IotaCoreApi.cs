using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iota.Lib.Api.Utils;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// This class provides access to the Iota core API
    /// </summary>
    public class IotaCoreApi
    {
        private readonly IGenericIotaCoreApi _genericIotaCoreApi;

        /// <summary>
        /// Creates a core api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        public IotaCoreApi(string host, int port)
        {
            _genericIotaCoreApi = new GenericIotaCoreApi(host, port);
        }

        /// <summary>
        /// Attaches the specified transactions (trytes) to the Tangle by doing Proof of Work. 
        /// You need to supply branchTransaction as well as trunkTransaction 
        /// (basically the tips which you're going to validate and reference with this transaction)
        ///  - both of which you'll get through the getTransactionsToApprove API call.
        /// </summary>
        /// <param name="trunkTransaction">Trunk transaction to approve.</param>
        /// <param name="branchTransaction">Branch transaction to approve.</param>
        /// <param name="trytes">List of trytes (raw transaction data) to attach to the tangle.</param>
        /// <param name="minWeightMagnitude">Proof of Work intensity. Minimum value is 18</param>
        /// <returns>The returned value contains a different set of tryte values which you can input into broadcastTransactions and storeTransactions. 
        /// The returned tryte value, the last 243 trytes basically consist of the: trunkTransaction + branchTransaction + nonce. 
        /// These are valid trytes which are then accepted by the network.</returns>
        public AttachToTangleResponse AttachToTangle(string trunkTransaction, string branchTransaction, string[] trytes)
        {
            InputValidator.CheckIfArrayOfTrytes(trytes);

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction, trytes, Constants.MIN_WEIGHT_MAGNITUDE);
            return _genericIotaCoreApi.Request<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }

        /// <summary>
        /// Broadcasts the transactions.
        /// </summary>
        /// <param name="trytes">The transactions in trytes representation</param>
        /// <returns>the BroadcastTransactionsResponse <see cref="BroadcastTransactionsResponse"/></returns>
        public BroadcastTransactionsResponse BroadcastTransactions(List<string> trytes)
        {
            return _genericIotaCoreApi.Request<BroadcastTransactionsRequest, BroadcastTransactionsResponse>(new BroadcastTransactionsRequest(trytes));
        }

        /// <summary>
        /// Find the transactions which match the specified input and return.
        /// All input values are lists, for which a list of return values (transaction hashes), in the same order, is returned for all individual elements.
        /// The input fields can either be bundles, addresses, tags or approvees.
        /// Using multiple of these input fields returns the intersection of the values.
        /// </summary>
        /// <param name="bundles">List of bundle hashes. The hashes need to be extended to 81chars by padding the hash with 9's.</param>
        /// <param name="addresses">List of addresses.</param>
        /// <param name="tags">List of transaction tags.</param>
        /// <param name="approves">List of approves of a transaction.</param>
        /// <returns>A <see cref="FindTransactionsResponse"/> that contains a list of hashes</returns>
        public FindTransactionsResponse FindTransactions(List<string> bundles = null, List<string> addresses = null, List<string> tags = null, List<string> approves = null)
        {
            FindTransactionsRequest findTransactionsRequest = new FindTransactionsRequest(bundles, addresses, tags, approves);
            return  _genericIotaCoreApi.Request<FindTransactionsRequest, FindTransactionsResponse>(findTransactionsRequest);
        }

        /// <summary>
        /// Find the transactions which match the specified input asynchronously and return.
        /// All input values are lists, for which a list of return values (transaction hashes), in the same order, is returned for all individual elements.
        /// The input fields can either be bundles, addresses, tags or approvees.
        /// Using multiple of these input fields returns the intersection of the values.
        /// </summary>
        /// <param name="bundles">List of bundle hashes. The hashes need to be extended to 81chars by padding the hash with 9's.</param>
        /// <param name="addresses">List of addresses.</param>
        /// <param name="tags">List of transaction tags.</param>
        /// <param name="approves">List of approves of a transaction.</param>
        /// <returns>A <see cref="FindTransactionsResponse"/> that contains a list of hashes</returns>
        public async Task<FindTransactionsResponse> FindTransactionsAsync(List<string> bundles = null, List<string> addresses = null, List<string> tags = null, List<string> approves = null)
        {
            FindTransactionsRequest findTransactionsRequest = new FindTransactionsRequest(bundles, addresses, tags, approves);
            return await _genericIotaCoreApi.RequestAsync<FindTransactionsRequest, FindTransactionsResponse>(findTransactionsRequest);
        }

        /// <summary>
        /// Gets the balances.
        /// </summary>
        /// <param name="addresses">The addresses.</param>
        /// <param name="threshold">The threshold.</param>
        /// <returns> It returns the confirmed balance which a list of addresses have at the latest confirmed milestone. 
        /// In addition to the balances, it also returns the milestone as well as the index with which the confirmed balance was determined. 
        /// The balances is returned as a list in the same order as the addresses were provided as input.</returns>
        public GetBalancesResponse GetBalances(List<string> addresses, long threshold = 100)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses, threshold);
            return _genericIotaCoreApi.Request<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        /// <summary>
        /// Gets the inclusion states of the specified transactions
        /// </summary>
        /// <param name="transactions">The transactions.</param>
        /// <param name="milestones">The milestones.</param>
        /// <returns>a GetInclusionStatesResponse, see <see cref="GetInclusionStatesResponse"/></returns>
        public GetInclusionStatesResponse GetInclusionStates(string[] transactions, string[] milestones)
        {
            return _genericIotaCoreApi.Request<GetInclusionStatesRequest, GetInclusionStatesResponse>(new GetInclusionStatesRequest(transactions, milestones));
        }

        /// <summary>
        /// Stores the specified transactions in trytes into the local storage. The trytes to be used for this call are returned by attachToTangle.
        /// </summary>
        /// <param name="trytes">The trytes representing the transactions</param>
        /// <returns>a <see cref="StoreTransactionsResponse"/></returns>
        public StoreTransactionsResponse StoreTransactions(List<string> trytes)
        {
            return
                _genericIotaCoreApi.Request<StoreTransactionsRequest, StoreTransactionsResponse>(new StoreTransactionsRequest(trytes));
        }

        /// <summary>
        /// Returns information about your node.
        /// </summary>
        /// <returns>A <see cref="GetNodeInfoResponse"/> containing information about the node.</returns>
        public GetNodeInfoResponse GetNodeInfo()
        {
            return _genericIotaCoreApi.Request<GetNodeInfoRequest, GetNodeInfoResponse>(new GetNodeInfoRequest());
        }

        /// <summary>
        /// Returns information about your node.
        /// </summary>
        /// <returns>A <see cref="GetNodeInfoResponse"/> containing information about the node.</returns>
        public async Task<GetNodeInfoResponse> GetNodeInfoAsync()
        {
            return await _genericIotaCoreApi.RequestAsync<GetNodeInfoRequest, GetNodeInfoResponse>(new GetNodeInfoRequest());
        }

        /// <summary>
        /// Returns the list of tips.
        /// </summary>
        /// <returns>A <see cref="GetTipsResponse"/> containing a list of tips</returns>
        public GetTipsResponse GetTips()
        {
            return _genericIotaCoreApi.Request<GetTipsRequest, GetTipsResponse>(new GetTipsRequest());
        }

        /// <summary>
        /// Returns the list of tips asynchronously.
        /// </summary>
        /// <returns>A <see cref="GetTipsResponse"/> containing a list of tips</returns>
        public async Task<GetTipsResponse> GetTipsAsync()
        {
            return await _genericIotaCoreApi.RequestAsync<GetTipsRequest, GetTipsResponse>(new GetTipsRequest());
        }

        /// <summary>
        /// Gets the transactions to approve.
        /// </summary>
        /// <param name="depth">The depth is the number of bundles to go back to determine the transactions for approval. 
        /// The higher your depth value, the more "babysitting" you do for the network (as you have to confirm more transactions).</param>
        /// <returns> trunkTransaction and branchTransaction (result of the Tip selection)</returns>
        public GetTransactionsToApproveResponse GetTransactionsToApprove(int depth)
        {
            return _genericIotaCoreApi.Request<GetTransactionsToApproveRequest, GetTransactionsToApproveResponse>(new GetTransactionsToApproveRequest(depth));
        }

        /// <summary>
        /// Gets the raw transaction data (trytes) of a specific transaction.
        /// These trytes can then be easily converted into the actual transaction object using the constructor of Transaction
        /// </summary>
        /// <param name="hashes">The hashes of the transactions</param>
        /// <returns>a <see cref="GetTrytesResponse"/> containing a list of trytes</returns>
        public GetTrytesResponse GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() {Hashes = hashes};
            return _genericIotaCoreApi.Request<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        /// <summary>
        /// Interrupts and completely aborts the attachToTangle process.
        /// </summary>
        /// <returns>an <see cref="InterruptAttachingToTangleResponse"/></returns>
        public InterruptAttachingToTangleResponse InterruptAttachingToTangle()
        {
            return _genericIotaCoreApi.Request<InterruptAttachingToTangleRequest, InterruptAttachingToTangleResponse>(new InterruptAttachingToTangleRequest());
        }

        /// <summary>
        /// Returns the set of neighbors you are connected with, as well as their activity count. The activity counter is reset after restarting IRI.
        /// </summary>
        /// <returns>A <see cref="GetNeighborsResponse"/> containing the set of neighbors the node is connected to as well as their activity count. The activity counter is reset after restarting IRI.</returns>
        public GetNeighborsResponse GetNeighbors()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            return _genericIotaCoreApi.Request<GetNeighborsRequest, GetNeighborsResponse>(getNeighborsRequest);
        }

        /// <summary>
        /// Returns the set of neighbors you are connected with asychronously, as well as their activity count. The activity counter is reset after restarting IRI.
        /// </summary>
        /// <returns>A <see cref="GetNeighborsResponse"/> containing the set of neighbors the node is connected to as well as their activity count. The activity counter is reset after restarting IRI.</returns>
        public async Task<GetNeighborsResponse> GetNeighborsAsync()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            return await _genericIotaCoreApi.RequestAsync<GetNeighborsRequest, GetNeighborsResponse>(getNeighborsRequest);
        }

        /// <summary>
        /// Add a list of neighbors to your node. It should be noted that this is only temporary, and the added neighbors will be removed from your set of neighbors after you relaunch IRI.
        /// </summary>
        /// <param name="uris">List of URI elements. The URI (Unique Resource Identification) for adding neighbors is: udp://IPADDRESS:PORT</param>
        /// <returns><see cref="AddNeighborsResponse"/> containing the number of added Neighbors</returns>
        public AddNeighborsResponse AddNeighbors(params string[] uris)
        {
            return _genericIotaCoreApi.Request<AddNeighborsRequest, AddNeighborsResponse>(new AddNeighborsRequest(uris.ToList()));
        }

        /// <summary>
        /// Add a list of neighbors to your node asynchronously. It should be noted that this is only temporary, and the added neighbors will be removed from your set of neighbors after you relaunch IRI.
        /// </summary>
        /// <param name="uris">List of URI elements. The URI (Unique Resource Identification) for adding neighbors is: udp://IPADDRESS:PORT</param>
        /// <returns><see cref="AddNeighborsResponse"/> containing the number of added Neighbors</returns>
        public async Task<AddNeighborsResponse> AddNeighborsAsync(params string[] uris)
        {
            return await _genericIotaCoreApi.RequestAsync<AddNeighborsRequest, AddNeighborsResponse>(new AddNeighborsRequest(uris.ToList()));
        }

        /// <summary>
        /// Removes a list of neighbors from your node. This is only temporary, and if you have your neighbors added via the command line, they will be retained after you restart your node. 
        /// </summary>
        /// <param name="uris">List of URI elements. The URI (Unique Resource Identification) format for removing neighbors is "udp://IPADDRESS:PORT"</param>
        /// <returns>A <see cref="RemoveNeighborsResponse"/> containing the number of removed neighbors</returns>
        public RemoveNeighborsResponse RemoveNeighbors(params string[] uris)
        {
            return _genericIotaCoreApi.Request<RemoveNeighborsRequest, RemoveNeighborsResponse>(new RemoveNeighborsRequest(uris.ToList()));
        }

        /// <summary>
        /// Removes a list of neighbors from your node. This is only temporary, and if you have your neighbors added via the command line, they will be retained after you restart your node. 
        /// </summary>
        /// <param name="uris">List of URI elements. The URI (Unique Resource Identification) format for removing neighbors is "udp://IPADDRESS:PORT"</param>
        /// <returns>A <see cref="RemoveNeighborsResponse"/> containing the number of removed neighbors</returns>
        public async Task<RemoveNeighborsResponse> RemoveNeighborsAsync(params string[] uris)
        {
            return await _genericIotaCoreApi.RequestAsync<RemoveNeighborsRequest, RemoveNeighborsResponse>(new RemoveNeighborsRequest(uris.ToList()));
        }
    }
}