namespace backend.Test.Helpers.Exceptions;

[System.Serializable]
public class TestSetupException : Exception
{
    public TestSetupException() { }
    public TestSetupException(string message) : base(message) { }
    public TestSetupException(string message, System.Exception inner) : base(message, inner) { }
    protected TestSetupException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}