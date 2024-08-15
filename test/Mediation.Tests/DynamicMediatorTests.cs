using Moq;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;
public class DynamicMediatorTests
{
    //test DynamicMediator should be public class
    [Fact]
    public void Should_Be_Public_Class()
    {
        //Arrange
        var type = typeof(DynamicMediator);

        //Act
        var isPublic = type.IsPublic;

        //Assert
        Assert.True(isPublic);
    }

    //test DynamicMediator should be in Olbrasoft.Mediation assembly
    [Fact]
    public void Should_Be_In_Olbrasoft_Mediation_Assembly()
    {
        //Arrange
        var type = typeof(DynamicMediator);

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
        var type = typeof(DynamicMediator);

        //Act
        var @namespace = type.Namespace;

        //Assert
        Assert.Equal("Olbrasoft.Mediation", @namespace);
    }

    //test DynamicMediator should be implement IMediator
    [Fact]
    public void Should_Implement_IMediator()
    {
        //Arrange
        var type = typeof(DynamicMediator);

        //Act
        var implements = type.GetInterfaces().Contains(typeof(IMediator));

        //Assert
        Assert.True(implements);

    }

    //test MediateAsync mock getHandler verify call _getHandler
    [Fact]
    public async Task MediateAsync_Should_Call_GetHandler()
    {
        //Arrange
        var request = new Mock<IRequest<object>>();
        var handler = new Mock<IRequestHandler<IRequest<object>, object>>();

        var getHandler = new Mock<Func<Type, object>>();
        getHandler.Setup(m => m(It.IsAny<Type>())).Returns(handler.Object);

        var mediator = new DynamicMediator(getHandler.Object);

        //Act
        await mediator.MediateAsync(request.Object);

        //Assert
        getHandler.Verify(m => m(It.IsAny<Type>()), Times.Once);
    }

    //MediateAsync throw ArgumentNullException if request is null
    [Fact]
    public async Task MediateAsync_Throw_ArgumentNullException_If_Request_Is_Null()
    {
        //Arrange
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        IRequest<string> request = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        var handler = new Mock<IRequestHandler<IRequest<object>, object>>();

        var getHandler = new Mock<Func<Type, object>>();
        getHandler.Setup(m => m(It.IsAny<Type>())).Returns(handler.Object);

        var mediator = new DynamicMediator(getHandler.Object);

        //Act
#pragma warning disable CS8604 // Possible null reference argument.
        async Task Act() => await mediator.MediateAsync(request);
#pragma warning restore CS8604 // Possible null reference argument.

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
    }

}
