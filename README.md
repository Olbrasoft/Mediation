# Mediation

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6%7C7%7C8%7C9-blue)](https://dotnet.microsoft.com/download)
[![NuGet](https://img.shields.io/nuget/v/Olbrasoft.Mediation.svg)](https://www.nuget.org/packages/Olbrasoft.Mediation/)

A lightweight, high-performance implementation of the mediator design pattern for .NET applications. This library provides a simple and efficient way to implement CQRS (Command Query Responsibility Segregation) patterns and decouple your application components.

## 🚀 Features

- **High Performance**: Optimized for speed with minimal overhead
- **Lightweight**: Zero external dependencies except Microsoft.Extensions.DependencyInjection.Abstractions (latest version 9.0.7)
- **Multiple Mediator Implementations**: Choose the right mediator for your needs
  - `RequestHandlerMediator` - Direct handler resolution
  - `DynamicMediator` - Dynamic method invocation
  - `ReflectionMediator` - Reflection-based approach
  - `RequestHandlerWrapperMediator` - Wrapper-based implementation with caching
- **Dependency Injection Ready**: Built-in support for Microsoft.Extensions.DependencyInjection
- **Multi-Target Framework**: Supports .NET 6, 7, 8, 9 and .NET Standard 2.1
- **Generic Request/Response**: Type-safe request and response handling
- **Async/Await Support**: Fully asynchronous API
- **Comprehensive Testing**: 96+ unit tests ensuring reliability

## 📦 Installation

### Package Manager
```bash
Install-Package Olbrasoft.Mediation
```

### .NET CLI
```bash
dotnet add package Olbrasoft.Mediation
```

### PackageReference
```xml
<PackageReference Include="Olbrasoft.Mediation" Version="9.0.1" />
```

## 🔧 Quick Start

### 1. Define a Request
```csharp
using Olbrasoft.Mediation.Abstractions;

public class GetUserQuery : IRequest<User>
{
    public int UserId { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

### 2. Create a Handler
```csharp
using Olbrasoft.Mediation;

public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> HandleAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(request.UserId);
    }
}
```

### 3. Register Services
```csharp
using Microsoft.Extensions.DependencyInjection;
using Olbrasoft.Mediation;

var services = new ServiceCollection();

// Register mediation with handler discovery
services.AddMediation(typeof(GetUserHandler).Assembly)
        .UseRequestHandlerMediator(); // Choose your preferred mediator

// Register your other services
services.AddScoped<IUserRepository, UserRepository>();

var serviceProvider = services.BuildServiceProvider();
```

### 4. Use the Mediator
```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<User> GetUser(int id)
    {
        var query = new GetUserQuery { UserId = id };
        return await _mediator.MediateAsync<User>(query);
    }
}
```

## 🎯 Mediator Types

### RequestHandlerMediator (Recommended)
Fast and direct handler resolution through dependency injection.

```csharp
services.AddMediation(assemblies).UseRequestHandlerMediator();
```

### DynamicMediator
Uses dynamic method invocation for flexibility.

```csharp
services.AddMediation(assemblies).UseDynamicMediator();
```

### ReflectionMediator
Reflection-based approach for complex scenarios.

```csharp
services.AddMediation(assemblies).UseReflectionMediator();
```

### RequestHandlerWrapperMediator
Wrapper-based implementation with advanced caching.

```csharp
services.AddMediation(assemblies).UseRequestHandlerWrapperMediator();
```

## 📋 Advanced Usage

### Generic Requests
```csharp
public class GetEntityQuery<T> : IRequest<T> where T : class
{
    public int Id { get; set; }
}

public class GetEntityHandler<T> : IRequestHandler<GetEntityQuery<T>, T> 
    where T : class
{
    public async Task<T> HandleAsync(GetEntityQuery<T> request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Command Pattern
```csharp
public class CreateUserCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> HandleAsync(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Create user and return ID
        return newUserId;
    }
}
```

### Multiple Assemblies
```csharp
services.AddMediation(
    typeof(UserHandler).Assembly,
    typeof(OrderHandler).Assembly,
    typeof(ProductHandler).Assembly
).UseRequestHandlerMediator();
```

## 🏗️ Architecture

The library follows a clean architecture approach:

- **Abstractions**: Core interfaces (`IRequest<T>`, `IRequestHandler<T,R>`, `IMediator`)
- **Implementations**: Various mediator implementations optimized for different scenarios
- **Extensions**: Dependency injection integration and builder pattern
- **Performance**: Minimal allocations and optimized execution paths

## 🔧 Configuration Options

### Custom Timeout
The library includes built-in protection against infinite loops in complex generic scenarios:

```csharp
// Default timeout is 15 seconds for handler registration
services.AddMediation(assemblies); // Uses default timeout

// The library will throw TimeoutException if registration takes too long
```

### Limits and Safety
- Maximum generic type parameters: 10
- Maximum types closing: 100  
- Maximum generic type registrations: 125,000
- Registration timeout: 15 seconds

## 🧪 Testing

The library includes comprehensive test coverage:

```bash
dotnet test
# Result: 96 tests passed
```

Example test:
```csharp
[Fact]
public async Task Mediator_Should_Handle_Request_Successfully()
{
    // Arrange
    var services = new ServiceCollection();
    services.AddMediation(Assembly.GetExecutingAssembly())
            .UseRequestHandlerMediator();
    
    var serviceProvider = services.BuildServiceProvider();
    var mediator = serviceProvider.GetRequiredService<IMediator>();
    
    // Act
    var result = await mediator.MediateAsync<string>(new TestRequest { Value = "test" });
    
    // Assert
    Assert.Equal("Handled: test", result);
}
```

## 🚀 Performance

Benchmarks show excellent performance characteristics:

- **Low Memory Allocation**: Minimal GC pressure
- **Fast Execution**: Optimized resolution paths
- **Scalable**: Handles thousands of requests efficiently

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Related Projects

- [MediatR](https://github.com/jbogard/MediatR) - The original inspiration
- [Microsoft.Extensions.DependencyInjection](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## 📞 Support

- Create an [Issue](https://github.com/Olbrasoft/Mediation/issues) for bug reports or feature requests
- Check the [Wiki](https://github.com/Olbrasoft/Mediation/wiki) for detailed documentation

---

**Made with ❤️ by [Olbrasoft](https://github.com/Olbrasoft)**
