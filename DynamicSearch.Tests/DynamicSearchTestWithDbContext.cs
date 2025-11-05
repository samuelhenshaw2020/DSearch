using DynamicSearch.Net;
using Microsoft.EntityFrameworkCore;

namespace DynamicSearch.Tests;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

public class TestDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}

public class DynamicSearchTestWithDbContext :IDisposable
{
    private readonly TestDbContext _dbContext;
    public DynamicSearchTestWithDbContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TestDbContext(options);
        SeedDatabase();
    }
    
    private void SeedDatabase()
    {
        var products = new List<Product>();
        for (int i = 1; i <= 100; i++)
        {
            products.Add(new Product
            {
                Id = i,
                Name = $"Product {i}",
                Price = i * 10m,
                Category = i % 2 == 0 ? "Electronics" : "Clothing"
            });
        }

        _dbContext.Products.AddRange(products);
        _dbContext.SaveChanges();
    }

    [Fact]
    public void ShouldReturnAllProducts()
    {
        var result = _dbContext.Products.ToList();
        Assert.True(result.Count > 0);
    }

    [Fact]
    public void ShouldSearchWithKeywordFilter_AndReturnList()
    {
        var filter = new SearchFilter()
        {
            Keyword = "Product 1",
            Fields = [nameof(Product.Name)],
        };
        var result = _dbContext.Products
            .SearchDynamic(filter)
            .ToList();
        
        Assert.True(result.Count > 0);
    }
    
    [Fact]
    public void ShouldSearchMultipleFields_AndReturnList()
    {
        var filter = new SearchFilter()
        {
            Keyword = "Clothing",
            Fields = [nameof(Product.Category), nameof(Product.Name)],
            Logic = KeywordSearchLogic.Or
        };
        var result = _dbContext.Products
            .SearchDynamic(filter)
            .ToList();
        
        Assert.True(result.Count > 0);
    }


    public void Dispose()
    {
        _dbContext.Dispose();
    }
}