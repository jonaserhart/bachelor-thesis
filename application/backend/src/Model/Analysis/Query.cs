using System.Data;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace backend.Model.Analysis;

public class Query
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public Guid? ReferencedId { get; set; }
    public bool IsReferenced => ReferencedId == null;
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public AnalysisModel? Model { get; set; }
    public List<FieldInfo> Select { get; set; } = new List<FieldInfo>();
    public Clause? Where { get; set; }

    public static Query From(QueryHierarchyItem item)
    {
        var q = new Query
        {
            ReferencedId = item.Id,
            Name = item.Name,
        };
        if (item.Clauses != null)
        {
            q.Where = Clause.From(item.Clauses);
        }

        return q;
    }

    public string ToQuery(string iterationPath, string? asOfDateTime = null)
    {
        if (Model == null)
            throw new ArgumentException("Model cannot be 'null' when creating a query");
        var select = string.Join(",\n", this.Select.Select(x => x.ReferenceName));
        var from = "workitems";
        var where = $"[System.TeamProject] = {Model.ProjectId}\nAND System.IterationPath = '{iterationPath}'";
        if (this.Where != null)
        {
            where = $"({where}) AND ({this.Where.ToString()})";
        }
        var asOf = string.Empty;
        if (asOfDateTime != null)
        {
            asOf = $"ASOF '{asOfDateTime}'";
        }

        return $"""
        SELECT 
            {select}
        FROM {from}
        WHERE
            {where}
        {asOf}
        """;
    }
}