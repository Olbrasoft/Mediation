using Microsoft.Extensions.DependencyInjection;
using Moq;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;

public class RequestHandlerWrapperMediatorTests
{
    // Test request and response types
    public class TestRequest : IRequest<string>
    {
        public string Value { get; set; } = "";
    }

    public class TestResponse
    {
        public string Result { get; set; } = "";
    }

    public class TestRequestWithResponse : IRequest<TestResponse>
    {
        public string Input { get; set; } = "";
    }

    // Test request handler implementations
    public class TestRequestHandler : IRequestHandler<TestRequest, string>
    {
        public Task<string> HandleAsync(TestRequest request, CancellationToken token)
        {
            return Task.FromResult($"Handled: {request.Value}");
        }
    }

    public class TestRequestWithResponseHandler : IRequestHandler<TestRequestWithResponse, TestResponse>
    {
        public Task<TestResponse> HandleAsync(TestRequestWithResponse request, CancellationToken token)
        {
            return Task.FromResult(new TestResponse { Result = $"Processed: {request.Input}" });
        }
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_Should_Accept_ServiceProvider()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>();

        // Act
        var mediator = new RequestHandlerWrapperMediator(serviceProvider.Object);

        // Assert
        Assert.NotNull(mediator);
    }

    #endregion

    #region MediateAsync Tests

    [Fact]
    public async Task MediateAsync_Should_Throw_ArgumentNullException_When_Request_Is_Null()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>();
        var mediator = new RequestHandlerWrapperMediator(serviceProvider.Object);
        TestRequest nullRequest = null!;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => mediator.MediateAsync<string>(nullRequest));
        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public async Task MediateAsync_Should_Handle_Request_Successfully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, string>, TestRequestHandler>();
        var serviceProvider = services.BuildServiceProvider();
        
        var mediator = new RequestHandlerWrapperMediator(serviceProvider);
        var request = new TestRequest { Value = "test" };

        // Act
        var result = await mediator.MediateAsync<string>(request);

        // Assert
        Assert.Equal("Handled: test", result);
    }

    [Fact]
    public async Task MediateAsync_Should_Handle_Complex_Response_Type()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequestWithResponse, TestResponse>, TestRequestWithResponseHandler>();
        var serviceProvider = services.BuildServiceProvider();
        
        var mediator = new RequestHandlerWrapperMediator(serviceProvider);
        var request = new TestRequestWithResponse { Input = "complex test" };

        // Act
        var result = await mediator.MediateAsync<TestResponse>(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Processed: complex test", result.Result);
    }

    [Fact]
    public async Task MediateAsync_Should_Pass_CancellationToken_To_Handler()
    {
        // Arrange
        var mockHandler = new Mock<IRequestHandler<TestRequest, string>>();
        mockHandler.Setup(h => h.HandleAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new OperationCanceledException());

        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, string>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();
        
        var mediator = new RequestHandlerWrapperMediator(serviceProvider);
        var request = new TestRequest { Value = "test" };
        var cancellationToken = new CancellationToken(true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => mediator.MediateAsync<string>(request, cancellationToken));
        
        mockHandler.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task MediateAsync_Should_Throw_InvalidOperationException_When_Handler_Not_Registered()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        
        var mediator = new RequestHandlerWrapperMediator(serviceProvider);
        var request = new TestRequest { Value = "test" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => mediator.MediateAsync<string>(request));
    }

    [Fact]
    public async Task MediateAsync_Should_Cache_RequestHandlers()
    {
        // Arrange
        var handlerCallCount = 0;
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, string>>(_ => 
        {
            handlerCallCount++;
            return new TestRequestHandler();
        });
        var serviceProvider = services.BuildServiceProvider();
        
        var mediator = new RequestHandlerWrapperMediator(serviceProvider);
        var request1 = new TestRequest { Value = "test1" };
        var request2 = new TestRequest { Value = "test2" };

        // Act
        await mediator.MediateAsync<string>(request1);
        await mediator.MediateAsync<string>(request2);

        // Assert
        // Handler should be created twice (once for each request), but wrapper should be cached
        Assert.Equal(2, handlerCallCount);
    }

    [Fact]
    public async Task MediateAsync_Should_Handle_Multiple_Different_Request_Types()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, string>, TestRequestHandler>();
        services.AddTransient<IRequestHandler<TestRequestWithResponse, TestResponse>, TestRequestWithResponseHandler>();
        var serviceProvider = services.BuildServiceProvider();
        
        var mediator = new RequestHandlerWrapperMediator(serviceProvider);
        var request1 = new TestRequest { Value = "test1" };
        var request2 = new TestRequestWithResponse { Input = "test2" };

        // Act
        var result1 = await mediator.MediateAsync<string>(request1);
        var result2 = await mediator.MediateAsync<TestResponse>(request2);

        // Assert
        Assert.Equal("Handled: test1", result1);
        Assert.NotNull(result2);
        Assert.Equal("Processed: test2", result2.Result);
    }

    #endregion

    #region RequestHandlerWrapper Tests

    [Fact]
    public void RequestHandlerWrapper_Should_Be_Abstract()
    {
        // Arrange & Act
        var type = typeof(RequestHandlerWrapper<>);

        // Assert
        Assert.True(type.IsAbstract);
        Assert.True(type.IsGenericTypeDefinition);
    }

    [Fact]
    public void RequestHandlerWrapper_Should_Implement_IBaseRequestHandler()
    {
        // Arrange & Act
        var type = typeof(RequestHandlerWrapper<string>);

        // Assert
        Assert.True(typeof(IBaseRequestHandler).IsAssignableFrom(type));
    }

    #endregion

    #region RequestHandlerWrapperImpl Tests

    [Fact]
    public void RequestHandlerWrapperImpl_Should_Inherit_From_RequestHandlerWrapper()
    {
        // Arrange & Act
        var type = typeof(RequestHandlerWrapperImpl<TestRequest, string>);
        var baseType = typeof(RequestHandlerWrapper<string>);

        // Assert
        Assert.True(baseType.IsAssignableFrom(type));
    }

    [Fact]
    public async Task RequestHandlerWrapperImpl_HandleAsync_Should_Call_ServiceProvider_GetRequiredService()
    {
        // Arrange
        var mockHandler = new Mock<IRequestHandler<TestRequest, string>>();
        mockHandler.Setup(h => h.HandleAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync("test result");

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequest, string>)))
                          .Returns(mockHandler.Object);

        var wrapper = new RequestHandlerWrapperImpl<TestRequest, string>();
        var request = new TestRequest { Value = "test" };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await wrapper.HandleAsync(request, mockServiceProvider.Object, cancellationToken);

        // Assert
        Assert.Equal("test result", result);
        mockServiceProvider.Verify(sp => sp.GetService(typeof(IRequestHandler<TestRequest, string>)), Times.Once);
        mockHandler.Verify(h => h.HandleAsync(request, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RequestHandlerWrapperImpl_HandleAsync_Should_Cast_Request_Correctly()
    {
        // Arrange
        var mockHandler = new Mock<IRequestHandler<TestRequest, string>>();
        mockHandler.Setup(h => h.HandleAsync(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync("casted result");

        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, string>>(_ => mockHandler.Object);
        var serviceProvider = services.BuildServiceProvider();

        var wrapper = new RequestHandlerWrapperImpl<TestRequest, string>();
        var request = new TestRequest { Value = "cast test" };

        // Act
        var result = await wrapper.HandleAsync(request, serviceProvider, CancellationToken.None);

        // Assert
        Assert.Equal("casted result", result);
        mockHandler.Verify(h => h.HandleAsync(It.Is<TestRequest>(r => r.Value == "cast test"), CancellationToken.None), Times.Once);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Full_Integration_Test_With_Real_ServiceProvider()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, string>, TestRequestHandler>();
        services.AddTransient<IRequestHandler<TestRequestWithResponse, TestResponse>, TestRequestWithResponseHandler>();
        services.AddTransient<IMediator, RequestHandlerWrapperMediator>();
        
        var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var stringRequest = new TestRequest { Value = "integration test" };
        var complexRequest = new TestRequestWithResponse { Input = "complex integration test" };

        // Act
        var stringResult = await mediator.MediateAsync<string>(stringRequest);
        var complexResult = await mediator.MediateAsync<TestResponse>(complexRequest);

        // Assert
        Assert.Equal("Handled: integration test", stringResult);
        Assert.NotNull(complexResult);
        Assert.Equal("Processed: complex integration test", complexResult.Result);
    }

    #endregion
}
