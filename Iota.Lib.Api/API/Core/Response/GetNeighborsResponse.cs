using System.Collections.Generic;
using Iota.Lib.Api.Model;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Response of <see cref="GetNeighborsRequest"/>
    /// </summary>
    /// <seealso cref="IotaResponse"/>
    public class GetNeighborsResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the neighbors.
        /// </summary>
        /// <value>
        /// The neighbors.
        /// </value>
        public List<Neighbor> Neighbors { get; set; }
    }
}