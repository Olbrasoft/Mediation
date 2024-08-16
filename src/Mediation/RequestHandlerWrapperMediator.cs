using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Olbrasoft.Mediation;
public class RequestHandlerWrapperMediator(IServiceProvider serviceProvider) : IMediator
{

    private static readonly ConcurrentDictionary<Type, IBaseRequestHandler> _requestHandlers = new();

    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {
        if (request != null)
        {

            return ((RequestHandlerWrapper<TResponse>)_requestHandlers.GetOrAdd(request.GetType(), static requestType =>
             {
                 var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
                 var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
                 return (IBaseRequestHandler)wrapper;
             })).HandleAsync(request, _serviceProvider, token);


            //var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            //var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {request.GetType()}");
            //return ((RequestHandlerWrapper<TResponse>)wrapper).HandleAsync(request, _serviceProvider, token);

        }

        throw new ArgumentNullException(nameof(request));
    }
}



public abstract class RequestHandlerWrapper<TResponse> : IBaseRequestHandler
{
    public abstract Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}


public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{

    public override Task<TResponse> HandleAsync(IRequest<TResponse> request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {

        return serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .HandleAsync((TRequest)request, cancellationToken);

    }


}