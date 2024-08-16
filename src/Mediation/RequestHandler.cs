namespace Olbrasoft.Mediation;

public class RequestHandler<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler) : IRequestHandler<TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _handler = handler ?? throw new ArgumentNullException(nameof(handler));

    public Task<TResponse> HandleAsync(IRequest<TResponse> request, CancellationToken token = default)
    {
        return request is null ? throw new ArgumentNullException(nameof(request)) : _handler.HandleAsync((TRequest)request, token);
    }
}