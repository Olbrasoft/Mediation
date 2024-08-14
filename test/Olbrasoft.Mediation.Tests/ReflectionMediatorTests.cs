using Moq;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;
public class ReflectionMediatorTests
{
    //test ReflectionMediator should be public class
    [Fact]
    public void Should_Be_Public_Class()
    {
        //Arrange
        var type = typeof(ReflectionMediator);

        //Act
        var isPublic = type.IsPublic;

        //Assert
        Assert.True(isPublic);
    }

    //test ReflectionMediator should be in Olbrasoft.Mediation assembly
    [Fact]
    public void Should_Be_In_Olbrasoft_Mediation_Assembly()
    {
        //Arrange
        var type = typeof(ReflectionMediator);

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
        var type = typeof(ReflectionMediator);

        //Act
        var @namespace = type.Namespace;

        //Assert
        Assert.Equal("Olbrasoft.Mediation", @namespace);
    }

    //test ReflectionMediator should BE implement IMediator
    [Fact]
    public void Should_Implement_IMediator()
    {
        //Arrange
        var type = typeof(ReflectionMediator);

        //Act
        var implements = type.GetInterfaces().Contains(typeof(IMediator));

        //Assert
        Assert.True(implements);
    }

    //MediateAsync mock getHandler verify call _getHandler
    [Fact]
    public void MediateAsync_Mock_GetHandler_Verify_Call_GetHandler()
    {
        //Arrange
        var request = new Mock<IRequest<string>>();
        var token = new CancellationToken();
        var handler = new Mock<IRequestHandler<IRequest<string>, string>>();
        var getHandler = new Mock<Func<Type, object>>();
        var mediator = new ReflectionMediator(getHandler.Object);

        getHandler.Setup(g => g(It.IsAny<Type>())).Returns(handler.Object);

        //Act
        mediator.MediateAsync(request.Object, token);

        //Assert
        getHandler.Verify(g => g(It.IsAny<Type>()), Times.Once);
    }

}
