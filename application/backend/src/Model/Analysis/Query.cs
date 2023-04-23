using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace backend.Model.Analysis;

public class Query
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [JsonIgnore]
    public AnalysisModel? Model { get; set; }
    public IEnumerable<FieldInfo> FieldInfos { get; set; } = new List<FieldInfo>();
    public IEnumerable<string> Select { get; set; } = new List<string>();
    public IEnumerable<string> Where { get; set; } = new List<string>();

    public string ToQuery(string iterationPath, string? asOfDateTime = null)
    {
        if (Model == null)
            throw new ArgumentException("Model cannot be 'null' when creating a query");
        var select = string.Join(",\n", this.Select);
        var from = "workitems";
        var where = $"[System.TeamProject] = {Model.ProjectId}\nAND System.IterationPath = '{iterationPath}'";
        if (this.Where.Count() > 0)
        {
            where = $"{where} AND ({string.Join('\n', this.Where)})";
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