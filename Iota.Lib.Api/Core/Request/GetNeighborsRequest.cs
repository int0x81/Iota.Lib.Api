﻿namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents the core API request 'GetNeighbors'
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class GetNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetNeighborsRequest"/> class.
        /// </summary>
        public GetNeighborsRequest() : base(Core.Command.GetNeighbors)
        {
        }
    }
}