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

    #region UseRequestHandlerWrapperMediator Tests

    [Fact]
    public void UseRequestHandlerWrapperMediator_Should_Add_IMediator_ServiceDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        var result = builder.UseRequestHandlerWrapperMediator();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.Equal(typeof(RequestHandlerWrapperMediator), descriptor.ImplementationType);
        Assert.Same(services, result);
    }

    [Fact]
    public void UseRequestHandlerWrapperMediator_Should_Add_IMediator_With_Custom_Lifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseRequestHandlerWrapperMediator(ServiceLifetime.Singleton);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.Equal(typeof(RequestHandlerWrapperMediator), descriptor.ImplementationType);
    }

    [Fact]
    public void UseRequestHandlerWrapperMediator_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        var result = builder.UseRequestHandlerWrapperMediator();

        // Assert
        Assert.Same(services, result);
        Assert.IsType<ServiceCollection>(result);
    }

    #endregion

    #region UseRequestHandlerMediator Tests

    [Fact]
    public void UseRequestHandlerMediator_Should_Add_Func_Type_IBaseRequestHandler_ServiceDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseRequestHandlerMediator();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(Func<Type, IBaseRequestHandler>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.NotNull(descriptor.ImplementationFactory);
        Assert.Null(descriptor.ImplementationType);
    }

    [Fact]
    public void UseRequestHandlerMediator_Should_Add_RequestHandler_ServiceDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseRequestHandlerMediator();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(RequestHandler<,>));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.Equal(typeof(RequestHandler<,>), descriptor.ImplementationType);
    }

    [Fact]
    public void UseRequestHandlerMediator_Should_Add_IMediator_ServiceDescriptor()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseRequestHandlerMediator();

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.Equal(typeof(RequestHandlerMediator), descriptor.ImplementationType);
    }

    [Fact]
    public void UseRequestHandlerMediator_Should_Add_Services_With_Custom_Lifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseRequestHandlerMediator(ServiceLifetime.Scoped);

        // Assert
        var funcDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(Func<Type, IBaseRequestHandler>));
        var requestHandlerDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(RequestHandler<,>));
        var mediatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));

        Assert.NotNull(funcDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, funcDescriptor.Lifetime);

        Assert.NotNull(requestHandlerDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, requestHandlerDescriptor.Lifetime);

        Assert.NotNull(mediatorDescriptor);
        Assert.Equal(ServiceLifetime.Scoped, mediatorDescriptor.Lifetime);
    }

    [Fact]
    public void UseRequestHandlerMediator_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        var result = builder.UseRequestHandlerMediator();

        // Assert
        Assert.Same(services, result);
        Assert.IsType<ServiceCollection>(result);
    }

    [Fact]
    public void UseRequestHandlerMediator_Factory_Should_Create_Valid_Function()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);
        builder.UseRequestHandlerMediator();
        var provider = services.BuildServiceProvider();

        // Act
        var factory = provider.GetRequiredService<Func<Type, IBaseRequestHandler>>();

        // Assert
        Assert.NotNull(factory);
    }

    #endregion

    #region UseDynamicMediator Additional Tests

    [Fact]
    public void UseDynamicMediator_Should_Add_Services_With_Custom_Lifetime()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseDynamicMediator(ServiceLifetime.Singleton);

        // Assert
        var funcDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(Func<Type, dynamic>));
        var mediatorDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMediator));

        Assert.NotNull(funcDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, funcDescriptor.Lifetime);

        Assert.NotNull(mediatorDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, mediatorDescriptor.Lifetime);
    }

    [Fact]
    public void UseDynamicMediator_Should_Return_ServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        var result = builder.UseDynamicMediator();

        // Assert
        Assert.Same(services, result);
        Assert.IsType<ServiceCollection>(result);
    }

    #endregion

    #region Integration Tests

    [Theory]
    [InlineData(ServiceLifetime.Transient)]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton)]
    public void All_Extension_Methods_Should_Support_Different_Lifetimes(ServiceLifetime lifetime)
    {
        // Arrange
        var services1 = new ServiceCollection();
        var services2 = new ServiceCollection();
        var services3 = new ServiceCollection();
        var builder1 = new MediationBuilder(services1);
        var builder2 = new MediationBuilder(services2);
        var builder3 = new MediationBuilder(services3);

        // Act
        builder1.UseRequestHandlerWrapperMediator(lifetime);
        builder2.UseRequestHandlerMediator(lifetime);
        builder3.UseDynamicMediator(lifetime);

        // Assert
        var mediatorDescriptor1 = services1.First(d => d.ServiceType == typeof(IMediator));
        var mediatorDescriptor2 = services2.First(d => d.ServiceType == typeof(IMediator));
        var mediatorDescriptor3 = services3.First(d => d.ServiceType == typeof(IMediator));

        Assert.Equal(lifetime, mediatorDescriptor1.Lifetime);
        Assert.Equal(lifetime, mediatorDescriptor2.Lifetime);
        Assert.Equal(lifetime, mediatorDescriptor3.Lifetime);
    }

    [Fact]
    public void TryAdd_Should_Not_Register_Same_Service_Twice()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = new MediationBuilder(services);

        // Act
        builder.UseRequestHandlerMediator();
        builder.UseRequestHandlerMediator(); // Call twice

        // Assert
        var mediatorDescriptors = services.Where(d => d.ServiceType == typeof(IMediator)).ToList();
        Assert.Single(mediatorDescriptors); // Should only have one registration
    }

    #endregion
}
