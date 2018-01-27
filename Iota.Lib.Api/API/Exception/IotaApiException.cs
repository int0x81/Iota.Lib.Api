namespace Iota.Lib.Api.Exception
{
    /// <summary>
    /// Occures when the communication with a node fails
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IotaApiException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IotaApiException"/> class.
        /// </summary>
        /// <param name="error">The error.</param>
        public IotaApiException(string error) : base(error)
        {
        }
    }
}