using Moq;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;
public class RequestHandlerTests
{
    [Fact]
    public void Constructor_Should_Throw_ArgumentNullException_When_Handler_Is_Null()
    {
        //Arrange
        IRequestHandler<IRequest<object>, object> handler = null!;

        //Act
        var exception = Assert.Throws<ArgumentNullException>(() => new RequestHandler<IRequest<object>, object>(handler));

        //Assert
        Assert.Equal("handler", exception.ParamName);
    }

    [Fact]
    public async Task HandleAsync_Should_Throw_ArgumentNullException_When_Request_Is_Null()
    {
        //Arrange
        IRequest<object> request = null!;
        var handler = new Mock<IRequestHandler<IRequest<object>, object>>();

        var requestHandler = new RequestHandler<IRequest<object>, object>(handler.Object);

        //Act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => requestHandler.HandleAsync(request));

        //Assert
        Assert.Equal("request", exception.ParamName);
    }

    [Fact]
    public async Task HandleAsync_Should_Call_HandleAsync_From_Handler()
    {
        //Arrange
        var request = new Mock<IRequest<object>>();
        var handler = new Mock<IRequestHandler<IRequest<object>, object>>();

        var requestHandler = new RequestHandler<IRequest<object>, object>(handler.Object);

        //Act
        await requestHandler.HandleAsync(request.Object);

        //Assert
        handler.Verify(h => h.HandleAsync(request.Object, default));
    }
}
