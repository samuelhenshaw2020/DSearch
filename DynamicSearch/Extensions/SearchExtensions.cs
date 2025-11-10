using System.Linq.Expressions;

namespace DSearch;

/// <summary>
/// 
/// </summary>
public static class SearchExtensions
{
    // public static AbstractSearch<TSource> AddFields<TSource>(this AbstractSearch<TSource> source, Expression<Func<TSource, string?>> expression)
    //     where TSource :  class
    // {
    //     return source;
    // }
    

    public static AbstractSearch SetFields(this AbstractSearch source, HashSet<string> fields)
    {
        source.Fields = fields;
        return source;
    }
    
    // public static AbstractSearch<TSource> AddAdvanceFilter<TSource>(this AbstractSearch<TSource> source, Expression<Func<TSource, List<SearchField>>> expression)
    //     where TSource :  class
    // {
    //     return source;
    // }
    
    public static AbstractSearch SetKeywordSearchLogic(this AbstractSearch source, SearchLogic logic)
    {
        source.Logic = logic;
        return source;
    }

    public static AbstractSearch<TSource> SetKeywordSearchLogic<TSource>(this AbstractSearch<TSource> source, SearchLogic logic)
        where TSource :  class
    {
        source.Logic = logic;
        return source;
    }
    
    public static AbstractSearch SetKeyword(this AbstractSearch source, string keyword)
    {
        source.Keyword = keyword;
        return source;
    }

    public static AbstractSearch<TSource> SetKeyword<TSource>(this AbstractSearch<TSource> source, string keyword)
        where TSource :  class
    {
        source.Keyword = keyword;
        return source;
    }
}