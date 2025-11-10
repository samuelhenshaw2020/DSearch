namespace DSearch;

/// <summary>
/// Defines the available search operations for filtering data.
/// </summary>
public enum SearchOperations
{
    /// <summary>
    /// Checks if a string property contains the specified value (case-insensitive).
    /// </summary>
    Contains,
    
    /// <summary>
    /// Checks if a property equals the specified value (case-insensitive for strings).
    /// </summary>
    Equals,
    
    /// <summary>
    /// Checks if a string property starts with the specified value.
    /// </summary>
    StartsWith,
    
    /// <summary>
    /// Checks if a string property ends with the specified value.
    /// </summary>
    EndsWith,
    
    /// <summary>
    /// Checks if a numeric or DateTime property is greater than the specified value.
    /// </summary>
    GreaterThan,
    
    /// <summary>
    /// Checks if a numeric or DateTime property is greater than or equal to the specified value.
    /// </summary>
    GreaterThanOrEqualTo,
    
    /// <summary>
    /// Checks if a numeric or DateTime property is less than the specified value.
    /// </summary>
    LessThan,
    
    /// <summary>
    /// Checks if a numeric or DateTime property is less than or equal to the specified value.
    /// </summary>
    LessThanOrEqualTo,
}