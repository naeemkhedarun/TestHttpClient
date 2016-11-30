using System;
using System.Threading.Tasks;

namespace TestHttpClient
{
    class ExecuteOnce : IScenario
    {
        private readonly IHttpClient _client;

        public ExecuteOnce(IHttpClient client)
        {
            _client = client;
        }

        public async Task ExecuteAsync(Func<Task> changingParameters)
        {
            await _client.GetAsync();
        }
    }
}