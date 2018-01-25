namespace Iota.Lib.CSharp.Api.Core
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
        public IotaRequest(Command command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <value>
        /// The command
        /// </value>
        public Command Command { get; set; }
    }
}