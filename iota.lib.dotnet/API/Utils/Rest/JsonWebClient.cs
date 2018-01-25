using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Iota.Lib.CSharp.Api.Core;
using Iota.Lib.CSharp.Api.Exception;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("Iota.Lib.CSharpTests")]

namespace Iota.Lib.CSharp.Api.Utils.Rest
{
    internal class JsonWebClient
    {
        public TResponse GetResponse<TResponse>(Uri uri, string data) where TResponse : IotaResponse, new()
        {
            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["X-IOTA-API-Version"] = "1";

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseString = reader.ReadToEnd();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            TResponse result = JsonConvert.DeserializeObject<TResponse>(responseString);
                            result.StatusCode = response.StatusCode;
                            return result;
                        }

                        throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(responseString).Error);
                    }
                }
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    String errorResponse = reader.ReadToEnd();
                    throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(errorResponse).Error);
                }
            }
        }

        public async Task<TResponse> GetResponseAsync<TResponse>(Uri uri, string data) where TResponse : IotaResponse, new()
        {

            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["X-IOTA-API-Version"] = "1";

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(bytes, 0, bytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseString = await reader.ReadToEndAsync();

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            TResponse result = JsonConvert.DeserializeObject<TResponse>(responseString);
                            result.StatusCode = response.StatusCode;
                            return result;
                        }

                        throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(responseString).Error);
                    }
                }
            }
            catch (WebException ex)
            {
                Stream stream = ex.Response.GetResponseStream();
                using (var reader = new StreamReader(stream))
                {
                    String errorResponse = await reader.ReadToEndAsync();
                    throw new IotaApiException(JsonConvert.DeserializeObject<ErrorResponse>(errorResponse).Error);
                }
            }
        }
    }
}