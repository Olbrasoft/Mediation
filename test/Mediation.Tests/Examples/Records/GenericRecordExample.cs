using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Examples.Records;

/// <summary>
/// Examples demonstrating generic record requests with the Mediation library.
/// Shows how to use type parameters with records for reusable query patterns.
/// </summary>
public class GenericRecordExample
{
    #region Generic Query Records

    /// <summary>
    /// Generic record query for fetching any entity by ID.
    /// Type parameter T represents the result type.
    /// </summary>
    public record GetEntityByIdQuery<T>(int Id) : IRequest<T?> where T : class;

    /// <summary>
    /// Generic record query with multiple parameters.
    /// </summary>
    public record SearchEntitiesQuery<T>(
        string SearchTerm,
        int PageSize,
        int PageNumber
    ) : IRequest<List<T>> where T : class;

    /// <summary>
    /// Generic record with multiple type parameters.
    /// TEntity = source type, TDto = result type.
    /// </summary>
    public record MapEntityQuery<TEntity, TDto>(TEntity Entity) : IRequest<TDto>
        where TEntity : class
        where TDto : class;

    #endregion

    #region Generic Handler Implementation

    /// <summary>
    /// Generic handler for GetEntityByIdQuery.
    /// Note: Generic handlers typically need manual registration.
    /// </summary>
    public class GetEntityByIdHandler<T> : IRequestHandler<GetEntityByIdQuery<T>, T?>
        where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GetEntityByIdHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T?> HandleAsync(GetEntityByIdQuery<T> query, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(query.Id);
        }
    }

    /// <summary>
    /// Generic search handler.
    /// </summary>
    public class SearchEntitiesHandler<T> : IRequestHandler<SearchEntitiesQuery<T>, List<T>>
        where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public SearchEntitiesHandler(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<List<T>> HandleAsync(SearchEntitiesQuery<T> query, CancellationToken cancellationToken)
        {
            return await _repository.SearchAsync(
                query.SearchTerm,
                query.PageSize,
                query.PageNumber
            );
        }
    }

    #endregion

    #region Concrete Generic Queries

    /// <summary>
    /// Example: Concrete usage of generic record for specific entity types.
    /// </summary>
    public class ConcreteExamples
    {
        // User-specific query using generic record
        public record GetUserQuery(int Id) : GetEntityByIdQuery<User>(Id);

        // Product-specific query using generic record
        public record GetProductQuery(int Id) : GetEntityByIdQuery<Product>(Id);

        // Search users using generic search
        public record SearchUsersQuery(string Term, int Size, int Page)
            : SearchEntitiesQuery<User>(Term, Size, Page);
    }

    #endregion

    #region Generic Record with Constraints

    /// <summary>
    /// Generic record with interface constraint.
    /// Only works with types implementing IIdentifiable.
    /// </summary>
    public record GetIdentifiableQuery<T>(Guid Id) : IRequest<T?>
        where T : class, IIdentifiable;

    /// <summary>
    /// Generic record with struct constraint for value types.
    /// </summary>
    public record CalculateQuery<T>(T Value1, T Value2) : IRequest<T>
        where T : struct, IComparable<T>;

    /// <summary>
    /// Handler for identifiable entities.
    /// </summary>
    public class GetIdentifiableHandler<T> : IRequestHandler<GetIdentifiableQuery<T>, T?>
        where T : class, IIdentifiable
    {
        private readonly IIdentifiableRepository<T> _repository;

        public GetIdentifiableHandler(IIdentifiableRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T?> HandleAsync(GetIdentifiableQuery<T> query, CancellationToken cancellationToken)
        {
            return await _repository.FindByIdAsync(query.Id);
        }
    }

    #endregion

    #region Supporting Types

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public interface IIdentifiable
    {
        Guid Id { get; }
    }

    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> SearchAsync(string searchTerm, int pageSize, int pageNumber);
    }

    public interface IIdentifiableRepository<T> where T : class, IIdentifiable
    {
        Task<T?> FindByIdAsync(Guid id);
    }

    #endregion

    #region Usage Examples

    public class UsageExample
    {
        private readonly IMediator _mediator;

        public UsageExample(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Using generic record query with specific type.
        /// </summary>
        public async Task<User?> GetUserExample()
        {
            // Create generic query for User type
            var query = new GetEntityByIdQuery<User>(42);

            // Mediator resolves to GetEntityByIdHandler<User>
            var user = await _mediator.MediateAsync<User?>(query);

            return user;
        }

        /// <summary>
        /// Using generic search query.
        /// </summary>
        public async Task<List<Product>> SearchProductsExample()
        {
            var query = new SearchEntitiesQuery<Product>("laptop", 20, 1);
            var products = await _mediator.MediateAsync<List<Product>>(query);
            return products;
        }

        /// <summary>
        /// Using concrete query derived from generic record.
        /// </summary>
        public async Task<User?> GetUserWithConcreteQuery()
        {
            var query = new ConcreteExamples.GetUserQuery(42);
            var user = await _mediator.MediateAsync<User?>(query);
            return user;
        }

        /// <summary>
        /// Demonstrates type safety with generic records.
        /// </summary>
        public void DemonstrateTypeSafety()
        {
            // Type-safe - compiler knows result is User?
            var userQuery = new GetEntityByIdQuery<User>(42);

            // Type-safe - compiler knows result is List<Product>
            var productQuery = new SearchEntitiesQuery<Product>("search", 10, 1);

            // Compile error if types don't match constraints
            // var invalidQuery = new GetEntityByIdQuery<int>(42); // Error: int is not a class
        }

        /// <summary>
        /// Demonstrates generic record equality.
        /// </summary>
        public void DemonstrateGenericRecordEquality()
        {
            var query1 = new GetEntityByIdQuery<User>(42);
            var query2 = new GetEntityByIdQuery<User>(42);
            var query3 = new GetEntityByIdQuery<Product>(42);

            // Same type parameter and value - equal
            bool areEqual = query1 == query2; // true

            // Different type parameter - not equal (different types)
            // bool notComparable = query1 == query3; // Compile error: can't compare different types
        }
    }

    #endregion
}
