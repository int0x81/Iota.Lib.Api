namespace Iota.Lib.Exception
{
    /// <summary>
    /// Occurs when an invalid tail transaction was encountered
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvalidTailTransactionException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTailTransactionException"/> class.
        /// </summary>
        public InvalidTailTransactionException() : base()
        {
        }
    }
}