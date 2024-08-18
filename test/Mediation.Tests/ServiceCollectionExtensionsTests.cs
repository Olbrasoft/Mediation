namespace Olbrasoft.Mediation.Tests;
public class ServiceCollectionExtensionsTests
{

    //add test if ServiceCollectionExtensions namespace is Olbrasoft.Extensions.DependencyInjection
    [Fact]
    public void ServiceCollectionExtensions_Namespace_ShouldBe_Olbrasoft_Extensions_DependencyInjection()
    {
        // Arrange
        var expectedNamespace = "Olbrasoft.Mediation";

        // Act
        var actualNamespace = typeof(ServiceCollectionExtensions).Namespace;

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }




}
