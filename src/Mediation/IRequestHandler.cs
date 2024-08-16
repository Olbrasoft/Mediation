namespace Olbrasoft.Mediation;
/// <summary>
/// Defines a handler for a request
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public interface IRequestHandler<in TRequest, TResponse> : IBaseRequestHandler where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles a request asynchronously.
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>The response from the request</returns>
    Task<TResponse> HandleAsync(TRequest request, CancellationToken token);
}


/// <summary>
/// Defines a handler for a request
/// </summary>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public interface IRequestHandler<TResponse> : IBaseRequestHandler
{
    /// <summary>
    /// Handles a request asynchronously.
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <param name="token">The cancellation token</param>
    /// <returns>The response from the request</returns>
    Task<TResponse> HandleAsync(IRequest<TResponse> request, CancellationToken token);
}
