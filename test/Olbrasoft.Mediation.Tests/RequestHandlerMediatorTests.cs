using Moq;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;
public class RequestHandlerMediatorTests
{
    //test RequestHandlerMediator should be public class
    [Fact]
    public void Should_Be_Public_Class()
    {
        //Arrange
        var type = typeof(RequestHandlerMediator);

        //Act
        var isPublic = type.IsPublic;

        //Assert
        Assert.True(isPublic);
    }

    //test RequestHandlerMediator should be in Olbrasoft.Mediation assembly
    [Fact]
    public void Should_Be_In_Olbrasoft_Mediation_Assembly()
    {
        //Arrange
        var type = typeof(RequestHandlerMediator);

        //Act
        var assembly = type.Assembly;

        //Assert
        Assert.Equal("Olbrasoft.Mediation", assembly.GetName().Name);
    }

    //test mediator namespace should be Olbrasoft.Mediation
    [Fact]
    public void Should_Have_Namespace_Olbrasoft_Mediation()
    {
        //Arrange
        var type = typeof(RequestHandlerMediator);

        //Act
        var @namespace = type.Namespace;

        //Assert
        Assert.Equal("Olbrasoft.Mediation", @namespace);
    }

    //test RequestHandlerMediator should implement IMediator
    [Fact]
    public void Should_Implement_IMediator()
    {
        //Arrange
        var type = typeof(RequestHandlerMediator);

        //Act
        var implements = type.GetInterfaces().Contains(typeof(IMediator));

        //Assert
        Assert.True(implements);
    }

    //test RequestHandlerMediator should have MediateAsync method
    [Fact]
    public void Should_Have_MediateAsync_Method()
    {
        //Arrange
        var type = typeof(RequestHandlerMediator);

        //Act
        var method = type.GetMethod("MediateAsync");

        //Assert
        Assert.NotNull(method);
    }

    //test MediateAsync verify getHandlerMock getHandler is called
    [Fact]
    public async Task MediateAsync_Verify_Mock_GetHandler_Is_Called()
    {
        //Arrange
        var handler = new Mock<IRequestHandler<string>>();

        var getHandlerMock = new Mock<Func<Type, IBaseRequestHandler>>();
        getHandlerMock.Setup(m => m(It.IsAny<Type>())).Returns(handler.Object);

        var mediator = new RequestHandlerMediator(getHandlerMock.Object);
        var request = new Mock<IRequest<string>>();
        var token = new CancellationToken();

        //Act
        await mediator.MediateAsync(request.Object, token);

        //Assert
        getHandlerMock.Verify(m => m(It.IsAny<Type>()), Times.Once);


    }

    //test MediateAsync throws ArgumentNullException when request is null
    [Fact]
    public async Task MediateAsync_Throws_ArgumentNullException_When_Request_Is_Null()
    {
        //Arrange
        var handler = new Mock<IRequestHandler<string>>();

        var getHandlerMock = new Mock<Func<Type, IBaseRequestHandler>>();
        getHandlerMock.Setup(m => m(It.IsAny<Type>())).Returns(handler.Object);

        var mediator = new RequestHandlerMediator(getHandlerMock.Object);
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IRequest<string> request = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        var token = new CancellationToken();

        //Act
#pragma warning disable CS8604 // Possible null reference argument.
        async Task Act() => await mediator.MediateAsync(request, token);
#pragma warning restore CS8604 // Possible null reference argument.

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }


}
