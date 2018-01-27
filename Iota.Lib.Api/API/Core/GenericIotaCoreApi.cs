using System;
using System.Threading.Tasks;
using Iota.Lib.Api.Utils;

namespace Iota.Lib.Api.Core
{
    /// <summary>
    /// Represents a generic version of the core API that is used internally 
    /// </summary>
    /// <seealso cref="IGenericIotaCoreApi" />
    public class GenericIotaCoreApi : IGenericIotaCoreApi
    {
        string _host;
        int _port;
        JsonWebClient jsonWebClient = new JsonWebClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIotaCoreApi"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public GenericIotaCoreApi(string host, int port)
        {
            _host = host;
            _port = port;
        }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname
        {
            get { return _host; }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port
        {
            get { return _port; }
        }

        /// <summary>
        /// Requests the specified request.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public TResponse Request<TRequest, TResponse>(TRequest request) where TRequest: IotaRequest where TResponse : IotaResponse, new()
        {
            return jsonWebClient.GetResponse<TResponse>(new Uri(CreateBaseUrl()), new JsonSerializer().Serialize(request));
        }

        public Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request) where TRequest : IotaRequest where TResponse : IotaResponse, new()
        {
            return jsonWebClient.GetResponseAsync<TResponse>(new Uri(CreateBaseUrl()), new JsonSerializer().Serialize(request));
        }

        private string CreateBaseUrl()
        {
            return "https://" + _host + ":" + _port;
        }
    }
}