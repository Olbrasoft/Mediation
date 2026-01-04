using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Examples.Records;

/// <summary>
/// Simple example demonstrating record-based queries with the Mediation library.
/// Records are perfect for immutable queries in CQRS patterns.
/// </summary>
public class SimpleRecordQueryExample
{
    #region Query Definitions

    /// <summary>
    /// Simple record query with primary constructor.
    /// Benefits: Immutable, concise syntax, value-based equality.
    /// </summary>
    public record GetUserByIdQuery(int UserId) : IRequest<UserDto>;

    /// <summary>
    /// Record DTO for query response.
    /// Records are ideal for DTOs - immutable, value equality, pattern matching support.
    /// </summary>
    public record UserDto(int Id, string Name, string Email);

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Handler for the record-based query.
    /// Works exactly the same as with class-based requests.
    /// </summary>
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _repository;

        public GetUserByIdHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            // Fetch user from repository
            var user = await _repository.GetByIdAsync(query.UserId);

            if (user is null)
            {
                throw new InvalidOperationException($"User with ID {query.UserId} was not found.");
            }

            // Map to record DTO
            return new UserDto(user.Id, user.Name, user.Email);
        }
    }

    #endregion

    #region Supporting Types

    /// <summary>
    /// Example domain entity (class).
    /// Note: Entities are typically classes, not records, because they have identity and lifecycle.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// Example repository interface.
    /// </summary>
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
    }

    #endregion

    #region Usage Example

    /// <summary>
    /// Example of using the record query with mediator.
    /// </summary>
    public class UsageExample
    {
        private readonly IMediator _mediator;

        public UsageExample(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<UserDto> GetUserAsync(int userId)
        {
            // Create query using primary constructor
            var query = new GetUserByIdQuery(userId);

            // Send through mediator
            var result = await _mediator.MediateAsync<UserDto>(query);

            return result;
        }

        /// <summary>
        /// Demonstrates record equality - useful for caching, deduplication, etc.
        /// </summary>
        public void DemonstrateRecordEquality()
        {
            var query1 = new GetUserByIdQuery(42);
            var query2 = new GetUserByIdQuery(42);
            var query3 = new GetUserByIdQuery(99);

            // Value-based equality - both queries are equal
            bool areEqual = query1 == query2; // true
            bool areDifferent = query1 != query3; // true

            // Records also override GetHashCode properly
            var hashCode1 = query1.GetHashCode();
            var hashCode2 = query2.GetHashCode(); // Same as hashCode1
        }

        /// <summary>
        /// Demonstrates record immutability and 'with' expression.
        /// </summary>
        public void DemonstrateRecordImmutability()
        {
            var originalQuery = new GetUserByIdQuery(42);

            // Cannot modify properties directly - this would be a compile error:
            // originalQuery.UserId = 99; // Compile error: property is init-only

            // Instead, use 'with' to create modified copy
            var modifiedQuery = originalQuery with { UserId = 99 };

            // Original remains unchanged
            Console.WriteLine(originalQuery.UserId); // 42
            Console.WriteLine(modifiedQuery.UserId); // 99
        }
    }

    #endregion
}
