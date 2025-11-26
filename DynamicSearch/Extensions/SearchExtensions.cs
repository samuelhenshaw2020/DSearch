using System.Linq.Expressions;

namespace DSearch;

/// <summary>
/// 
/// </summary>
public static class SearchExtensions
{
    public static T SetFields<T>(this T source, HashSet<string> fields)
        where T : IAbstractSearch
    {
        source.Fields = fields;
        return source;
    }
    
    public static T SetLogic<T>(this T source, SearchLogic logic)
        where T : IAbstractSearch
    {
        source.Logic = logic;
        return source;
    }
    
    public static T SetKeyword<T>(this T source, string keyword)
        where T : IAbstractSearch
    {
        source.Keyword = keyword;
        return source;
    }
}