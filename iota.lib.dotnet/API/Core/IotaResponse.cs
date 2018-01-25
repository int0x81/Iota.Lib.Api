namespace Iota.Lib.CSharp.Api.Core
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
    }
}