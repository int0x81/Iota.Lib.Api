using System;
using System.Collections.Generic;
using System.Text;

namespace Iota.Lib.Exception
{
    /// <summary>
    /// Occurs when an invalid transaction is specified
    /// </summary>
    public class InvalidTransactionException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTransactionException"/> class.
        /// </summary>
        public InvalidTransactionException()
        {
        }
    }
}
