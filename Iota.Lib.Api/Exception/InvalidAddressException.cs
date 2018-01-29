using System;

namespace Iota.Lib.Exception
{
    /// <summary>
    /// Occurs when an invalid address is provided
    /// </summary>
    /// <seealso cref="System.ArgumentException" />
    public class InvalidAddressException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAddressException"/> class.
        /// </summary>
        /// <param name="error">The error</param>
        public InvalidAddressException(string error) : base(error)
        {
        }
    }
}