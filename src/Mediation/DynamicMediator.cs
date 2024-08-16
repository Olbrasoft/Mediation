using System.Collections.Concurrent;

namespace Olbrasoft.Mediation;

public class DynamicMediator(Func<Type, object> getHandler) : IMediator
{

    private static readonly ConcurrentDictionary<Type, dynamic> _handlers = new();

    private readonly Func<Type, object> _getHandler = getHandler ?? throw new ArgumentNullException(nameof(getHandler));

    public Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {
        if (request is not null)
        {

            return (_handlers.GetOrAdd(request.GetType(),
                requestType =>
                {
                    var handlerType = (typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse)));
                    return _getHandler(handlerType) ?? throw new InvalidOperationException($"Could not create handler for handlerType {handlerType}");
                })).HandleAsync((dynamic)request, token);



            //var handlerType = typeof(IRequestHandler<,>)
            //    .MakeGenericType(request.GetType(), typeof(TResponse));

            //dynamic handler = _getHandler(handlerType);

            //return handler.HandleAsync((dynamic)request, token);
        }

        throw new ArgumentNullException(nameof(request));
    }
}