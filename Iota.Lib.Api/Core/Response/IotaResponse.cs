using System.Net;

namespace Iota.Lib.Core
{
    /// <summary>
    /// Base class for all api responses
    /// </summary>
    public abstract class IotaResponse
    {
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public long Duration { get; set; }

        /// <summary>
        /// The Status Code
        /// </summary>
        public HttpStatusCode StatusCode;
    }
}