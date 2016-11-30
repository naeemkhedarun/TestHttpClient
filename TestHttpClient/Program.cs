using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using PSHostsFile;
using Xunit;
using Xunit.Abstractions;

namespace TestHttpClient
{
    public class DnsTests : IDisposable
    {
        private readonly ITestOutputHelper _logger;
        private string _hostname = "testdns";
        private readonly int _dnsRefreshTimeout;
        private readonly int _connectionLeaseTimeout;
        private readonly Uri _uri;

        public DnsTests(ITestOutputHelper logger)
        {
            _logger = logger;
            _dnsRefreshTimeout = ServicePointManager.DnsRefreshTimeout;
            _connectionLeaseTimeout = ServicePointManager.FindServicePoint(new Uri("http://testdns:9200")).ConnectionLeaseTimeout;
            _uri = new Uri($"http://{_hostname}:9200");
        }

        public void Dispose()
        {
            HostsFile.Remove(_hostname);
            ServicePointManager.DnsRefreshTimeout = _dnsRefreshTimeout;
            ServicePointManager.FindServicePoint(new Uri("http://testdns:9200")).ConnectionLeaseTimeout = _connectionLeaseTimeout;
        }

        [Fact]
        public async Task ConnectTimeoutTest()
        {
            HostsFile.Set(_hostname, "128.0.0.1");
            var client = new TransientHttpClient(_uri);

            var timer = Stopwatch.StartNew();
            try
            {
                await new ExecuteOnce(client).ExecuteAsync(() => Task.CompletedTask);
            }
            catch
            {
                _logger.WriteLine(timer.ElapsedMilliseconds.ToString());
            }
        }

        [Fact]
        public async Task TransientDnsChangeTest()
        {
            HostsFile.Set(_hostname, "127.0.0.1");
            var client = new TransientHttpClient(_uri);

            var timer = Stopwatch.StartNew();
            try
            {
                await new LoopUntilFailure(client).ExecuteAsync(
                    () => Task.Run(() => HostsFile.Set(_hostname, "128.0.0.1")));
            }
            catch
            {
                _logger.WriteLine($"{typeof(IHttpClient).Name} - {timer.ElapsedMilliseconds}");
            }
        }

        [Fact]
        public async Task SingletonDnsChangeTest()
        {
            HostsFile.Set(_hostname, "127.0.0.1");
            var client = new SingletonHttpClient(_uri);

            var timer = Stopwatch.StartNew();
            try
            {
                await new LoopUntilFailure(client).ExecuteAsync(
                    () => Task.Run(() => HostsFile.Set(_hostname, "128.0.0.1")));
            }
            catch
            {
                _logger.WriteLine($"{typeof(IHttpClient).Name} - {timer.ElapsedMilliseconds}");
            }
        }


        [Fact]
        public async Task TransientDnsChangeWithDnsRefreshTimeoutTest()
        {
            HostsFile.Set(_hostname, "127.0.0.1");
            ServicePointManager.DnsRefreshTimeout = 10000;

            var client = new TransientHttpClient(_uri);
            
            var timer = Stopwatch.StartNew();
            try
            {
                await new LoopUntilFailure(client).ExecuteAsync(
                    () => Task.Run(() => HostsFile.Set(_hostname, "128.0.0.1")));
            }
            catch
            {
                _logger.WriteLine($"{typeof(IHttpClient).Name} - {timer.ElapsedMilliseconds}");
            }
        }

        [Fact]
        public async Task SingletonDnsChangeWithConnectionLeaseTimeoutTest()
        {
            HostsFile.Set(_hostname, "127.0.0.1");
            ServicePointManager.FindServicePoint(_uri).ConnectionLeaseTimeout = 10000;

            var client = new SingletonHttpClient(_uri);

            var timer = Stopwatch.StartNew();
            try
            {
                await new LoopUntilFailure(client).ExecuteAsync(
                    () => Task.Run(() => HostsFile.Set(_hostname, "128.0.0.1")));
            }
            catch
            {
                _logger.WriteLine($"{typeof(IHttpClient).Name} - {timer.ElapsedMilliseconds}");
            }
        }

        [Fact]
        public async Task SingletonDnsChangeWithConnectionLeaseTimeoutAndDnsRefreshTimeoutTest()
        {
            HostsFile.Set(_hostname, "127.0.0.1");
            ServicePointManager.DnsRefreshTimeout = 10000;
            ServicePointManager.FindServicePoint(_uri).ConnectionLeaseTimeout = 10000;

            var client = new SingletonHttpClient(_uri);

            var timer = Stopwatch.StartNew();
            try
            {
                await new LoopUntilFailure(client).ExecuteAsync(
                    () => Task.Run(() => HostsFile.Set(_hostname, "128.0.0.1")));
            }
            catch
            {
                _logger.WriteLine($"{typeof(IHttpClient).Name} - {timer.ElapsedMilliseconds}");
            }
        }
    }

//    public class IPConnectionWithReducedConnectionLeaseTest : ITest
//    {
//        public void Execute(IHttpClient client)
//        {
//            ServicePointManager.FindServicePoint(new Uri("http://127.0.0.1:9200")).ConnectionLeaseTimeout = 1000;
//            ServicePointManager.FindServicePoint(new Uri("http://127.0.0.1:9200")).MaxIdleTime = 1000;
//            ServicePointManager.DnsRefreshTimeout = 1000;
//             
//            client.GetAsync().ContinueWith(task => Task.Delay(5000)).Wait();
//        }
//    }

}