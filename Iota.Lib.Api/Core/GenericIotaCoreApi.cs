using System;
using System.Threading.Tasks;
using Iota.Lib.Utils;

namespace Iota.Lib.Core
{
    /// <summary>
    /// Represents a generic version of the core API that is used internally 
    /// </summary>
    /// <seealso cref="IGenericIotaCoreApi" />
    public class GenericIotaCoreApi : IGenericIotaCoreApi
    {
        string _host;
        int _port;
        bool _ssl;
        JsonWebClient jsonWebClient = new JsonWebClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericIotaCoreApi"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="ssl">States if the connection is using ssl-encryption(https)</param>
        public GenericIotaCoreApi(string host, int port, bool ssl)
        {
            _host = host;
            _port = port;
            _ssl = ssl;
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
        /// Gets the state that says if the connection is using ssl-encryption(https)
        /// </summary>
        /// <value>
        /// The state
        /// </value>
        public bool SSL
        {
            get { return _ssl; }
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
            return jsonWebClient.GetResponse<TResponse>(new Uri(CreateBaseUrl()), new JsonSerializer().Serialize(request));
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
            return jsonWebClient.GetResponseAsync<TResponse>(new Uri(CreateBaseUrl()), new JsonSerializer().Serialize(request));
        }

        private string CreateBaseUrl()
        {
            if(SSL)
            {
                return "https://" + _host + ":" + _port;
            }
            else
            {
                return "http://" + _host + ":" + _port;
            }
        }
    }
}