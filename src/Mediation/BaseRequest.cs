namespace Olbrasoft.Mediation;


/// <summary>
/// Represents a base class for requests in the mediation pattern.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public abstract class BaseRequest<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// Gets or sets the mediator instance.
    /// </summary>
    public IMediator? Mediator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRequest{TResponse}"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    protected BaseRequest(IMediator mediator) => Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRequest{TResponse}"/> class.
    /// </summary>
    protected BaseRequest()
    {
    }
}

