using backend.Model.Enum;

namespace backend.Model.Analysis;

public class Query
{
    public Query(string id, string name, QueryReturnType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public string Id { get; }
    public string Name { get; }
    public QueryReturnType Type { get; }

    public override bool Equals(object? obj) => obj is Query query && Id.Equals(query.Id);
    public override int GetHashCode() => Id.GetHashCode();
}