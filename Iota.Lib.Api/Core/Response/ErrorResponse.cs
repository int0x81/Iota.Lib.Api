namespace Iota.Lib.Core
{
    /// <summary>
    /// Occurs when a an invalid request was made or the node does not support a specific command
    /// </summary>
    /// <seealso cref="IotaResponse"/>
    public class ErrorResponse : IotaResponse
    {
        public string Error { get; set; }
    }
}