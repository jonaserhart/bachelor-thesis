using System.Runtime.Serialization;

namespace backend.Model.Exceptions;

[Serializable]
public class DbKeyNotFoundException : Exception
{
    public DbKeyNotFoundException()
    {
    }

    public DbKeyNotFoundException(object key, Type entity): base($"Key {key} was not found in table for entity {entity.Name}")
    {
        
    }

    public DbKeyNotFoundException(object key, Type entity, Exception? innerException): base($"Key {key} was not found in table for entity {entity.Name}", innerException)
    {
        
    }

    protected DbKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}