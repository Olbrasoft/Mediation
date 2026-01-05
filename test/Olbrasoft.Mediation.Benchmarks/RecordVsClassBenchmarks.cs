using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Benchmarks;

/// <summary>
/// Benchmarks comparing performance characteristics of record-based requests vs class-based requests.
/// Tests creation speed, equality comparison, and mediator processing across different implementations.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
public class RecordVsClassBenchmarks
{
    // Test data
    private const int UserId = 42;
    private const string UserName = "John Doe";
    private const string UserEmail = "john@example.com";

    // Service providers (for disposal)
    private ServiceProvider _provider1 = null!;
    private ServiceProvider _provider2 = null!;
    private ServiceProvider _provider3 = null!;

    // Mediators
    private IMediator _requestHandlerMediator = null!;
    private IMediator _dynamicMediator = null!;
    private IMediator _requestHandlerWrapperMediator = null!;

    // Pre-created instances for comparison benchmarks
    private GetUserRecordQuery _recordQuery1 = null!;
    private GetUserRecordQuery _recordQuery2 = null!;
    private GetUserClassQuery _classQuery1 = null!;
    private GetUserClassQuery _classQuery2 = null!;

    // Pre-created instances for mediator benchmarks (to avoid allocation overhead)
    private GetUserRecordQuery _recordQueryForMediator = null!;
    private GetUserClassQuery _classQueryForMediator = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Setup RequestHandlerMediator
        var services1 = new ServiceCollection();
        services1.AddMediation();
        services1.AddTransient<IRequestHandler<GetUserRecordQuery, UserDto>, GetUserRecordHandler>();
        services1.AddTransient<IRequestHandler<GetUserClassQuery, UserDto>, GetUserClassHandler>();
        _provider1 = services1.BuildServiceProvider();
        _requestHandlerMediator = _provider1.GetRequiredService<IMediator>();

        // Setup DynamicMediator
        var services2 = new ServiceCollection();
        services2.AddSingleton<IMediator, DynamicMediator>();
        services2.AddTransient<IRequestHandler<GetUserRecordQuery, UserDto>, GetUserRecordHandler>();
        services2.AddTransient<IRequestHandler<GetUserClassQuery, UserDto>, GetUserClassHandler>();
        _provider2 = services2.BuildServiceProvider();
        _dynamicMediator = _provider2.GetRequiredService<IMediator>();

        // Setup RequestHandlerWrapperMediator
        var services3 = new ServiceCollection();
        services3.AddSingleton<IMediator, RequestHandlerWrapperMediator>();
        services3.AddTransient<IRequestHandler<GetUserRecordQuery, UserDto>, GetUserRecordHandler>();
        services3.AddTransient<IRequestHandler<GetUserClassQuery, UserDto>, GetUserClassHandler>();
        _provider3 = services3.BuildServiceProvider();
        _requestHandlerWrapperMediator = _provider3.GetRequiredService<IMediator>();

        // Pre-create instances for equality benchmarks
        _recordQuery1 = new GetUserRecordQuery(UserId);
        _recordQuery2 = new GetUserRecordQuery(UserId);
        _classQuery1 = new GetUserClassQuery(UserId);
        _classQuery2 = new GetUserClassQuery(UserId);

        // Pre-create instances for mediator benchmarks
        _recordQueryForMediator = new GetUserRecordQuery(UserId);
        _classQueryForMediator = new GetUserClassQuery(UserId);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _provider1?.Dispose();
        _provider2?.Dispose();
        _provider3?.Dispose();
    }

    #region Request Creation Benchmarks

    [Benchmark(Description = "Create Record Request")]
    public GetUserRecordQuery CreateRecordRequest()
    {
        return new GetUserRecordQuery(UserId);
    }

    [Benchmark(Description = "Create Class Request")]
    public GetUserClassQuery CreateClassRequest()
    {
        return new GetUserClassQuery(UserId);
    }

    [Benchmark(Description = "Create Record Request with 3 params")]
    public CreateUserRecordCommand CreateRecordRequestMultipleParams()
    {
        return new CreateUserRecordCommand(UserId, UserName, UserEmail);
    }

    [Benchmark(Description = "Create Class Request with 3 params")]
    public CreateUserClassCommand CreateClassRequestMultipleParams()
    {
        return new CreateUserClassCommand
        {
            UserId = UserId,
            UserName = UserName,
            UserEmail = UserEmail
        };
    }

    #endregion

    #region Equality Comparison Benchmarks

    [Benchmark(Description = "Record Equality (Equal)")]
    public bool RecordEqualityEqual()
    {
        return _recordQuery1.Equals(_recordQuery2);
    }

    [Benchmark(Description = "Class Equality (Equal)")]
    public bool ClassEqualityEqual()
    {
        return _classQuery1.Equals(_classQuery2);
    }

    [Benchmark(Description = "Record == operator")]
    public bool RecordEqualityOperator()
    {
        return _recordQuery1 == _recordQuery2;
    }

    [Benchmark(Description = "Class == operator")]
    public bool ClassEqualityOperator()
    {
        return _classQuery1 == _classQuery2;
    }

    [Benchmark(Description = "Record GetHashCode")]
    public int RecordGetHashCode()
    {
        return _recordQuery1.GetHashCode();
    }

    [Benchmark(Description = "Class GetHashCode")]
    public int ClassGetHashCode()
    {
        return _classQuery1.GetHashCode();
    }

    #endregion

    #region Mediator Processing Benchmarks - RequestHandlerMediator

    [Benchmark(Description = "RequestHandlerMediator - Record")]
    public async Task<UserDto> RequestHandlerMediatorRecord()
    {
        return await _requestHandlerMediator.MediateAsync(_recordQueryForMediator);
    }

    [Benchmark(Description = "RequestHandlerMediator - Class")]
    public async Task<UserDto> RequestHandlerMediatorClass()
    {
        return await _requestHandlerMediator.MediateAsync(_classQueryForMediator);
    }

    #endregion

    #region Mediator Processing Benchmarks - DynamicMediator

    [Benchmark(Description = "DynamicMediator - Record")]
    public async Task<UserDto> DynamicMediatorRecord()
    {
        return await _dynamicMediator.MediateAsync(_recordQueryForMediator);
    }

    [Benchmark(Description = "DynamicMediator - Class")]
    public async Task<UserDto> DynamicMediatorClass()
    {
        return await _dynamicMediator.MediateAsync(_classQueryForMediator);
    }

    #endregion

    #region Mediator Processing Benchmarks - RequestHandlerWrapperMediator

    [Benchmark(Description = "RequestHandlerWrapperMediator - Record")]
    public async Task<UserDto> RequestHandlerWrapperMediatorRecord()
    {
        return await _requestHandlerWrapperMediator.MediateAsync(_recordQueryForMediator);
    }

    [Benchmark(Description = "RequestHandlerWrapperMediator - Class")]
    public async Task<UserDto> RequestHandlerWrapperMediatorClass()
    {
        return await _requestHandlerWrapperMediator.MediateAsync(_classQueryForMediator);
    }

    #endregion

    #region Record Immutability Benchmarks

    [Benchmark(Description = "Record 'with' expression")]
    public GetUserRecordQuery RecordWithExpression()
    {
        return _recordQuery1 with { UserId = 99 };
    }

    [Benchmark(Description = "Class copy with mutation")]
    public GetUserClassQuery ClassCopyWithMutation()
    {
        return new GetUserClassQuery(_classQuery1.UserId) { UserId = 99 };
    }

    #endregion
}

#region Test Request Types - Records

/// <summary>
/// Record-based query with primary constructor.
/// </summary>
public record GetUserRecordQuery(int UserId) : IRequest<UserDto>;

/// <summary>
/// Record-based command with multiple parameters.
/// </summary>
public record CreateUserRecordCommand(
    int UserId,
    string UserName,
    string UserEmail
) : IRequest<bool>;

#endregion

#region Test Request Types - Classes

/// <summary>
/// Class-based query for comparison.
/// </summary>
public class GetUserClassQuery : IRequest<UserDto>
{
    public int UserId { get; set; }

    public GetUserClassQuery(int userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// Class-based command with multiple properties.
/// </summary>
public class CreateUserClassCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
}

#endregion

#region Response DTO

/// <summary>
/// Shared response DTO.
/// </summary>
public record UserDto(int Id, string Name, string Email);

#endregion

#region Request Handlers

/// <summary>
/// Handler for record-based query.
/// </summary>
public class GetUserRecordHandler : IRequestHandler<GetUserRecordQuery, UserDto>
{
    public Task<UserDto> HandleAsync(GetUserRecordQuery request, CancellationToken cancellationToken)
    {
        // Simulate lightweight data access
        return Task.FromResult(new UserDto(request.UserId, "John Doe", "john@example.com"));
    }
}

/// <summary>
/// Handler for class-based query.
/// </summary>
public class GetUserClassHandler : IRequestHandler<GetUserClassQuery, UserDto>
{
    public Task<UserDto> HandleAsync(GetUserClassQuery request, CancellationToken cancellationToken)
    {
        // Simulate lightweight data access
        return Task.FromResult(new UserDto(request.UserId, "John Doe", "john@example.com"));
    }
}

#endregion
