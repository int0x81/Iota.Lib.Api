using System;
using System.Threading.Tasks;
using Iota.Lib.Utils;

namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents a generic version of the core API that is used internally 
    /// </summary>
    internal class GenericIotaCoreApi
    {
        JsonWebClient jsonWebClient = new JsonWebClient();
        string baseURL;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIotaCoreApi"/> class
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="is_ssl">States if the connection you want to establish is using ssl encryption(https)</param>
        public GenericIotaCoreApi(string host, int port, bool is_ssl)
        {
            if (is_ssl)
            {
                 baseURL = "https://" + host + ":" + port;
            }
            else
            {
                 baseURL = "http://" + host + ":" + port;
            }
        }
        
        /// <summary>
        /// Requests the specified request
        /// </summary>
        /// <typeparam name="TRequest">The type of the request</typeparam>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <param name="request">The request</param>
        /// <returns>A corresponding response</returns>
        public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest: IotaRequest where TResponse : IotaResponse, new()
        {
            return jsonWebClient.GetResponse<TResponse>(new Uri(baseURL), new JsonSerializer().Serialize(request));
        }

        /// <summary>
        /// Requests the specified request asynchronously
        /// </summary>
        /// <typeparam name="TRequest">The type of the request</typeparam>
        /// <typeparam name="TResponse">The type of the response</typeparam>
        /// <param name="request">The request</param>
        /// <returns>A corresponding response</returns>
        public Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : IotaRequest where TResponse : IotaResponse, new()
        {
            return jsonWebClient.GetResponseAsync<TResponse>(new Uri(baseURL), new JsonSerializer().Serialize(request));
        }
    }
}