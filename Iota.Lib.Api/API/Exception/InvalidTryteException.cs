namespace Iota.Lib.Api.Exception
{
    /// <summary>
    /// Occurs when invalid trytes are encountered
    /// </summary>
    public class InvalidTryteException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTryteException"/> class.
        /// </summary>
        /// <param name="error">The error</param>
        public InvalidTryteException() : base("Provided string is no array of trytes")
        {
        }
    }
}