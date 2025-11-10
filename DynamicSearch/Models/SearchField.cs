namespace DSearch;

/// <summary>
/// Represents an advanced search field with a property, value, operation, and condition.
/// </summary>
public class SearchField
{
    /// <summary>
    /// Gets or initializes the property name to search on. Supports nested properties using dot notation (e.g., "Category.Name").
    /// </summary>
    public string? Property { get; init; }
    
    /// <summary>
    /// Gets or initializes the value to compare against.
    /// </summary>
    public object? Value { get; init; }
    
    /// <summary>
    /// Gets or initializes the search operation to perform.
    /// </summary>
    public SearchOperations Operation { get; init; }
    
    /// <summary>
    /// Gets or initializes a value indicating whether this search field should be applied. Default is true.
    /// </summary>
    public bool Condition { get; init; } = true;
}