using System.Linq.Expressions;
using System.Reflection;

namespace DSearch;


public static class QueryableExtensions
{
    public static IQueryable<TSource> DynamicSearch<TSource>(this IQueryable<TSource> queryable, IAbstractSearch filter)
        
    {
        if (filter == null) return queryable;
        ParameterExpression parameter = Expression.Parameter(typeof(TSource), "x");
        var properties = typeof(TSource).GetProperties();
        
        Expression? predicate = null;

        if (!string.IsNullOrEmpty(filter.Keyword))
        {
            var pre = KeywordSearch(filter, properties, parameter);
            if (pre != null) predicate = pre;
        }

        if (filter.AdvanceFilters is { Count: > 0 } advanceFilters)
        {
            var pre = AdvanceFilterSearch(advanceFilters, properties, parameter);
            if (pre != null)
                predicate = predicate == null
                    ? pre
                    : Expression.AndAlso(predicate, pre);
        }

        if (predicate == null)
            return queryable;

        Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(predicate, parameter);
        return queryable
            .Where(lambda) ;
    }

    private static Expression? AdvanceFilterSearch(List<SearchField> advancedSearchFields, PropertyInfo[] properties, Expression parameter)
    {
        if(advancedSearchFields.Count > DynamicSearchOption.AdvanceSearchFilterCapacity) 
            throw new IndexOutOfRangeException("Advance search filters must be less than 10.");
        
        Expression? predicate = null;
        
        foreach (var field in advancedSearchFields)
        {
            if (field.Value == null || !field.Condition) continue;
            
            if (string.IsNullOrEmpty(field.Property))
                throw new Exception($"Property {field.Property} is empty or null");
            
            if (properties.All(x => x.Name != field.Property))
                throw new Exception($"Property {field.Property} not found");

            MemberExpression property = Expression.Property(parameter, field.Property);

            ConvertPropertyToType(property.Type, field.Value, out var keywordExpresison);

            var operationExpression = ParseOperationExpression(property, keywordExpresison, field.Operation);
            predicate = predicate == null
                ? operationExpression
                : Expression.AndAlso(predicate, operationExpression);
        }
        return predicate;
    }

    private static Expression? KeywordSearch(IAbstractSearch filter, PropertyInfo[] properties, Expression parameter)
    {
        var fields = filter.Fields;
        var keyword =  filter.Keyword;
        var logic = filter.Logic;
        
        if(fields.Count > DynamicSearchOption.KeywordSearchFieldCapacity) 
            throw new IndexOutOfRangeException("Keyword fields must be less than 10.");
        
        if(fields.Count <= 0)
            throw new InvalidDataException("Keyword fields must be greater than 0.");

        if (keyword == null)
            return null;
            
        Expression predicate = null!;
        foreach (var field in fields)
        {
            Expression property;
            if (field.Contains('.'))
            {
                property = BuildNestedPropertyAccess(parameter, field);
            }
            else
            {
                if (properties.All(x => x.Name != field))
                    throw new Exception($"Property {field} not found");
                property = Expression.Property(parameter, field);
            }
          
            var containExpr = BuildContainsExpression(property, keyword);
            
            predicate = predicate == null
                ? containExpr
                : HandleLogic(logic, predicate, containExpr);
        }

        return predicate;
    }

    private static Expression BuildNestedPropertyAccess(Expression parameter, string propertyPath)
    {
        var properties = propertyPath.Split('.');
        Expression expression = parameter;
        
        foreach (string prop in properties)
        {
            var propertyInfo = expression.Type.GetProperty(prop);
            if (propertyInfo == null)
                throw new Exception($"Property {prop} not found in {expression.Type.Name}");
            
            expression = Expression.Property(expression, propertyInfo);
        }
        
        return expression;
    }

    private static Expression BuildContainsExpression(Expression property, string keyword)
    {
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
        var lowCaseMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
        
        if (!property.Type.IsValueType && property.Type != typeof(string))
        {
            throw new Exception($"Property {property} must be a string type for keyword search");
        }
        
        var notNullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));
        var propertyLower = Expression.Call(property, lowCaseMethod!);
        var keywordLower = Expression.Call(Expression.Constant(keyword), lowCaseMethod!);
        var containExpr = Expression.Call(propertyLower, containsMethod!, keywordLower);
        
        return Expression.AndAlso(notNullCheck, containExpr);
    }

    private static Expression HandleLogic(SearchLogic logic, Expression predicate, Expression parameter)
    {
        return  logic switch
        {
            SearchLogic.And => Expression.AndAlso(predicate, parameter),
            SearchLogic.Or => Expression.OrElse(predicate, parameter),
            _ => throw new ArgumentOutOfRangeException(nameof(logic), logic, null)
        };
    }
    
    private static void ConvertPropertyToType(Type propertyType, object fieldValue, out Expression keyword)
    {
        if (propertyType == typeof(string))
        {
            keyword = Expression.Constant(fieldValue, typeof(string));
        }
        else if (propertyType == typeof(DateTime))
        {
            keyword = Expression.Convert(Expression.Constant(fieldValue), typeof(DateTime));
        }
        else if (propertyType == typeof(decimal))
        {
            var  number = decimal.Parse(fieldValue.ToString());
            keyword = Expression.Constant(number, typeof(decimal));
        }
        else if (propertyType == typeof(int))
        {
            var num = int.Parse(fieldValue.ToString());
            keyword = Expression.Constant(num, typeof(int));
        }
        else if(propertyType == typeof(bool))
        {
            var boolVal = bool.Parse(fieldValue.ToString());
            keyword = Expression.Constant(boolVal, typeof(bool));
        }
        else if(propertyType == typeof(Guid))
        {
            var guidVal = Guid.Parse(fieldValue.ToString());
            keyword = Expression.Constant(guidVal, typeof(Guid));
        }
        else
        {
            // TODO do we need to search through other data types
            keyword = Expression.Constant(fieldValue, propertyType);
        }
    }

    private static Expression ParseOperationExpression(Expression property, Expression keyword,
        SearchOperations operation)
    {
        switch (operation)
        {
            case SearchOperations.Contains:
                var lowCaseMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                var propertyLower = Expression.Call(property, lowCaseMethod!);
                var keywordLower = Expression.Call(keyword, lowCaseMethod!);
                var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
                return Expression.Call(propertyLower, containsMethod!, keywordLower); // TODO how to use StringCOmparison
            case SearchOperations.LessThan:
                return Expression.LessThan(property, keyword);
            case SearchOperations.GreaterThan:
                return Expression.GreaterThan(property, keyword);
            case SearchOperations.GreaterThanOrEqualTo:
                return Expression.GreaterThanOrEqual(property, keyword);
            case SearchOperations.LessThanOrEqualTo:
                return Expression.LessThanOrEqual(property, keyword);
            case SearchOperations.StartsWith:
                var startWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)]);
                return Expression.Call(property, startWithMethod!, keyword);
            case SearchOperations.EndsWith:
                var endWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)]);
                return Expression.Call(property, endWithMethod!, keyword);
            case SearchOperations.Equals:
                if (property.Type == typeof(string))
                {
                    lowCaseMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);
                    propertyLower = Expression.Call(property, lowCaseMethod!);
                    keywordLower = Expression.Call(keyword, lowCaseMethod!);
                    return Expression.Equal(propertyLower, keywordLower);
                }
                return Expression.Equal(property, keyword);

            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }
    }
}