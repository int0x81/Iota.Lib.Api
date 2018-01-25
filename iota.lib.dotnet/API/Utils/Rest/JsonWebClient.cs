using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using Iota.Lib.CSharp.Api.Core;
using Iota.Lib.CSharp.Api.Exception;
using Newtonsoft.Json;

namespace Iota.Lib.CSharp.Api.Utils.Rest
{
    internal class JsonWebClient
    {
        public TResponse GetResponse<TResponse>(Uri uri, string data)
        {
            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";
            request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.Headers.Add("Origin", "iota.lib.csharp");
            request.Headers.Add("Accept-Language", "de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4 ");
            request.KeepAlive = false;
            request.Timeout = 300000;
            request.ReadWriteTimeout = 300000;

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            WebResponse response = request.GetResponse();

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string responseString = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResponse>(responseString);
            }
        }

        public async Task<TResponse> GetResponseAsync<TResponse>(Uri uri, string data) where TResponse : IotaResponse, new()
        {
            HttpWebRequest request = WebRequest.CreateHttp(uri);

            request.Method = "POST";
            request.ContentType = "application-type/json;charset=utf-8";

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            WebResponse response = await request.GetResponseAsync();

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string responseString = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResponse>(responseString);
            }
        }
    }
}