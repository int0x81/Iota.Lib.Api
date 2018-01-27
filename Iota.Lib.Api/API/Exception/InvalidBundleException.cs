namespace Iota.Lib.Api.Exception
{
    /// <summary>
    /// Occurs if an invalid bundle was found or provided
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidBundleException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBundleException"/> class.
        /// </summary>
        /// <param name="message">The error</param>
        public InvalidBundleException(string error) : base(error)
        {
        }
    }
}