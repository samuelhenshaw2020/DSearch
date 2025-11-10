# DynamicSearch.Net

A powerful and flexible dynamic search library for .NET that enables keyword-based searching with support for navigation properties, advanced filtering, and multiple search operations.

[![NuGet](https://img.shields.io/nuget/v/DSearch.svg)](https://www.nuget.org/packages/DSearch/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

- ✨ **Keyword Search**: Search across multiple fields with a single keyword
- 🔗 **Navigation Properties**: Support for nested properties using dot notation (e.g., `"Category.Name"`)
- 🎯 **Advanced Filtering**: Multiple search operations (Contains, Equals, StartsWith, EndsWith, GreaterThan, LessThan, etc.)
- 🔀 **Flexible Logic**: AND/OR logic support for combining multiple search conditions
- 🛡️ **Null-Safe**: Automatic null-checking for navigation properties
- ⚡ **Efficient**: Uses Expression Trees for efficient LINQ queries
- 🔧 **Easy Integration**: Works seamlessly with Entity Framework Core and IQueryable

## Installation

```bash
dotnet add package DSearch
```

Or via NuGet Package Manager:

```
Install-Package DSearch
```

## Quick Start

### Basic Keyword Search

```csharp
using DSearch;

var filter = new SearchFilter
{
    Keyword = "electronics",
    Fields = ["Name", "Description"],
    Logic = KeywordSearchLogic.Or
};

var results = dbContext.Products
    .SearchDynamic(filter)
    .ToList();
```

### Navigation Property Search

Search through related entities using dot notation:

```csharp
var filter = new SearchFilter
{
    Keyword = "Electronics",
    Fields = ["Category.Name", "Supplier.CompanyName"],
    Logic = KeywordSearchLogic.Or
};

var results = dbContext.Products
    .Include(p => p.Category)      // Include navigation properties
    .Include(p => p.Supplier)
    .SearchDynamic(filter)
    .ToList();
```

### Advanced Filtering

Combine keyword search with advanced filters:

```csharp
var filter = new SearchFilter
{
    Keyword = "laptop",
    Fields = ["Name", "Category.Name"],
    Logic = KeywordSearchLogic.Or,
    AdvanceFilters = new List<SearchField>
    {
        new SearchField
        {
            Property = "Price",
            Value = 1000,
            Operation = SearchOperations.LessThan,
            Condition = true
        },
        new SearchField
        {
            Property = "InStock",
            Value = true,
            Operation = SearchOperations.Equals,
            Condition = true
        }
    }
};

var results = dbContext.Products
    .Include(p => p.Category)
    .SearchDynamic(filter)
    .ToList();
```

## Search Operations

The library supports the following search operations:

- `Contains` - Check if string contains value
- `Equals` - Exact match
- `StartsWith` - Check if string starts with value
- `EndsWith` - Check if string ends with value
- `GreaterThan` - Numeric/DateTime comparison
- `GreaterThanOrEqualTo` - Numeric/DateTime comparison
- `LessThan` - Numeric/DateTime comparison
- `LessThanOrEqualTo` - Numeric/DateTime comparison

## Examples

### Example 1: Simple Product Search

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Search products by name or category name
var filter = new SearchFilter
{
    Keyword = "laptop",
    Fields = ["Name", "Category.Name"],
    Logic = KeywordSearchLogic.Or
};

var products = await dbContext.Products
    .Include(p => p.Category)
    .SearchDynamic(filter)
    .ToListAsync();
```

### Example 2: Complex Search with Multiple Conditions

```csharp
var filter = new SearchFilter
{
    Keyword = "gaming",
    Fields = ["Name", "Description", "Category.Name"],
    Logic = KeywordSearchLogic.Or,
    AdvanceFilters = new List<SearchField>
    {
        new SearchField
        {
            Property = "Price",
            Value = 500,
            Operation = SearchOperations.GreaterThanOrEqualTo
        },
        new SearchField
        {
            Property = "Price",
            Value = 2000,
            Operation = SearchOperations.LessThanOrEqualTo
        },
        new SearchField
        {
            Property = "Category.Name",
            Value = "Electronics",
            Operation = SearchOperations.Equals
        }
    }
};

var results = await dbContext.Products
    .Include(p => p.Category)
    .SearchDynamic(filter)
    .ToListAsync();
```

## Best Practices

1. **Limit the number of fields** to search for better performance (default max: 10)
2. **Use appropriate search operations** - use `Equals` for exact matches, `Contains` for partial matches
3. **Combine keyword and advanced filters** for more precise results
4. **Consider indexing** frequently searched fields in your database

## Supported Types

The library supports searching and filtering on the following types:

- `string`
- `int`
- `decimal`
- `DateTime`
- `bool`
- `Guid`

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Changelog

### v1.0.0
- Initial release
- Keyword search across multiple fields
- Support for navigation properties with dot notation
- Advanced filtering with multiple operations
- AND/OR logic support
- Null-safe property access

## Support

If you encounter any issues or have questions, please file an issue on the [GitHub repository](https://github.com/samuelhenshaw2020/DSearch/issues).

