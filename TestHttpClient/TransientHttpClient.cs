using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestHttpClient
{
    class TransientHttpClient : IHttpClient
    {
        private Uri _uri;

        public TransientHttpClient(Uri uri)
        {
            _uri = uri;
        }

        public async Task<HttpResponseMessage> GetAsync()
        {
            using (var client = new HttpClient())
            {
                return await client.GetAsync(_uri, new CancellationTokenSource(180000).Token);
            }
        }
    }
}