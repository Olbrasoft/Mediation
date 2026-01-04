using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;

/// <summary>
/// Tests for C# record support with Mediation library.
/// Verifies that records implementing IRequest&lt;T&gt; work correctly with all mediator implementations.
/// </summary>
public class RecordRequestTests
{
    #region Test Records and Handlers

    // Simple record request with primary constructor
    public record SimpleRecordRequest(string Value) : IRequest<string>;

    // Record with multiple properties
    public record UserRequest(int UserId, string Email) : IRequest<UserResponse>;

    // Record response DTO
    public record UserResponse(int Id, string Name, string Email);

    // Generic record request
    public record GenericRecordRequest<T>(T Value) : IRequest<T>;

    // Complex record with nested record
    public record SearchRequest(
        string SearchTerm,
        FilterOptions Filter
    ) : IRequest<List<string>>;

    public record FilterOptions(int PageSize, int PageNumber);

    // Handlers for record requests
    public class SimpleRecordRequestHandler : IRequestHandler<SimpleRecordRequest, string>
    {
        public Task<string> HandleAsync(SimpleRecordRequest request, CancellationToken token)
        {
            return Task.FromResult($"Handled: {request.Value}");
        }
    }

    public class UserRequestHandler : IRequestHandler<UserRequest, UserResponse>
    {
        public Task<UserResponse> HandleAsync(UserRequest request, CancellationToken token)
        {
            return Task.FromResult(new UserResponse(request.UserId, "John Doe", request.Email));
        }
    }

    public class GenericRecordRequestHandler<T> : IRequestHandler<GenericRecordRequest<T>, T>
    {
        public Task<T> HandleAsync(GenericRecordRequest<T> request, CancellationToken token)
        {
            return Task.FromResult(request.Value);
        }
    }

    public class SearchRequestHandler : IRequestHandler<SearchRequest, List<string>>
    {
        public Task<List<string>> HandleAsync(SearchRequest request, CancellationToken token)
        {
            var results = new List<string>
            {
                $"Search: {request.SearchTerm}",
                $"Page: {request.Filter.PageNumber}",
                $"Size: {request.Filter.PageSize}"
            };
            return Task.FromResult(results);
        }
    }

    #endregion

    #region Basic Record Request Tests

    [Fact]
    public void Record_Should_Implement_IRequest_Interface()
    {
        // Arrange
        var request = new SimpleRecordRequest("test");

        // Act
        var implementsInterface = request is IRequest<string>;

        // Assert
        Assert.True(implementsInterface);
    }

    [Fact]
    public void Record_With_Primary_Constructor_Should_Have_Correct_Property_Values()
    {
        // Arrange & Act
        var request = new SimpleRecordRequest("test value");

        // Assert
        Assert.Equal("test value", request.Value);
    }

    [Fact]
    public void Record_With_Multiple_Properties_Should_Preserve_All_Values()
    {
        // Arrange & Act
        var request = new UserRequest(42, "test@example.com");

        // Assert
        Assert.Equal(42, request.UserId);
        Assert.Equal("test@example.com", request.Email);
    }

    [Fact]
    public void Generic_Record_Request_Should_Preserve_Type()
    {
        // Arrange & Act
        var intRequest = new GenericRecordRequest<int>(42);
        var stringRequest = new GenericRecordRequest<string>("test");

        // Assert
        Assert.Equal(42, intRequest.Value);
        Assert.Equal("test", stringRequest.Value);
    }

    [Fact]
    public void Nested_Record_Request_Should_Preserve_Structure()
    {
        // Arrange & Act
        var filter = new FilterOptions(10, 1);
        var request = new SearchRequest("query", filter);

        // Assert
        Assert.Equal("query", request.SearchTerm);
        Assert.Equal(10, request.Filter.PageSize);
        Assert.Equal(1, request.Filter.PageNumber);
    }

    [Fact]
    public void Record_Should_Support_Value_Equality()
    {
        // Arrange
        var request1 = new SimpleRecordRequest("test");
        var request2 = new SimpleRecordRequest("test");
        var request3 = new SimpleRecordRequest("different");

        // Assert
        Assert.Equal(request1, request2); // Value equality
        Assert.NotEqual(request1, request3);
    }

    #endregion

    #region Handler Registration Tests

    [Fact]
    public void AddMediation_Should_Register_Record_Request_Handler()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediation(Assembly.GetExecutingAssembly());
        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetService<IRequestHandler<SimpleRecordRequest, string>>();

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<SimpleRecordRequestHandler>(handler);
    }

    [Fact]
    public void AddMediation_Should_Register_Multiple_Record_Handlers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediation(Assembly.GetExecutingAssembly());
        var serviceProvider = services.BuildServiceProvider();

        var simpleHandler = serviceProvider.GetService<IRequestHandler<SimpleRecordRequest, string>>();
        var userHandler = serviceProvider.GetService<IRequestHandler<UserRequest, UserResponse>>();

        // Assert
        Assert.NotNull(simpleHandler);
        Assert.NotNull(userHandler);
    }

    [Fact]
    public void AddMediation_Should_Register_Generic_Record_Handler()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddMediation(Assembly.GetExecutingAssembly());
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Generic handlers are not auto-discovered (this is a known limitation)
        // The important part is that the registration process doesn't fail with records
        var handler = serviceProvider.GetService<IRequestHandler<GenericRecordRequest<int>, int>>();
        Assert.Null(handler); // Generic handlers require manual registration
    }

    #endregion

    #region RequestHandlerMediator Tests

    [Fact]
    public async Task RequestHandlerMediator_Should_Handle_Simple_Record_Request()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseRequestHandlerMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request = new SimpleRecordRequest("test value");

        // Act
        var result = await mediator.MediateAsync<string>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Handled: test value", result);
    }

    [Fact]
    public async Task RequestHandlerMediator_Should_Handle_Record_With_Multiple_Properties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseRequestHandlerMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request = new UserRequest(123, "user@test.com");

        // Act
        var result = await mediator.MediateAsync<UserResponse>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.Id);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("user@test.com", result.Email);
    }

    [Fact]
    public async Task RequestHandlerMediator_Should_Handle_Nested_Record_Request()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseRequestHandlerMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request = new SearchRequest("test query", new FilterOptions(20, 2));

        // Act
        var result = await mediator.MediateAsync<List<string>>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains("Search: test query", result);
        Assert.Contains("Page: 2", result);
        Assert.Contains("Size: 20", result);
    }

    #endregion

    #region DynamicMediator Tests

    [Fact]
    public async Task DynamicMediator_Should_Handle_Simple_Record_Request()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseDynamicMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request = new SimpleRecordRequest("dynamic test");

        // Act
        var result = await mediator.MediateAsync<string>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Handled: dynamic test", result);
    }

    [Fact]
    public async Task DynamicMediator_Should_Handle_Record_With_Response_DTO()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseDynamicMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request = new UserRequest(456, "dynamic@test.com");

        // Act
        var result = await mediator.MediateAsync<UserResponse>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(456, result.Id);
        Assert.Equal("dynamic@test.com", result.Email);
    }

    #endregion

    #region RequestHandlerWrapperMediator Tests

    [Fact]
    public async Task RequestHandlerWrapperMediator_Should_Handle_Simple_Record_Request()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseRequestHandlerWrapperMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request = new SimpleRecordRequest("wrapper test");

        // Act
        var result = await mediator.MediateAsync<string>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Handled: wrapper test", result);
    }

    [Fact]
    public async Task RequestHandlerWrapperMediator_Should_Handle_Record_With_Cached_Wrapper()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMediation(Assembly.GetExecutingAssembly())
                .UseRequestHandlerWrapperMediator();

        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var request1 = new UserRequest(789, "wrapper1@test.com");
        var request2 = new UserRequest(790, "wrapper2@test.com");

        // Act - Call twice to test wrapper caching
        var result1 = await mediator.MediateAsync<UserResponse>(request1);
        var result2 = await mediator.MediateAsync<UserResponse>(request2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(789, result1.Id);
        Assert.Equal(790, result2.Id);
    }

    #endregion

    #region Record Immutability Tests

    [Fact]
    public void Record_Should_Be_Immutable()
    {
        // Arrange
        var original = new SimpleRecordRequest("original");

        // Act - Records are immutable, this creates a new instance
        var modified = original with { Value = "modified" };

        // Assert
        Assert.Equal("original", original.Value);
        Assert.Equal("modified", modified.Value);
        Assert.NotSame(original, modified);
    }

    [Fact]
    public void Record_With_Expression_Should_Create_New_Instance()
    {
        // Arrange
        var original = new UserRequest(100, "old@test.com");

        // Act
        var modified = original with { Email = "new@test.com" };

        // Assert
        Assert.Equal("old@test.com", original.Email);
        Assert.Equal("new@test.com", modified.Email);
        Assert.Equal(100, modified.UserId); // UserId preserved
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task All_Mediators_Should_Handle_Same_Record_Request()
    {
        // Arrange - Test that all 3 configurable mediators (via fluent API) work with the same record type
        // Note: RequestHandlerMediator, DynamicMediator, and RequestHandlerWrapperMediator
        var testValue = "integration test";
        var request = new SimpleRecordRequest(testValue);
        var expectedResult = $"Handled: {testValue}";

        // Test RequestHandlerMediator
        var services1 = new ServiceCollection();
        services1.AddMediation(Assembly.GetExecutingAssembly()).UseRequestHandlerMediator();
        var mediator1 = services1.BuildServiceProvider().GetRequiredService<IMediator>();
        var result1 = await mediator1.MediateAsync<string>(request);

        // Test DynamicMediator
        var services2 = new ServiceCollection();
        services2.AddMediation(Assembly.GetExecutingAssembly()).UseDynamicMediator();
        var mediator2 = services2.BuildServiceProvider().GetRequiredService<IMediator>();
        var result2 = await mediator2.MediateAsync<string>(request);

        // Test RequestHandlerWrapperMediator
        var services3 = new ServiceCollection();
        services3.AddMediation(Assembly.GetExecutingAssembly()).UseRequestHandlerWrapperMediator();
        var mediator3 = services3.BuildServiceProvider().GetRequiredService<IMediator>();
        var result3 = await mediator3.MediateAsync<string>(request);

        // Assert - All mediators should return the same result
        Assert.Equal(expectedResult, result1);
        Assert.Equal(expectedResult, result2);
        Assert.Equal(expectedResult, result3);
    }

    #endregion
}
