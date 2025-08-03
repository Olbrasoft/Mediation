using Moq;
using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Tests;

public class BaseRequestTests
{
    // Konkrétní implementace BaseRequest pro testování
    private class TestRequest : BaseRequest<string>
    {
        public TestRequest(IMediator mediator) : base(mediator) { }
        public TestRequest() : base() { }
    }

    [Fact]
    public void Constructor_With_Mediator_Should_Set_Mediator_Property()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();

        // Act
        var request = new TestRequest(mockMediator.Object);

        // Assert
        Assert.NotNull(request.Mediator);
        Assert.Equal(mockMediator.Object, request.Mediator);
    }

    [Fact]
    public void Constructor_With_Null_Mediator_Should_Throw_ArgumentNullException()
    {
        // Arrange
        IMediator nullMediator = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new TestRequest(nullMediator));
        Assert.Equal("mediator", exception.ParamName);
    }

    [Fact]
    public void Constructor_Parameterless_Should_Set_Mediator_To_Null()
    {
        // Arrange & Act
        var request = new TestRequest();

        // Assert
        Assert.Null(request.Mediator);
    }

    [Fact]
    public void BaseRequest_Should_Implement_IRequest_Interface()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();

        // Act
        var request = new TestRequest(mockMediator.Object);

        // Assert
        Assert.IsAssignableFrom<IRequest<string>>(request);
    }

    [Fact]
    public void BaseRequest_Should_Be_Abstract_Class()
    {
        // Arrange & Act
        var type = typeof(BaseRequest<>);

        // Assert
        Assert.True(type.IsAbstract);
    }

    [Fact]
    public void Mediator_Property_Should_Be_ReadOnly()
    {
        // Arrange
        var type = typeof(BaseRequest<string>);
        var property = type.GetProperty(nameof(BaseRequest<string>.Mediator));

        // Act & Assert
        Assert.NotNull(property);
        Assert.True(property.CanRead);
        Assert.False(property.CanWrite);
    }

    [Fact]
    public void Mediator_Property_Should_Have_Nullable_Type()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var request = new TestRequest(mockMediator.Object);
        var nullableRequest = new TestRequest();

        // Act & Assert
        Assert.NotNull(request.Mediator);
        Assert.Null(nullableRequest.Mediator);
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(string))]
    [InlineData(typeof(object))]
    [InlineData(typeof(List<string>))]
    public void BaseRequest_Should_Support_Generic_Response_Types(Type responseType)
    {
        // Arrange
        var genericBaseRequestType = typeof(BaseRequest<>).MakeGenericType(responseType);

        // Act & Assert
        Assert.True(genericBaseRequestType.IsAbstract);
        Assert.True(genericBaseRequestType.IsGenericType);
    }
}
