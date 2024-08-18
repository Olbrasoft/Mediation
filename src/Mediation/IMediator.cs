namespace Olbrasoft.Mediation;

/// <summary>
/// Represents a mediator that handles the execution of requests and returns a response.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Executes the specified request and returns a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to be executed.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The response of the executed request.</returns>
    Task<TResponse> MediateAsync<TResponse>(IRequest<TResponse> request, CancellationToken token = default);
}
