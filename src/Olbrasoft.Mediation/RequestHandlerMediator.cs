using Olbrasoft.Mediation.Abstractions;
using System.Collections.Concurrent;

namespace Olbrasoft.Mediation;

public class RequestHandlerMediator(Func<Type, IBaseRequestHandler> getHandler) : IMediator
{
    private static readonly ConcurrentDictionary<Type, IBaseRequestHandler> _handlers = new();

    private readonly Func<Type, IBaseRequestHandler> _getHandler = getHandler ?? throw new ArgumentNullException(nameof(getHandler));

    public Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {
        if (request is not null)
        {
            return ((IRequestHandler<TResponse>)_handlers.GetOrAdd(request.GetType(),
                requestType =>
                {
                    var handlerType = (typeof(RequestHandler<,>).MakeGenericType(requestType, typeof(TResponse)));
                    return _getHandler(handlerType) ?? throw new InvalidOperationException($"Could not create handler for handlerType {handlerType}");
                })).HandleAsync(request, token);


            //return ((IRequestHandler<TResponse>)_getHandler(typeof(RequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse)))).HandleAsync(request, token);

        }

        throw new ArgumentNullException(nameof(request));
    }
}