using System;
using System.Threading.Tasks;

namespace TestHttpClient
{
    public interface IScenario
    {
        Task ExecuteAsync(Func<Task> changingParameters);
    }
}