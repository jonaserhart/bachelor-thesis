using System.Runtime.Serialization;

namespace backend.Model.Rest;

[DataContract]
public class ApiError
{
    [DataMember(Name = "message")]
    public string Message { get; set; } = string.Empty;
}