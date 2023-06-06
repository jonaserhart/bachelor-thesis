namespace backend.Model.Exceptions;

[System.Serializable]
public class ExpressionSaveException : Exception
{
    public ExpressionSaveException() { }
    public ExpressionSaveException(string message) : base(message) { }
    public ExpressionSaveException(string message, System.Exception inner) : base(message, inner) { }
    protected ExpressionSaveException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}