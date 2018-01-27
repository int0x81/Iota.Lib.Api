using System.Threading.Tasks;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Abstracts a generic version of the core api that is used internally.
    /// </summary>
    public interface IGenericIotaCoreApi
    {
        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        string Hostname { get; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        int Port { get; }

        /// <summary>
        /// Requests the specified request.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns>A <see cref="IotaResponse"/></returns>
        TResponse Request<TRequest, TResponse>(TRequest request) where TRequest : IotaRequest where TResponse : IotaResponse, new();

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns>A task that works on a <see cref="IotaResponse"/></returns>
        Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : IotaRequest where TResponse : IotaResponse, new();
    }
}