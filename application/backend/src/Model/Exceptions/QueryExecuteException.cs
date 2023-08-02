namespace backend.Model.Exceptions;

[Serializable]
public class QueryExecuteException : Exception
{
    public QueryExecuteException() { }
    public QueryExecuteException(string message) : base(message) { }
    public QueryExecuteException(string message, Exception inner) : base(message, inner) { }
    protected QueryExecuteException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}