using DSearch;
using Microsoft.EntityFrameworkCore;

namespace DynamicSearch.Tests;

public class PaginatedFilter
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public PaginatedFilter(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public int PageNumber { get; set; }
}

public class UserSearchFilter(int pageNumber, int pageSize) :  PaginatedFilter(pageNumber, pageSize), IAbstractSearch<Product>
{
    public string? Keyword { get; set; }
    public SearchLogic Logic { get; set; }
    public HashSet<string> Fields { get; set; }
    public List<SearchField>? AdvanceFilters { get; set; }
}

public class DynamicSearchWithPagination : IDisposable
{
    private readonly TestDbContext _dbContext;
    public DynamicSearchWithPagination()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TestDbContext(options);
        SeedDatabase();
    }
    
    private void SeedDatabase()
    {
        // Create categories
        var electronics = new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" };
        var clothing = new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel" };
        var books = new Category { Id = 3, Name = "Books", Description = "Books and literature" };
        
        _dbContext.Categories.AddRange(electronics, clothing, books);
        _dbContext.SaveChanges();
        
        var products = new List<Product>();
        for (int i = 1; i <= 100; i++)
        {
            var categoryId = i % 3 == 0 ? 1 : (i % 3 == 1 ? 2 : 3);
            products.Add(new Product
            {
                Id = i,
                Name = $"Product {i}",
                Price = i * 10m,
                CategoryName = i % 2 == 0 ? "Electronics" : "Clothing",
                CategoryId = categoryId,
                Category = categoryId == 1 ? electronics : (categoryId == 2 ? clothing : books)
            });
        }

        _dbContext.Products.AddRange(products);
        _dbContext.SaveChanges();
    }
    
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 10)]
    public async Task DynamicSearch_WithPagination_IsTypeOfPaginationResponse(int pageNumber, int pageSize)
    {
        var filter = new UserSearchFilter(pageNumber, pageSize)
        {
            Keyword = "Clothing",
            Fields = [nameof(Product.CategoryName), nameof(Product.Name)],
            Logic = SearchLogic.Or
        };

        var result = await _dbContext.Products
                .DynamicSearch(filter)
                .ToPaginatedList(filter.PageNumber, filter.PageSize);
        Assert.IsType<PaginatedResponse<Product>>(result);
    }
    
 
    [Theory]
    [InlineData(1, 10)]
    public async Task DynamicSearch_WithPagination_ResultMatchPageSize(int pageNumber, int pageSize)
    {
        var filter = new UserSearchFilter(pageNumber, pageSize)
        {
            Keyword = "Clothing",
            Fields = [nameof(Product.CategoryName), nameof(Product.Name)],
            Logic = SearchLogic.Or
        };

        var result = await _dbContext.Products
            .DynamicSearch(filter)
            .ToPaginatedList(filter.PageNumber, filter.PageSize);
        Assert.Equal(result.Data.Count, result.PageSize);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}

public class PaginatedResponse<T>(List<T> data, int pageNumber, int pageSize, int totalRecords)
    where T : class
{
    public List<T>? Data { get; set; } = data;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;
    public int TotalPages { get; } = (int)Math.Ceiling(totalRecords / (decimal)pageSize);
    public int TotalRecords { get; } = totalRecords;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public static class TestQueryExtensions
{
    public static async Task<PaginatedResponse<TSource>> ToPaginatedList<TSource>(
        this IQueryable<TSource> queryable, int pageNumber, int pageSize)
        where TSource : class
    {
        ArgumentNullException.ThrowIfNull(queryable);

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber),
                string.Format("{0} size must be greater than 0.", nameof(pageNumber)));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber),
                $"{nameof(pageSize)} size must be greater than 0.");

        var totalRecords = await queryable
            .CountAsync();

        List<TSource> data;
        if (totalRecords <= 0)
        {
            data = Enumerable.Empty<TSource>().ToList();
        }
        else
        {
            var result = await queryable
                .Skip((pageNumber - 1) * pageSize) // to begin from zero
                .Take(pageSize)
                .ToListAsync();

            data = result;
        }

        return new PaginatedResponse<TSource>(data, pageNumber, pageSize, totalRecords);
    }
}