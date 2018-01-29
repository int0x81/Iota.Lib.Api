namespace Iota.Lib.Exception
{
    /// <summary>
    /// Occurs when certain core API calls on the node are disabled
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IllegalAccessException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IllegalAccessException"/> class.
        /// </summary>
        /// <param name="error">The error</param>
        public IllegalAccessException(string error) : base(error)
        {
        }
    }
}