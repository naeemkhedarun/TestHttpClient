using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestHttpClient
{
    class LoopUntilFailure : IScenario
    {
        private readonly IHttpClient _client;

        public LoopUntilFailure(IHttpClient client)
        {
            _client = client;
        }

        public async Task ExecuteAsync(Func<Task> changingParameters)
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(3)).Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                await _client.GetAsync();
                await Task.Delay(10000, cancellationToken);
                await changingParameters();
            }
        }
    }
}