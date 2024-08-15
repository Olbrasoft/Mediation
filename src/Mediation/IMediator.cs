using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation;

public interface IMediator
{
    Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default);

}
