﻿using System.Collections.Generic;

namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents the core api request 'RemoveNeighbors'
    /// </summary>
    /// <seealso cref="IotaRequest" />
    public class RemoveNeighborsRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveNeighborsRequest"/> class.
        /// </summary>
        /// <param name="uris">The uris.</param>
        public RemoveNeighborsRequest(List<string> uris) : base(Core.Command.RemoveNeighbors)
        {
            Uris = uris;
        }

        /// <summary>
        /// Gets or sets the uris of the neighbours to remove
        /// </summary>
        /// <value>
        /// The uris of the neighbours to remove.
        /// </value>
        public List<string> Uris { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Uris)}: {string.Join(",", Uris)}";
        }
    }
}