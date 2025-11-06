using System.Linq.Expressions;

namespace DynamicSearch.Net;

public static class SearchExtensions
{
    public static AbstractSearch<TSource> AddFields<TSource>(this AbstractSearch<TSource> source, Expression<Func<TSource, object?>> expression)
        where TSource :  class
    {
        return source;
    }
    
    public static AbstractSearch SetFields(this AbstractSearch source, HashSet<string> fields)
    {
        source.Fields = fields;
        return source;
    }

    public static AbstractSearch<TSource> AddAdvanceFilter<TSource>(this AbstractSearch<TSource> source, Expression<Func<TSource, List<SearchField>>> expression)
        where TSource :  class
    {
        return source;
    }

    public static AbstractSearch SetDefaultLogic(this AbstractSearch source, SearchLogic logic)
    {
        source.Logic = logic;
        return source;
    }
    
    public static AbstractSearch SetKeyword(this AbstractSearch source, string keyword)
    {
        source.Keyword = keyword;
        return source;
    }
}