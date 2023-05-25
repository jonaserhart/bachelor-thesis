using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Model.Enum;
using Newtonsoft.Json;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace backend.Model.Analysis;

public class Clause
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    [JsonIgnore]
    public Query? Query { get; set; }
    [JsonIgnore]
    public Guid? QueryId { get; set; }
    [JsonIgnore]
    public Clause? ParentClause { get; set; }
    [JsonIgnore]
    public Guid? ParentClauseId { get; set; }
    public ICollection<Clause> Clauses { get; set; } = new List<Clause>();
    public string Field { get; set; } = string.Empty;
    public bool IsFieldValue { get; set; }
    public string FieldValue { get; set; } = string.Empty;
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.None;
    public FieldOperation? Operator { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsValidCondition =>
            !(this.Field != null)
            && (!string.IsNullOrEmpty(Value) || this.IsFieldValue && this.FieldValue != null)
            && Clauses.All(x => x.IsValidCondition);

    public static Clause From(WorkItemQueryClause clause)
    {
        var c = new Clause
        {
            LogicalOperator = clause.LogicalOperator switch
            {
                WorkItemQueryClause.LogicalOperation.NONE => LogicalOperator.None,
                WorkItemQueryClause.LogicalOperation.AND => LogicalOperator.And,
                WorkItemQueryClause.LogicalOperation.OR => LogicalOperator.Or,
                _ => LogicalOperator.None
            },
            IsFieldValue = clause.IsFieldValue.GetValueOrDefault(false),
            Operator = FieldOperation.From(clause.Operator),
            Value = clause.Value ?? string.Empty,
            Field = clause.Field?.ReferenceName ?? string.Empty,
            FieldValue = clause.FieldValue?.ReferenceName ?? string.Empty,
        };
        c.Clauses = clause.Clauses?.Where(x => x != null).Select(y =>
        {
            var newClause = Clause.From(y);
            newClause.ParentClause = c;
            return newClause;
        }).ToList() ?? new List<Clause>();
        return c;
    }

    public override string ToString()
    {
        if (this.LogicalOperator != LogicalOperator.None)
        {
            var childrenString = new StringBuilder();

            foreach (var childClause in this.Clauses)
            {
                childrenString.Append(childClause.ToString()).Append(" ");
            }

            return $"{this.LogicalOperator} ({childrenString})";
        }
        if (this.Field == null || this.Operator == null)
            return string.Empty;

        if (this.IsFieldValue)
        {
            if (string.IsNullOrEmpty(this.FieldValue))
                return string.Empty;

            return $"{this.Field} {this.Operator.Name} {this.FieldValue}";
        }

        if (string.IsNullOrEmpty(this.Value))
            return string.Empty;

        return $"{this.Field} {this.Operator.Name} {this.Value}";
    }
}