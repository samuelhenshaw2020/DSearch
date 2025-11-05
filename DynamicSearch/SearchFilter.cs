namespace DynamicSearch.Net;

public enum SearchOperations
{
    Contains,
    Equals,
    StartsWith,
    EndsWith,
    GreaterThan,
    GreaterThanOrEqualTo,
    LessThan,
    LessThanOrEqualTo,
}

public class SearchField
{
    public string? Property { get; init; }
    public object? Value { get; init; }
    public SearchOperations Operation { get; init; }
    public bool Condition { get; init; } = true;
}

public enum KeywordSearchLogic
{
    Or,
    And
}

public class SearchFilter
{
    public string? Keyword { get; set; }
    public KeywordSearchLogic Logic { get; set; } = KeywordSearchLogic.And;
    public HashSet<string> Fields { get; set; } = new(capacity: DynamicSearchOption.KeywordSearchFieldCapacity);
    public List<SearchField>? AdvanceFilters { get; set; } 
}