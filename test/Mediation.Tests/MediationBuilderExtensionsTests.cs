using Microsoft.Extensions.DependencyInjection;

namespace Olbrasoft.Mediation.Tests;
public class MediationBuilderExtensionsTests
{
    //add test if MediationBuilderExtensions namespace is Olbrasoft.Extensions.DependencyInjection
    [Fact]
    public void MediationBuilderExtensions_Namespace_ShouldBe_Olbrasoft_Extensions_DependencyInjection()
    {
        // Arrange
        var expectedNamespace = "Olbrasoft.Mediation";

        // Act
        var actualNamespace = typeof(MediationBuilderExtensions).Namespace;

        // Assert
        Assert.Equal(expectedNamespace, actualNamespace);
    }


    //add test UseDynamicMediator_Should_Add_ServiceDescriptor_With_Func_Type_Dynamic()
    [Fact]
    public void UseDynamicMediator_Should_Add_ServiceDescriptor_With_Func_Type_Dynamic()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseDynamicMediator();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(Func<Type, dynamic>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.NotNull(descriptor.ImplementationFactory);
        Assert.Null(descriptor.ImplementationInstance);
        Assert.Equal(typeof(Func<Type, dynamic>), descriptor.ServiceType);
        Assert.Null(descriptor.ImplementationType);
    }

    //add test UseDynamicMediator_Should_Add_ServiceDescriptor_With_IMediator()
    [Fact]
    public void UseDynamicMediator_Should_Add_ServiceDescriptor_With_IMediator()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseDynamicMediator();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));
        Assert.NotNull(descriptor);
        Assert.Equal(
            ServiceLifetime.Transient,
            descriptor.Lifetime);
        Assert.Null(descriptor.ImplementationFactory);
        Assert.Null(descriptor.ImplementationInstance);
        Assert.Equal(typeof(IMediator), descriptor.ServiceType);
        Assert.Equal(typeof(DynamicMediator), descriptor.ImplementationType);
        Assert.NotNull(descriptor.ImplementationType);

    }

    //add test UseDynamicMediator_CreateProvider_GetRequired_Func<Type, dynamic>_return_factory
    [Fact]
    public void UseDynamicMediator_CreateProvider_GetRequired_Func_Type_Dynamic_return_factory()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);
        builder.UseDynamicMediator();
        var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<Func<Type, dynamic>>();

        // Assert
        Assert.NotNull(factory);
    }




}
