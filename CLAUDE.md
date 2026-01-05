# Mediation - Claude Code Documentation

## Repository Overview

**Location:** `/home/jirka/Olbrasoft/Mediation/`
**Type:** .NET Library (Mediator Pattern Implementation)
**Target Frameworks:** .NET 6, 7, 8, 9, 10, .NET Standard 2.1
**NuGet Package:** [Olbrasoft.Mediation](https://www.nuget.org/packages/Olbrasoft.Mediation/)

## Purpose

Lightweight, high-performance implementation of the mediator design pattern for .NET applications. Provides foundation for CQRS (Command Query Responsibility Segregation) patterns and decoupled application components.

## Current State Analysis

### Projects Structure

```
Mediation/
├── src/
│   ├── Mediation.Abstractions/     # Core interfaces (IRequest<T>)
│   └── Mediation/                  # Mediator implementations
└── test/
    ├── Mediation.Tests/            # Unit tests (96+ tests)
    └── Olbrasoft.Mediation.Benchmarks/
```

### Key Components

#### 1. Core Interfaces (Mediation.Abstractions)

- **IRequest&lt;TResponse>** - Marker interface for requests
  - Simple interface with no methods
  - Used as type constraint for handlers

#### 2. Base Classes (Mediation)

- **BaseRequest&lt;TResponse>** - Abstract base class for requests
  - Optional IMediator dependency injection
  - Two constructors: with mediator and parameterless
  - **Limitation:** Abstract class - cannot be used with records

#### 3. Handler Interfaces

- **IRequestHandler&lt;TRequest, TResponse>** - Defines async request handler
- **IBaseRequestHandler** - Marker interface for handler discovery

#### 4. Mediator Implementations

1. **RequestHandlerMediator** (Recommended)
   - Direct handler resolution via DI
   - Fastest performance

2. **DynamicMediator**
   - Dynamic method invocation
   - More flexible

3. **ReflectionMediator**
   - Reflection-based approach
   - Complex scenarios

4. **RequestHandlerWrapperMediator**
   - Wrapper-based with caching

### Current Limitations - Record Support

#### What Works

- **Records implementing IRequest&lt;T> interface directly** should work with mediator
- ServiceCollectionExtensions scans for concrete types implementing IRequest&lt;T>
- `IsConcrete()` method checks: `!type.IsAbstract && !type.IsInterface`
- Records are classes, so they pass the concrete check

#### What Doesn't Work

- **No documentation** for using records
- **No tests** with record types
- **No examples** in README.md
- **BaseRequest&lt;T> cannot be used** - it's an abstract class (records can't inherit from abstract classes with constructors)

#### Why Records Aren't Supported Yet

1. **Documentation Gap:**
   - README shows only class-based examples
   - All examples use `BaseRequest<T>` inheritance
   - No mention of direct interface implementation

2. **Testing Gap:**
   - All tests use classes inheriting from `BaseRequest<T>`
   - Example: `TestRequest : BaseRequest<string>`
   - No tests verify record-based requests work

3. **Code Examples:**
   - Quick Start guide shows only class inheritance
   - No examples of records implementing `IRequest<T>` directly

### Technical Analysis - Record Compatibility

**Records CAN technically work because:**
```csharp
// This should work (but untested):
public record GetUserQuery(int UserId) : IRequest<User>;

public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    public async Task<User> HandleAsync(GetUserQuery request, CancellationToken token)
    {
        // Implementation
    }
}
```

**Records have advantages:**
- Immutability by default
- Value-based equality
- Concise syntax for DTOs
- Perfect fit for CQRS queries/commands

## Required Changes for Record Support

### 1. Add Record Tests
- Test record requests with all mediator implementations
- Test record queries and commands
- Test generic record requests
- Verify handler registration works

### 2. Update Documentation
- Add record examples to README.md
- Document direct `IRequest<T>` implementation
- Show record vs class trade-offs
- Update Quick Start with record example

### 3. Add Code Examples
- Create sample record-based queries
- Create sample record-based commands
- Show immutable request patterns
- Demonstrate record benefits

### 4. Potential API Extensions
- Consider adding marker interface helpers
- Add extension methods for record-friendly patterns
- Consider primary constructor support

## Build & Test Commands

```bash
# Navigate to repository
cd ~/Olbrasoft/Mediation

# Build solution
dotnet build

# Run tests (96+ tests should pass)
dotnet test

# Run benchmarks
cd test/Olbrasoft.Mediation.Benchmarks
dotnet run -c Release
```

## Dependencies

- **Microsoft.Extensions.DependencyInjection.Abstractions** (only dependency)
- **xUnit** (testing)
- **BenchmarkDotNet** (performance testing)

## GitHub Repository

- **URL:** https://github.com/Olbrasoft/Mediation
- **CI/CD:** GitHub Actions
  - build.yml - Build and test
  - publish-nuget.yml - Publish to NuGet.org

## Notes for Implementation

- Use xUnit for all tests (NOT NUnit)
- Follow existing test patterns in Mediation.Tests/
- Ensure all 4 mediator types work with records
- Add benchmarks for record performance
- Update version in .csproj after changes
- CI/CD will auto-publish to NuGet on version bump
