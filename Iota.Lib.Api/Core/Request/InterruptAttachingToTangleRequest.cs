namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents the core api request 'InterruptAttachingToTangle'
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class InterruptAttachingToTangleRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterruptAttachingToTangleRequest"/> class.
        /// </summary>
        public InterruptAttachingToTangleRequest() : base(Core.Command.InterruptAttachingToTangle)
        {
        }
    }
}