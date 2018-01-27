namespace Iota.Lib.Api.Exception
{
    /// <summary>
    /// Occurs when an invalid signature is encountered
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidSignatureException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSignatureException"/> class.
        /// </summary>
        public InvalidSignatureException() : base("Invalid signature")
        {
        }
    }
}