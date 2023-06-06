using System.Data;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using backend.Model.Enum;

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
    public QueryExecuteTime QueryExecuteTime { get; set; } = QueryExecuteTime.SprintEnd;


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

    public string ToQuery(string iterationPath, DateTime? asOfDateTime = null)
    {
        var select = string.Join(",", this.Select.Select(x => x.ReferenceName));
        var from = "workitems";
        var where = $"System.IterationPath = '{iterationPath}'";
        if (this.Where != null)
        {
            where = $"({where}) AND ({this.Where.ToString()})";
        }
        var asOf = string.Empty;
        if (asOfDateTime != null)
        {
            asOf = $"ASOF '{asOfDateTime.ToString()}'";
        }

        return $"SELECT {select} FROM {from} WHERE {where} {asOf}";
    }
}