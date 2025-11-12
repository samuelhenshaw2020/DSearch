namespace DSearch;

/// <summary>
/// Abstract base class for search filters providing keyword search and advanced filtering capabilities.
/// </summary>
public interface IAbstractSearch
{
    /// <summary>
    /// Gets or sets the keyword to search for across multiple fields.
    /// </summary>
    public string? Keyword { get; set; }
    
    /// <summary>
    /// Gets or sets the logical operator for combining multiple keyword search conditions. Default is Or.
    /// </summary>
    public SearchLogic Logic { get; set; } 
    
    /// <summary>
    /// Gets or sets the collection of field names to search within. Supports nested properties using dot notation (e.g., "Category.Name").
    /// </summary>
    public HashSet<string> Fields { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of advanced search filters for more complex queries.
    /// </summary>
    public List<SearchField>? AdvanceFilters { get; set; }
}

/// <summary>
/// Abstract generic base class for search filters providing keyword search and advanced filtering capabilities.
/// </summary>
/// <typeparam name="TSource">The entity type to search against.</typeparam>
public interface IAbstractSearch<TSource> : IAbstractSearch; 