namespace DSearch;

/// <summary>
/// Abstract base class for search filters providing keyword search and advanced filtering capabilities.
/// </summary>
public abstract class AbstractSearch
{
    /// <summary>
    /// Gets or sets the keyword to search for across multiple fields.
    /// </summary>
    public string? Keyword { get; set; }
    
    /// <summary>
    /// Gets or sets the logical operator for combining multiple keyword search conditions. Default is Or.
    /// </summary>
    public SearchLogic Logic { get; set; } = SearchLogic.Or;
    
    /// <summary>
    /// Gets or sets the collection of field names to search within. Supports nested properties using dot notation (e.g., "Category.Name").
    /// </summary>
    public virtual HashSet<string> Fields { get; set; } = new(capacity: DynamicSearchOption.KeywordSearchFieldCapacity);
    
    /// <summary>
    /// Gets or sets the collection of advanced search filters for more complex queries.
    /// </summary>
    public List<SearchField>? AdvanceFilters { get; set; }
}

/// <summary>
/// Generic abstract base class for strongly-typed search filters.
/// </summary>
/// <typeparam name="TSource">The entity type to search against.</typeparam>
public abstract class AbstractSearch<TSource> : AbstractSearch
    where TSource : class;

