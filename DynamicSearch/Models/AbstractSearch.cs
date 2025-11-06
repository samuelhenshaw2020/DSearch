namespace DynamicSearch.Net;

public abstract class AbstractSearch
{
    public string? Keyword { get; set; }
    public SearchLogic Logic { get; set; } = SearchLogic.Or;
    public virtual HashSet<string> Fields { get; set; } = new(capacity: DynamicSearchOption.KeywordSearchFieldCapacity);
    public List<SearchField>? AdvanceFilters { get; set; }
}

public abstract class AbstractSearch<TSource> : AbstractSearch
    where TSource : class;

