namespace backend.Model.Exceptions;

[System.Serializable]
public class ExpressionEvaluationException : System.Exception
{
    public ExpressionEvaluationException() { }
    public ExpressionEvaluationException(string message) : base(message) { }
    public ExpressionEvaluationException(string message, System.Exception inner) : base(message, inner) { }
    protected ExpressionEvaluationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}