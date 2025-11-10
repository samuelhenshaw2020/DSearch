using DSearch;
using Microsoft.EntityFrameworkCore;

namespace DynamicSearch.Tests;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    
    // Navigation property
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}

public class ProductSearchFilter : AbstractSearch;

public class TestDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}



public class DynamicSearchTestWithDbContext : IDisposable
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
    
    [Fact]
    public void ShouldSearchWithKeywordFilter_AndReturnList()
    {
        var filter = new ProductSearchFilter();

        filter
            .SetKeyword("Product 1")
            .SetFields(["Category.Name", nameof(Product.Name)]);
        
        var result = _dbContext.Products
            .DynamicSearch(filter)
            .ToList();
        
        Assert.True(result.Count > 0);
    }

    [Fact]
    public void ShouldReturnAllProducts()
    {
        var result = _dbContext.Products.ToList();
        Assert.True(result.Count > 0);
    }

   
    
    [Fact]
    public void ShouldSearchMultipleFields_AndReturnList()
    {
        var filter = new ProductSearchFilter()
        {
            Keyword = "Clothing",
            Fields = [nameof(Product.CategoryName), nameof(Product.Name)],
            Logic = SearchLogic.Or
        };
        var result = _dbContext.Products
            .DynamicSearch(filter)
            .ToList();
        
        Assert.True(result.Count > 0);
    }
    
    [Fact]
    public void ShouldSearchNavigationProperty_AndReturnList()
    {
        var filter = new ProductSearchFilter()
        {
            Keyword = "Electronics",
            Fields = ["Category.Name"],
            Logic = SearchLogic.Or
        };
        
        var result = _dbContext.Products
            .Include(p => p.Category) // Include navigation property
            .DynamicSearch(filter)
            .ToList();
        
        Assert.True(result.Count > 0);
        Assert.All(result, p => Assert.Equal("Electronics", p.Category!.Name));
    }
    
    [Fact]
    public void ShouldSearchMultipleFieldsIncludingNavigationProperty_AndReturnList()
    {
        var filter = new ProductSearchFilter()
        {
            Keyword = "Books",
            Fields = [nameof(Product.Name), nameof(Product.CategoryName), "Category.Name", "Category.Description"],
            Logic = SearchLogic.Or
        };
        
        var result = _dbContext.Products
            .Include(p => p.Category)
            .DynamicSearch(filter)
            .ToList();
        
        Assert.True(result.Count > 0);
    }

    [Fact]
    public void ShouldSearchWithAdvanceFilter_AndReturnList()
    {
        var filter = new ProductSearchFilter()
        {
            AdvanceFilters =
            [
                new SearchField()
                    { Property = nameof(Product.Name), Operation = SearchOperations.Equals, Value = "Product 1" }
            ]
        };
        var result = _dbContext.Products
            .Include(p => p.Category)
            .DynamicSearch(filter)
            .ToList();
        Assert.True(result.Count > 0);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}

