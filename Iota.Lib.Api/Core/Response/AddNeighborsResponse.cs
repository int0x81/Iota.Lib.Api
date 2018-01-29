namespace Iota.Lib.Core
{

    /// <summary>
    /// Response of <see cref="AddNeighborsRequest"/>
    /// </summary>
    /// <seealso cref="IotaResponse"/>
    public class AddNeighborsResponse : IotaResponse
    {
        /// <summary>
        /// Gets the number of added neighbors.
        /// </summary>
        /// <value>
        /// The number of added neighbors.
        /// </value>
        public long AddedNeighbors { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(AddedNeighbors)}: {AddedNeighbors}";
        }
    }
}