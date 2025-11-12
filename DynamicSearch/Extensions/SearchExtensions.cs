using System.Linq.Expressions;

namespace DSearch;

/// <summary>
/// 
/// </summary>
public static class SearchExtensions
{
    public static IAbstractSearch SetFields(this IAbstractSearch source, HashSet<string> fields)
    {
        source.Fields = fields;
        return (AbstractSearch)source;
    }
    
    public static AbstractSearch SetKeywordSearchLogic(this IAbstractSearch source, SearchLogic logic)
    {
        source.Logic = logic;
        return (AbstractSearch)source;
    }

    public static AbstractSearch<TSource> SetKeywordSearchLogic<TSource>(this IAbstractSearch<TSource> source, SearchLogic logic)
        where TSource :  class
    {
        source.Logic = logic;
        return (AbstractSearch<TSource>)source;
    }
    
    public static AbstractSearch SetKeyword(this IAbstractSearch source, string keyword)
    {
        source.Keyword = keyword;
        return (AbstractSearch)source;
    }

    public static AbstractSearch<TSource> SetKeyword<TSource>(this AbstractSearch<TSource> source, string keyword)
        where TSource :  class
    {
        source.Keyword = keyword;
        return source;
    }
}