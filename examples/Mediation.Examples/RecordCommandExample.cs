using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Examples.Records;

/// <summary>
/// Examples demonstrating record-based commands with the Mediation library.
/// Shows both immutable commands (records) and when to use BaseCommand (classes).
/// </summary>
public class RecordCommandExample
{
    #region Immutable Record Command

    /// <summary>
    /// Simple immutable command using record.
    /// Best for commands that don't need status tracking or events.
    /// </summary>
    public record CreateUserCommand(string Name, string Email) : IRequest<int>;

    /// <summary>
    /// Handler for immutable record command.
    /// </summary>
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserRepository _repository;

        public CreateUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
        {
            // Create new user
            var user = new User
            {
                Name = command.Name,
                Email = command.Email
            };

            // Save to repository
            await _repository.CreateAsync(user);

            return user.Id;
        }
    }

    #endregion

    #region Complex Record Command with Validation

    /// <summary>
    /// Record command with built-in validation.
    /// Uses init-only property for validation logic.
    /// </summary>
    public record CreateOrderCommand(
        int CustomerId,
        List<OrderItem> Items,
        string ShippingAddress
    ) : IRequest<OrderResult>
    {
        /// <summary>
        /// Validation property - computed from record properties.
        /// </summary>
        public bool IsValid => CustomerId > 0
            && Items.Count > 0
            && !string.IsNullOrWhiteSpace(ShippingAddress);

        /// <summary>
        /// Calculated total - demonstrates computed properties in records.
        /// </summary>
        public decimal TotalAmount => Items.Sum(i => i.Price * i.Quantity);
    }

    /// <summary>
    /// Nested record for order items.
    /// </summary>
    public record OrderItem(int ProductId, int Quantity, decimal Price);

    /// <summary>
    /// Record result type.
    /// </summary>
    public record OrderResult(int OrderId, decimal TotalAmount, DateTime CreatedAt);

    /// <summary>
    /// Handler with validation check.
    /// </summary>
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResult>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderResult> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            // Validation using record's IsValid property
            if (!command.IsValid)
            {
                throw new ArgumentException("Invalid order command");
            }

            // Create order
            var order = new Order
            {
                CustomerId = command.CustomerId,
                ShippingAddress = command.ShippingAddress,
                TotalAmount = command.TotalAmount,
                CreatedAt = DateTime.UtcNow
            };

            // Add items
            foreach (var item in command.Items)
            {
                order.Items.Add(new OrderItemEntity
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            await _orderRepository.CreateAsync(order);

            return new OrderResult(order.Id, order.TotalAmount, order.CreatedAt);
        }
    }

    #endregion

    #region When to Use Classes Instead of Records

    /// <summary>
    /// When NOT to use records: Commands needing mutable state or complex lifecycle.
    /// Use classes when you need to track command execution state or have mutable properties.
    /// </summary>
    /// <remarks>
    /// This example shows a class-based command for scenarios where:
    /// - Command has mutable state (e.g., status tracking)
    /// - Command needs to be modified after creation
    /// - Command has complex lifecycle management
    ///
    /// Note: For most cases, immutable record commands are preferred.
    /// Consider using result objects to communicate status rather than mutable command state.
    /// </remarks>
    public class ProcessPaymentCommand : IRequest<PaymentResult>
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        // Mutable state - this is why we use a class
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public List<string> AuditLog { get; } = new();

        public void AddAuditEntry(string entry)
        {
            AuditLog.Add($"{DateTime.UtcNow:O}: {entry}");
        }
    }

    public enum PaymentStatus { Pending, Processing, Completed, Failed }

    public record PaymentResult(bool Success, string TransactionId, PaymentStatus FinalStatus);

    /// <summary>
    /// Recommended approach: Use record command with result object that contains status.
    /// This maintains immutability while still communicating execution status.
    /// </summary>
    public record ProcessPaymentRecordCommand(decimal Amount, string PaymentMethod) : IRequest<PaymentResult>;

    #endregion

    #region Supporting Types

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemEntity> Items { get; set; } = new();
    }

    public class OrderItemEntity
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public interface IUserRepository
    {
        Task CreateAsync(User user);
    }

    public interface IOrderRepository
    {
        Task CreateAsync(Order order);
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

        public async Task CreateUserExample()
        {
            // Simple immutable command
            var command = new CreateUserCommand("John Doe", "john@example.com");
            var userId = await _mediator.MediateAsync<int>(command);
        }

        public async Task CreateOrderExample()
        {
            // Complex command with nested records
            var command = new CreateOrderCommand(
                CustomerId: 123,
                Items: new List<OrderItem>
                {
                    new(ProductId: 1, Quantity: 2, Price: 29.99m),
                    new(ProductId: 2, Quantity: 1, Price: 49.99m)
                },
                ShippingAddress: "123 Main St, City, Country"
            );

            // Command validates itself
            if (command.IsValid)
            {
                var result = await _mediator.MediateAsync<OrderResult>(command);
                Console.WriteLine($"Order created: {result.OrderId}, Total: {result.TotalAmount}");
            }
        }

        /// <summary>
        /// Demonstrates command immutability - creating modified copies.
        /// </summary>
        public void DemonstrateCommandModification()
        {
            var original = new CreateUserCommand("John", "john@example.com");

            // Create modified copy
            var modified = original with { Email = "newemail@example.com" };

            // Original unchanged
            Console.WriteLine(original.Email); // john@example.com
            Console.WriteLine(modified.Email); // newemail@example.com
        }
    }

    #endregion
}
