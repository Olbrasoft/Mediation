using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Olbrasoft.Mediation;

namespace Olbrasoft.Extensions.DependencyInjection;
public static class MediationBuilderExtensions
{

    public static IServiceCollection UseRequestHandlerMediator(this MediationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {

        builder.Services.TryAddSingleton<Func<Type, IBaseRequestHandler>>(p => (t) => (IBaseRequestHandler)p.GetRequiredService(t));

        builder.Services.TryAdd(new ServiceDescriptor(typeof(RequestHandler<,>), typeof(RequestHandler<,>), lifetime));

        builder.Services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(RequestHandlerMediator), lifetime));

        return builder.Services;

    }

    public static IServiceCollection UseDynamicMediator(this MediationBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {

        builder.Services.TryAddSingleton<Func<Type, object>>(p => (t) => p.GetRequiredService(t));

        builder.Services.TryAdd(new ServiceDescriptor(typeof(IMediator), typeof(DynamicMediator), lifetime));


        return builder.Services;
    }

}
