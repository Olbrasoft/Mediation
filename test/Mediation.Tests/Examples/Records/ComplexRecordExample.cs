using Olbrasoft.Mediation.Abstractions;

namespace Olbrasoft.Mediation.Examples.Records;

/// <summary>
/// Complex examples demonstrating advanced record usage patterns with the Mediation library.
/// Shows nested records, validation, pagination, and real-world scenarios.
/// </summary>
public class ComplexRecordExample
{
    #region Nested Record Structures

    /// <summary>
    /// Complex query with nested record parameters.
    /// Demonstrates composing records for sophisticated query patterns.
    /// </summary>
    public record SearchProductsQuery(
        SearchCriteria Criteria,
        PagingOptions Paging,
        SortingOptions Sorting
    ) : IRequest<PagedResult<ProductDto>>;

    /// <summary>
    /// Nested record for search criteria.
    /// </summary>
    public record SearchCriteria(
        string? Keyword,
        PriceRange? Price,
        string? Category,
        bool OnlyInStock
    );

    /// <summary>
    /// Nested record for price range filtering.
    /// </summary>
    public record PriceRange(decimal Min, decimal Max)
    {
        /// <summary>
        /// Validation property.
        /// </summary>
        public bool IsValid => Min >= 0 && Max >= Min;
    }

    /// <summary>
    /// Nested record for paging parameters.
    /// </summary>
    public record PagingOptions(int Page, int PageSize)
    {
        /// <summary>
        /// Default paging options.
        /// </summary>
        public static PagingOptions Default => new(1, 20);

        /// <summary>
        /// Validation property.
        /// </summary>
        public bool IsValid => Page > 0 && PageSize > 0 && PageSize <= 100;
    }

    /// <summary>
    /// Nested record for sorting.
    /// </summary>
    public record SortingOptions(string SortBy, SortDirection Direction)
    {
        public static SortingOptions Default => new("Name", SortDirection.Ascending);
    }

    public enum SortDirection { Ascending, Descending }

    /// <summary>
    /// Generic paged result record.
    /// </summary>
    public record PagedResult<T>(
        List<T> Items,
        int TotalCount,
        int Page,
        int PageSize
    )
    {
        /// <summary>
        /// Calculated property - total pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Calculated property - has more pages.
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Calculated property - has previous page.
        /// </summary>
        public bool HasPreviousPage => Page > 1;
    }

    /// <summary>
    /// Product DTO record.
    /// </summary>
    public record ProductDto(
        int Id,
        string Name,
        decimal Price,
        string Category,
        bool InStock
    );

    #endregion

    #region Complex Handler with Validation

    public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductDto>>
    {
        private readonly IProductRepository _repository;

        public SearchProductsHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ProductDto>> HandleAsync(
            SearchProductsQuery query,
            CancellationToken cancellationToken)
        {
            // Validate nested records
            if (!query.Paging.IsValid)
            {
                throw new ArgumentException("Invalid paging options");
            }

            if (query.Criteria.Price != null && !query.Criteria.Price.IsValid)
            {
                throw new ArgumentException("Invalid price range");
            }

            // Build query using nested record properties
            var products = await _repository.SearchAsync(
                keyword: query.Criteria.Keyword,
                priceMin: query.Criteria.Price?.Min,
                priceMax: query.Criteria.Price?.Max,
                category: query.Criteria.Category,
                onlyInStock: query.Criteria.OnlyInStock,
                sortBy: query.Sorting.SortBy,
                sortDirection: query.Sorting.Direction,
                page: query.Paging.Page,
                pageSize: query.Paging.PageSize
            );

            // Map to DTOs
            var dtos = products.Items.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Price,
                p.Category,
                p.InStock
            )).ToList();

            return new PagedResult<ProductDto>(
                dtos,
                products.TotalCount,
                query.Paging.Page,
                query.Paging.PageSize
            );
        }
    }

    #endregion

    #region Builder Pattern for Complex Records

    /// <summary>
    /// Builder class for creating complex queries fluently.
    /// Demonstrates how to make complex record creation more ergonomic.
    /// </summary>
    public class SearchProductsQueryBuilder
    {
        private string? _keyword;
        private decimal? _priceMin;
        private decimal? _priceMax;
        private string? _category;
        private bool _onlyInStock;
        private int _page = 1;
        private int _pageSize = 20;
        private string _sortBy = "Name";
        private SortDirection _sortDirection = SortDirection.Ascending;

        public SearchProductsQueryBuilder WithKeyword(string keyword)
        {
            _keyword = keyword;
            return this;
        }

        public SearchProductsQueryBuilder WithPriceRange(decimal min, decimal max)
        {
            _priceMin = min;
            _priceMax = max;
            return this;
        }

        public SearchProductsQueryBuilder WithCategory(string category)
        {
            _category = category;
            return this;
        }

        public SearchProductsQueryBuilder OnlyInStock(bool inStock = true)
        {
            _onlyInStock = inStock;
            return this;
        }

        public SearchProductsQueryBuilder WithPaging(int page, int pageSize)
        {
            _page = page;
            _pageSize = pageSize;
            return this;
        }

        public SearchProductsQueryBuilder WithSorting(string sortBy, SortDirection direction = SortDirection.Ascending)
        {
            _sortBy = sortBy;
            _sortDirection = direction;
            return this;
        }

        public SearchProductsQuery Build()
        {
            var priceRange = _priceMin.HasValue && _priceMax.HasValue
                ? new PriceRange(_priceMin.Value, _priceMax.Value)
                : null;

            return new SearchProductsQuery(
                new SearchCriteria(_keyword, priceRange, _category, _onlyInStock),
                new PagingOptions(_page, _pageSize),
                new SortingOptions(_sortBy, _sortDirection)
            );
        }
    }

    #endregion

    #region Record Inheritance Hierarchy

    /// <summary>
    /// Base record for all audit-tracked commands.
    /// Demonstrates record inheritance.
    /// </summary>
    public abstract record AuditedCommand<TResult> : IRequest<TResult>
    {
        public string PerformedBy { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Specific command inheriting from base record.
    /// </summary>
    public record UpdateProductPriceCommand(
        int ProductId,
        decimal NewPrice,
        string Reason
    ) : AuditedCommand<bool>;

    /// <summary>
    /// Another command inheriting from base record.
    /// </summary>
    public record DeactivateProductCommand(
        int ProductId,
        string Reason
    ) : AuditedCommand<bool>;

    #endregion

    #region Supporting Types

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool InStock { get; set; }
    }

    public class ProductSearchResult
    {
        public List<Product> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public interface IProductRepository
    {
        Task<ProductSearchResult> SearchAsync(
            string? keyword,
            decimal? priceMin,
            decimal? priceMax,
            string? category,
            bool onlyInStock,
            string sortBy,
            SortDirection sortDirection,
            int page,
            int pageSize
        );
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
        /// Example 1: Complex query construction using nested records.
        /// </summary>
        public async Task<PagedResult<ProductDto>> SearchProductsExample1()
        {
            var query = new SearchProductsQuery(
                Criteria: new SearchCriteria(
                    Keyword: "laptop",
                    Price: new PriceRange(500, 2000),
                    Category: "Electronics",
                    OnlyInStock: true
                ),
                Paging: new PagingOptions(Page: 1, PageSize: 20),
                Sorting: new SortingOptions("Price", SortDirection.Ascending)
            );

            return await _mediator.MediateAsync<PagedResult<ProductDto>>(query);
        }

        /// <summary>
        /// Example 2: Using builder pattern for complex query.
        /// </summary>
        public async Task<PagedResult<ProductDto>> SearchProductsExample2()
        {
            var query = new SearchProductsQueryBuilder()
                .WithKeyword("laptop")
                .WithPriceRange(500, 2000)
                .WithCategory("Electronics")
                .OnlyInStock()
                .WithPaging(1, 20)
                .WithSorting("Price", SortDirection.Ascending)
                .Build();

            return await _mediator.MediateAsync<PagedResult<ProductDto>>(query);
        }

        /// <summary>
        /// Example 3: Modifying query using 'with' expression.
        /// </summary>
        public async Task DemonstrateQueryModification()
        {
            var baseQuery = new SearchProductsQuery(
                new SearchCriteria("laptop", null, null, false),
                PagingOptions.Default,
                SortingOptions.Default
            );

            // Navigate to next page
            var nextPageQuery = baseQuery with
            {
                Paging = baseQuery.Paging with { Page = 2 }
            };

            // Change sorting
            var sortedByPriceQuery = baseQuery with
            {
                Sorting = new SortingOptions("Price", SortDirection.Descending)
            };

            // Multiple modifications
            var refinedQuery = baseQuery with
            {
                Criteria = baseQuery.Criteria with
                {
                    Category = "Electronics",
                    OnlyInStock = true
                },
                Paging = new PagingOptions(1, 50)
            };
        }

        /// <summary>
        /// Example 4: Using inherited record commands.
        /// </summary>
        public async Task AuditedCommandExample()
        {
            var command = new UpdateProductPriceCommand(
                ProductId: 123,
                NewPrice: 199.99m,
                Reason: "Promotion"
            )
            {
                PerformedBy = "admin@example.com",
                Timestamp = DateTime.UtcNow
            };

            var success = await _mediator.MediateAsync<bool>(command);
        }

        /// <summary>
        /// Example 5: Working with paged results.
        /// </summary>
        public async Task PaginationExample()
        {
            var query = new SearchProductsQuery(
                new SearchCriteria("gaming", null, "Electronics", false),
                new PagingOptions(1, 10),
                SortingOptions.Default
            );

            var result = await _mediator.MediateAsync<PagedResult<ProductDto>>(query);

            Console.WriteLine($"Page {result.Page} of {result.TotalPages}");
            Console.WriteLine($"Total items: {result.TotalCount}");
            Console.WriteLine($"Has next: {result.HasNextPage}");

            // Load next page if available
            if (result.HasNextPage)
            {
                var nextPageQuery = query with
                {
                    Paging = query.Paging with { Page = result.Page + 1 }
                };

                var nextPage = await _mediator.MediateAsync<PagedResult<ProductDto>>(nextPageQuery);
            }
        }
    }

    #endregion
}
