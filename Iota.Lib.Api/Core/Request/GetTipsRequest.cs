namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents the core API request 'GetTips'
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class GetTipsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTipsRequest"/> class.
        /// </summary>
        public GetTipsRequest() : base(Core.Command.GetTips)
        {
        }
    }
}