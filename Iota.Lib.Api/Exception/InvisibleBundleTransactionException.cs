namespace Iota.Lib.Exception
{
    /// <summary>
    /// Occurs when a bundle or transaction is not visible in the tangle
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class InvisibleBundleTransactionException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvisibleBundleTransactionException"/> class
        /// </summary>
        public InvisibleBundleTransactionException() : base()
        {
        }
    }
}