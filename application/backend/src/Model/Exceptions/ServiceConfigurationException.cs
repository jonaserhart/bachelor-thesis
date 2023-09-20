namespace backend.Model.Exceptions;

[Serializable]
public class ServiceConfigurationException : Exception
{
    public ServiceConfigurationException() { }
    public ServiceConfigurationException(string message) : base(message) { }
    public ServiceConfigurationException(string message, Exception inner) : base(message, inner) { }
    protected ServiceConfigurationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}