﻿namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// Represents the core API request 'GetTrytes'
    /// </summary>
    public class GetTrytesRequest : IotaRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTrytesRequest"/> class.
        /// </summary>
        public GetTrytesRequest() : base(Command.GetTrytes)
        {

        }

        /// <summary>
        /// Gets or sets the hashes.
        /// </summary>
        /// <value>
        /// The hashes.
        /// </value>
        public string[] Hashes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Hashes)}: {string.Join(",", Hashes)}";
        }
    }
}