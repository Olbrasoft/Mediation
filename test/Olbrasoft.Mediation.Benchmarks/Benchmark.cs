using BenchmarkDotNet.Attributes;
using Dispatching.Benchmarks;
using Microsoft.Extensions.DependencyInjection;

namespace Olbrasoft.Mediation.Benchmarks;
public class Benchmark
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private AwesomeRequest _request;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private RequestHandlerMediator _requestHandlerMediator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private DynamicMediator _dynamicMediator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ReflectionMediator _reflectionMediator;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();


        services.AddTransient<IRequestHandler<AwesomeRequest, string>, AwesomeRequestHandler>();

        // services.AddSingleton<Factory>(p => p.GetService!);


        services.AddSingleton<Func<Type, IBaseRequestHandler>>(p => (t) => (IBaseRequestHandler)p.GetRequiredService(t));

        services.AddSingleton<Func<Type, object>>(p => (t) => p.GetRequiredService(t));

        services.AddTransient(typeof(RequestHandler<,>), typeof(RequestHandler<,>));

        services.AddTransient<RequestHandlerMediator, RequestHandlerMediator>();

        services.AddTransient<DynamicMediator, DynamicMediator>();

        services.AddTransient<ReflectionMediator, ReflectionMediator>();

        var provider = services.BuildServiceProvider();

        _requestHandlerMediator = provider.GetRequiredService<RequestHandlerMediator>();
        _dynamicMediator = provider.GetRequiredService<DynamicMediator>();
        _reflectionMediator = provider.GetRequiredService<ReflectionMediator>();


        _request = new AwesomeRequest();
    }




    [Benchmark]
    public async Task DynamicMediatorSend()
    {
        var result = await _dynamicMediator.MediateAsync(_request);

    }

    [Benchmark]
    public async Task ReflectionMediatorSend()
    {
        var result = await _reflectionMediator.MediateAsync(_request);

    }

    [Benchmark]
    public async Task RequestHandlerMediatorSend()
    {
        var result = await _requestHandlerMediator.MediateAsync(_request);

    }


}
