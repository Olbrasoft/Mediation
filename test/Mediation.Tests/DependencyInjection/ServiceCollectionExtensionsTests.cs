using Olbrasoft.Extensions.DependencyInjection;

namespace Olbrasoft.Mediation.Tests.DependencyInjection;
public class ServiceCollectionExtensionsTests
{

    //add test if ServiceCollectionExtensions namespace is Olbrasoft.Extensions.DependencyInjection
    [Fact]
    public void ServiceCollectionExtensions_Namespace_ShouldBe_Olbrasoft_Extensions_DependencyInjection()
    {
        // Arrange
        var expectedNamespace = "Olbrasoft.Extensions.DependencyInjection";

        // Act
        var actualNamespace = typeof(ServiceCollectionExtensions).Namespace;

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }




}
