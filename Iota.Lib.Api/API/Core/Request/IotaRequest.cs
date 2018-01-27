using Iota.Lib.Api.Utils;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Base class for all api requests
    /// </summary>
    public abstract class IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IotaRequest"/> class.
        /// </summary>
        /// <param name="command">The command</param>
        public IotaRequest(Core.Command command)
        {
            Command = command.GetCommandString();
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command
        /// </value>
        public string Command { get; set; }
    }
}