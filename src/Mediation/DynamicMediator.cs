using System.Collections.Concurrent;

namespace Olbrasoft.Mediation;

public class DynamicMediator(Func<Type, dynamic> getHandler) : IMediator
{

    private static readonly ConcurrentDictionary<Type, Type> _handlerTypes = new();

    private readonly Func<Type, dynamic> _getHandler = getHandler ?? throw new ArgumentNullException(nameof(getHandler));

    public Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {
        if (request is not null)
        {
            var handlerType =
              _handlerTypes.GetOrAdd(request.GetType(), static requestType => (typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse))));

            return (_getHandler(handlerType)
                ?? throw new InvalidOperationException($"Could not create handler for handlerType {handlerType}")).HandleAsync((dynamic)request, token);
        }

        throw new ArgumentNullException(nameof(request));
    }
}