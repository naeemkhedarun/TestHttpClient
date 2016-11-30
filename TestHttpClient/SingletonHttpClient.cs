using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestHttpClient
{
    public class SingletonHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly Uri _uri;

        public SingletonHttpClient(Uri uri)
        {
            _uri = uri;
            _client = new HttpClient();
        }

        public Task<HttpResponseMessage> GetAsync()
        {
            return _client.GetAsync(_uri);
        }
    }
}