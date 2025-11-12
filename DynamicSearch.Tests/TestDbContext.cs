using Microsoft.EntityFrameworkCore;

namespace DynamicSearch.Tests;

public class TestDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}
