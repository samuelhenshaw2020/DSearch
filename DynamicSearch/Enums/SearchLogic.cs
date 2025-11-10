namespace DSearch;

/// <summary>
/// Defines the logical operators for combining multiple search conditions.
/// </summary>
public enum SearchLogic
{
    /// <summary>
    /// At least one of the search conditions must be true (logical OR).
    /// </summary>
    Or,
    
    /// <summary>
    /// All search conditions must be true (logical AND).
    /// </summary>
    And
}