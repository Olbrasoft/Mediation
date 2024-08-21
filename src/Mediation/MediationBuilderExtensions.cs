using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Olbrasoft.Mediation;


/// <summary>
/// Extensions for configuring mediation in the dependency injection container.
/// </summary>
public static class MediationBuilderExtensions
{


    public static IServiceCollection UseRequestHandlerWrapperMediator(this MediationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {

        builder.Services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(RequestHandlerWrapperMediator), lifetime));

        return builder.Services;
    }


    /// <summary>
    /// Configures the mediator to use request handlers.
    /// </summary>
    /// <param name="builder">The <see cref="MediationBuilder"/> instance.</param>
    /// <param name="lifetime">The lifetime of the registered services. Default is <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection UseRequestHandlerMediator(this MediationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        builder.Services.TryAdd(
         new ServiceDescriptor(
        typeof(Func<Type, IBaseRequestHandler>),
        p => new Func<Type, IBaseRequestHandler>(t => (IBaseRequestHandler)p.GetRequiredService(t)),
        lifetime));

        builder.Services.TryAdd(new ServiceDescriptor(typeof(RequestHandler<,>), typeof(RequestHandler<,>), lifetime));
        builder.Services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(RequestHandlerMediator), lifetime));
        return builder.Services;
    }

    /// <summary>
    /// Configures the mediator to use dynamic handlers.
    /// </summary>
    /// <param name="builder">The <see cref="MediationBuilder"/> instance.</param>
    /// <param name="lifetime">The lifetime of the registered services. Default is <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection UseDynamicMediator(this MediationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {

        builder.Services.TryAdd(new ServiceDescriptor(typeof(Func<Type, dynamic>),
            p => new Func<Type, dynamic>(t => p.GetRequiredService(t)), lifetime));


        builder.Services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(DynamicMediator), lifetime));
        return builder.Services;
    }
}
