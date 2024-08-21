using System.Collections.Concurrent;

namespace Olbrasoft.Mediation;

public class RequestHandlerMediator(Func<Type, IBaseRequestHandler> getHandler) : IMediator
{
    private static readonly ConcurrentDictionary<Type, Type> _handlerTypes = new();

    private readonly Func<Type, IBaseRequestHandler> _getHandler = getHandler ?? throw new ArgumentNullException(nameof(getHandler));

    public Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {
        if (request is not null)
        {
            var handlerType =
                _handlerTypes.GetOrAdd(request.GetType(), static requestType => (typeof(RequestHandler<,>).MakeGenericType(requestType, typeof(TResponse))));

            return ((IRequestHandler<TResponse>)_getHandler(handlerType) ??
                throw new InvalidOperationException($"Could not create handler for handlerType {handlerType}")).HandleAsync(request, token);
        }
        else
        {
            throw new ArgumentNullException(nameof(request));
        }
    }
}