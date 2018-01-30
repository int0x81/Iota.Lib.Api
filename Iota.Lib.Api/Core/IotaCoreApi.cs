using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iota.Lib.Utils;

namespace Iota.Lib.Core
{
    /// <summary>
    /// This class provides access to the Iota core API
    /// </summary>
    public class IotaCoreApi
    {
        private readonly GenericIotaCoreApi _genericIotaCoreApi;

        /// <summary>
        /// Creates a core api object that uses the specified connection settings to connect to a node
        /// </summary>
        /// <param name="host">hostname or API address of a node to interact with</param>
        /// <param name="port">tcp/udp port</param>
        /// <param name="is_ssl">States if the connection you want to establish is using ssl-encryption (https)</param>
        public IotaCoreApi(string host, int port, bool is_ssl)
        {
            _genericIotaCoreApi = new GenericIotaCoreApi(host, port, is_ssl);
        }

        /// <summary>
        /// Attaches the specified transactions (trytes) to the Tangle by doing Proof of Work. 
        /// You need to supply branchTransaction as well as trunkTransaction.
        /// </summary>
        /// <param name="trunkTransaction">Trunk transaction to approve.</param>
        /// <param name="branchTransaction">Branch transaction to approve.</param>
        /// <param name="trytes">List of trytes (raw transaction data) to attach to the tangle.</param>
        /// <param name="minWeightMagnitude">Proof of Work intensity.</param>
        /// <returns>The returned value contains a different set of tryte values which you can input into broadcastTransactions and storeTransactions. 
        /// The returned tryte value, the last 243 trytes basically consist of the: trunkTransaction + branchTransaction + nonce. 
        /// These are valid trytes which are then accepted by the network.</returns>
        public AttachToTangleResponse AttachToTangle(string trunkTransaction, string branchTransaction, List<string> trytes, int minWeightMagnitude = Constants.MIN_WEIGHT_MAGNITUDE)
        {
            InputValidator.CheckIfArrayOfTrytes(trytes.ToArray());

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction, trytes, minWeightMagnitude);
            return _genericIotaCoreApi.Request<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }

        /// <summary>
        /// Attaches the specified transactions (trytes) asynchronously to the Tangle by doing Proof of Work. 
        /// You need to supply branchTransaction as well as trunkTransaction.
        /// </summary>
        /// <param name="trunkTransaction">Trunk transaction to approve.</param>
        /// <param name="branchTransaction">Branch transaction to approve.</param>
        /// <param name="trytes">List of trytes (raw transaction data) to attach to the tangle.</param>
        /// <param name="minWeightMagnitude">Proof of Work intensity.</param>
        /// <returns>The returned value contains a different set of tryte values which you can input into broadcastTransactions and storeTransactions. 
        /// The returned tryte value, the last 243 trytes basically consist of the: trunkTransaction + branchTransaction + nonce. 
        /// These are valid trytes which are then accepted by the network.</returns>
        public async Task<AttachToTangleResponse> AttachToTangleAsync(string trunkTransaction, string branchTransaction, List<string> trytes, int minWeightMagnitude = Constants.MIN_WEIGHT_MAGNITUDE)
        {
            InputValidator.CheckIfArrayOfTrytes(trytes.ToArray());

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction, trytes, minWeightMagnitude);
            return await _genericIotaCoreApi.RequestAsync<AttachToTangleRequest, AttachToTangleResponse>(attachToTangleRequest);
        }
        
        /// <summary>
        /// Broadcast a list of transactions to all neighbors. The input trytes for this call are provided by attachToTangle.
        /// </summary>
        /// <param name="trytes">List of raw data of transactions to be broadcasted</param>
        /// <returns>the BroadcastTransactionsResponse <see cref="BroadcastTransactionsResponse"/></returns>
        public BroadcastTransactionsResponse BroadcastTransactions(List<string> trytes)
        {
            return _genericIotaCoreApi.Request<BroadcastTransactionsRequest, BroadcastTransactionsResponse>(new BroadcastTransactionsRequest(trytes));
        }

        /// <summary>
        /// Broadcast a list of transactions to all neighbors asynchronously. The input trytes for this call are provided by attachToTangle.
        /// </summary>
        /// <param name="trytes">List of raw data of transactions to be broadcasted</param>
        /// <returns>the BroadcastTransactionsResponse <see cref="BroadcastTransactionsResponse"/></returns>
        public async Task<BroadcastTransactionsResponse> BroadcastTransactionsAsync(List<string> trytes)
        {
            return await _genericIotaCoreApi.RequestAsync<BroadcastTransactionsRequest, BroadcastTransactionsResponse>(new BroadcastTransactionsRequest(trytes));
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
        /// Returns the confirmed balance which a list of addresses have at the latest confirmed milestone. 
        /// In addition to the balances, it also returns the milestone as well as the index with which the confirmed balance was determined. 
        /// The balances is returned as a list in the same order as the addresses were provided as input.
        /// </summary>
        /// <param name="addresses">List of addresses you want to get the confirmed balance from.</param>
        /// <param name="threshold">Confirmation threshold.</param>
        /// <returns>The confirmed balance which a list of addresses have at the latest confirmed milestone and the milestone itself.</returns>
        public GetBalancesResponse GetBalances(List<string> addresses, int threshold = 100)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses, threshold);
            return _genericIotaCoreApi.Request<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        /// <summary>
        /// Returns the confirmed balance which a list of addresses have at the latest confirmed milestone asynchronously. 
        /// In addition to the balances, it also returns the milestone as well as the index with which the confirmed balance was determined. 
        /// The balances is returned as a list in the same order as the addresses were provided as input.
        /// </summary>
        /// <param name="addresses">List of addresses you want to get the confirmed balance from.</param>
        /// <param name="threshold">Confirmation threshold.</param>
        /// <returns>The confirmed balance which a list of addresses have at the latest confirmed milestone and the milestone itself.</returns>
        public async Task<GetBalancesResponse> GetBalancesAsync(List<string> addresses, int threshold = 100)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses, threshold);
            return await _genericIotaCoreApi.RequestAsync<GetBalancesRequest, GetBalancesResponse>(getBalancesRequest);
        }

        /// <summary>
        /// Returns a list of boolean values in the same order as the transaction list you submitted,
        /// thus you get a true/false whether a transaction is confirmed or not.
        /// </summary>
        /// <param name="transactions">List of transactions you want to get the inclusion state for.</param>
        /// <param name="tips">List of tips (including milestones) you want to search for the inclusion state.</param>
        /// <returns>a GetInclusionStatesResponse, see <see cref="GetInclusionStatesResponse"/></returns>
        public GetInclusionStatesResponse GetInclusionStates(string[] transactions, string[] tips)
        {
            return _genericIotaCoreApi.Request<GetInclusionStatesRequest, GetInclusionStatesResponse>(new GetInclusionStatesRequest(transactions, tips));
        }

        /// <summary>
        /// Returns a list of boolean values asynchronously in the same order as the transaction list you submitted,
        /// thus you get a true/false whether a transaction is confirmed or not.
        /// </summary>
        /// <param name="transactions">List of transactions you want to get the inclusion state for.</param>
        /// <param name="tips">List of tips (including milestones) you want to search for the inclusion state.</param>
        /// <returns>a GetInclusionStatesResponse, see <see cref="GetInclusionStatesResponse"/></returns>
        public async Task<GetInclusionStatesResponse> GetInclusionStatesAsync(string[] transactions, string[] tips)
        {
            return await _genericIotaCoreApi.RequestAsync<GetInclusionStatesRequest, GetInclusionStatesResponse>(new GetInclusionStatesRequest(transactions, tips));
        }

        /// <summary>
        /// Stores the specified transactions into the local storage. The trytes to be used for this call are returned by attachToTangle.
        /// </summary>
        /// <param name="trytes">List of raw data of transactions to be rebroadcast</param>
        /// <returns>A <see cref="StoreTransactionsResponse"/></returns>
        public StoreTransactionsResponse StoreTransactions(List<string> trytes)
        {
            return _genericIotaCoreApi.Request<StoreTransactionsRequest, StoreTransactionsResponse>(new StoreTransactionsRequest(trytes));
        }

        /// <summary>
        /// Stores the specified transactions into the local storage. The trytes to be used for this call are returned by attachToTangle.
        /// </summary>
        /// <param name="trytes">List of raw data of transactions to be rebroadcast</param>
        /// <returns>A <see cref="StoreTransactionsResponse"/></returns>
        public async Task<StoreTransactionsResponse> StoreTransactionsAsync(List<string> trytes)
        {
            return await _genericIotaCoreApi.RequestAsync<StoreTransactionsRequest, StoreTransactionsResponse>(new StoreTransactionsRequest(trytes));
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
        /// Returns two transactions to approve.
        /// </summary>
        /// <param name="depth">Number of bundles to go back to determine the transactions for approval.</param>
        /// <returns>A trunkTransaction and a branchTransaction</returns>
        public GetTransactionsToApproveResponse GetTransactionsToApprove(int depth)
        {
            return _genericIotaCoreApi.Request<GetTransactionsToApproveRequest, GetTransactionsToApproveResponse>(new GetTransactionsToApproveRequest(depth));
        }

        /// <summary>
        /// Returns two transactions to approve asychronously.
        /// </summary>
        /// <param name="depth">Number of bundles to go back to determine the transactions for approval.</param>
        /// <returns>A trunkTransaction and a branchTransaction</returns>
        public async Task<GetTransactionsToApproveResponse> GetTransactionsToApproveAsync(int depth)
        {
            return await _genericIotaCoreApi.RequestAsync<GetTransactionsToApproveRequest, GetTransactionsToApproveResponse>(new GetTransactionsToApproveRequest(depth));
        }

        /// <summary>
        /// Returns the raw transaction data (trytes) of a specific transaction.
        /// </summary>
        /// <param name="hashes">The hashes of the transactions</param>
        /// <returns>A <see cref="GetTrytesResponse"/> containing a list of trytes</returns>
        public GetTrytesResponse GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() {Hashes = hashes};
            return _genericIotaCoreApi.Request<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        /// <summary>
        /// Returns the raw transaction data (trytes) of a specific transaction asynchronously.
        /// </summary>
        /// <param name="hashes">The hashes of the transactions</param>
        /// <returns>A <see cref="GetTrytesResponse"/> containing a list of trytes</returns>
        public async Task<GetTrytesResponse> GetTrytesAsync(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() { Hashes = hashes };
            return await _genericIotaCoreApi.RequestAsync<GetTrytesRequest, GetTrytesResponse>(getTrytesRequest);
        }

        /// <summary>
        /// Interrupts and completely aborts the attachToTangle process.
        /// </summary>
        /// <returns>An <see cref="InterruptAttachingToTangleResponse"/></returns>
        public InterruptAttachingToTangleResponse InterruptAttachingToTangle()
        {
            return _genericIotaCoreApi.Request<InterruptAttachingToTangleRequest, InterruptAttachingToTangleResponse>(new InterruptAttachingToTangleRequest());
        }

        /// <summary>
        /// Interrupts and completely aborts the attachToTangle process.
        /// </summary>
        /// <returns>An <see cref="InterruptAttachingToTangleResponse"/></returns>
        public async Task<InterruptAttachingToTangleResponse> InterruptAttachingToTangleAsync()
        {
            return await _genericIotaCoreApi.RequestAsync<InterruptAttachingToTangleRequest, InterruptAttachingToTangleResponse>(new InterruptAttachingToTangleRequest());
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