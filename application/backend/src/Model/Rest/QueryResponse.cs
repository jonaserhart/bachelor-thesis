using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace backend.Model.Rest;

public class QueryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool? HasChildren { get; set; }
    public IEnumerable<QueryResponse> Children { get; set; } = new List<QueryResponse>(); 

    public static QueryResponse From(QueryHierarchyItem item)
    {
        var qr = new QueryResponse
        {
            Id = item.Id,
            Name = item.Name,
            HasChildren = item.HasChildren,
        };

        if (item.Children != null && item.Children.Any())
        {
            qr.Children = item.Children.Where((x) => x != null).Select(QueryResponse.From);
        }
        return qr;
    }
}