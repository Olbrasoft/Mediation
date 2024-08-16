using System.Collections.Concurrent;
using System.Reflection;

namespace Olbrasoft.Mediation;

public class ReflectionMediator(Func<Type, object> getHandler) : IMediator
{

    private static readonly Dictionary<Type, object> _handles = new();
    private static readonly ConcurrentDictionary<Type, Tuple<object, MethodInfo>> _handlersAndMethods = new();

    private readonly Func<Type, object> _getHandler = getHandler ?? throw new ArgumentNullException(nameof(getHandler));

    public Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {


        if (!_handlersAndMethods.TryGetValue(request.GetType(), out var handlerAndMethod))
        {
            var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

            var handler = _getHandler(handlerType);

            MethodInfo? method = handlerType.GetMethod("HandleAsync");

            if (method is not null)
            {
                handlerAndMethod = new Tuple<object, MethodInfo>(handler, method);
                _handlersAndMethods.TryAdd(request.GetType(), handlerAndMethod);
            }
        }

#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return (Task<TResponse>)handlerAndMethod.Item2.Invoke(handlerAndMethod.Item1, new object[] { request, token });
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore IDE0300 // Simplify collection initialization



        //var handlerType = typeof(IRequestHandler<,>)
        //.MakeGenericType(request.GetType(), typeof(TResponse));

        //if (!_handles.TryGetValue(request.GetType(), out var handler))
        //{      

        //    handler = _getHandler(handlerType);

        //    _handles.Add(request.GetType(), handler);

        //}

        //MethodInfo? method = handlerType.GetMethod("HandleAsync");

        //if (method is not null)
        //{

        //    var task = method.Invoke(handler, [request, token]);

        //    if (task is not null)
        //    {
        //        return (Task<TResponse>)task;
        //    }

        //}






        // var handlerType = typeof(IRequestHandler<,>)
        //.MakeGenericType(request.GetType(), typeof(TResponse));

        // object handler = _getHandler(handlerType);

        // MethodInfo? method = handlerType.GetMethod("HandleAsync");

        // if (method is not null)
        // {

        //     var task = method.Invoke(handler, [request, token]);

        //     if (task is not null)
        //     {
        //         return (Task<TResponse>)task;
        //     }

        // }




        throw new InvalidOperationException($"Could not find method HandleAsync");

    }
}