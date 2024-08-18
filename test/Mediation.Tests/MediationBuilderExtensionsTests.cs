namespace Olbrasoft.Mediation.Tests;
public class MediationBuilderExtensionsTests
{
    //add test if MediationBuilderExtensions namespace is Olbrasoft.Extensions.DependencyInjection
    [Fact]
    public void MediationBuilderExtensions_Namespace_ShouldBe_Olbrasoft_Extensions_DependencyInjection()
    {
        // Arrange
        var expectedNamespace = "Olbrasoft.Extensions.DependencyInjection";

        // Act
        var actualNamespace = typeof(MediationBuilderExtensions).Namespace;

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }



}
