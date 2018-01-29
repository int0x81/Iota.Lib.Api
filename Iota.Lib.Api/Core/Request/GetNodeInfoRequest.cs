namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents the core API request 'GetNodeInfo'.
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class GetNodeInfoRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNodeInfoRequest"/> class.
        /// </summary>
        public GetNodeInfoRequest() : base(Core.Command.GetNodeInfo)
        {
        }
    }
}