using Olbrasoft.Mediation;
using Olbrasoft.Mediation.Benchmarks;

namespace Dispatching.Benchmarks;

public class AwesomeRequestHandler : IRequestHandler<AwesomeRequest, string>
{


    public Task<string> HandleAsync(AwesomeRequest request, CancellationToken token)
    {
        return Task.FromResult("Hello World");

    }
}