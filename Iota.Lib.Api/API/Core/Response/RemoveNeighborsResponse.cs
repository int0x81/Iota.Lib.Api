namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// This class represents the response of <see cref="RemoveNeighborsRequest"/>
    /// </summary>
    /// /// <seealso cref="IotaResponse"/>
    public class RemoveNeighborsResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the number of removed neighbors.
        /// </summary>
        /// <value>
        /// The removed neighbors.
        /// </value>
        public long RemovedNeighbors { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(RemovedNeighbors)}: {RemovedNeighbors}";
        }
    }
}