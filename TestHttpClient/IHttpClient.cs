using System.Net.Http;
using System.Threading.Tasks;

namespace TestHttpClient
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync();
    }
}