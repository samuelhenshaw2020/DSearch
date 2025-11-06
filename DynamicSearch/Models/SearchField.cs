namespace DynamicSearch.Net;

public class SearchField
{
    public string? Property { get; init; }
    public object? Value { get; init; }
    public SearchOperations Operation { get; init; }
    public bool Condition { get; init; } = true;
}