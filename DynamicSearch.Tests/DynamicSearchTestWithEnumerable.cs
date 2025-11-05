using System.Collections;
using System.Linq.Expressions;
using DynamicSearch.Net;

namespace DynamicSearch.Tests;

public class TestData 
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Score { get; set; }
}

public class QueryableExtensionTestWithEnumerableLinq
{
    private static readonly IEnumerable<TestData> _enumerable = Enumerable
        .Range(1, 50)
        .Select(x => new TestData()
    {
        Name = "Name" + x,
        Description = "Description" + x,
        Score = Random.Shared.Next(1111, 99999)
    });
    
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void DynamicSearch_IsNotEmpty(int val)
    {
        var filter = new SearchFilter()
        {
            Keyword = "Name" + val,
            Fields = [nameof(TestData.Name)],
        };
        var result = _enumerable.AsQueryable()
            .SearchDynamic(filter).ToList();
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public void DynamicSearch_IsEmpty()
    {
        var filter = new SearchFilter()
        {
            Keyword = "Name" + 100,
            Fields = [nameof(TestData.Name)],
        };
        var result = _enumerable.AsQueryable().SearchDynamic(filter).ToList();
        Assert.Empty(result);
    }
    
    [Fact]
    public void DynamicSearch_AdvancedSearch()
    {
        var filter = new SearchFilter()
        {
            AdvanceFilters =
            [
                new SearchField()
                    { Property = nameof(TestData.Score), Operation = SearchOperations.LessThan, Value = 2 }
            ]
        };
        var result = _enumerable
            .AsQueryable()
            .SearchDynamic(filter).ToList();
        Assert.NotEmpty(result);
    }
    
 
}