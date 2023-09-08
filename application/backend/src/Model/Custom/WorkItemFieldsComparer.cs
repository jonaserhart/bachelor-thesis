namespace backend.Model.Custom;

public class WorkItemFieldsComparer : IEqualityComparer<Dictionary<string, object>>
{
    public bool Equals(Dictionary<string, object>? x, Dictionary<string, object>? y)
    {
        if (x == null)
        {
            return y == null;
        }
        if (y == null)
        {
            return x == null;
        }

        if (x.TryGetValue("System.Id", out var idX) && y.TryGetValue("System.Id", out var idY))
        {
            return idX == idY;
        }

        return x.Count == y.Count && !x.Except(y).Any();
    }

    public int GetHashCode(Dictionary<string, object> obj)
    {
        int hash = 0;
        if (obj.TryGetValue("System.Id", out var idX))
        {
            hash ^= idX.GetHashCode();
            return hash;
        }

        foreach (var pair in obj)
        {
            hash ^= pair.Key.GetHashCode();
            if (pair.Value != null)
                hash ^= pair.Value.GetHashCode();
        }
        return hash;
    }
}