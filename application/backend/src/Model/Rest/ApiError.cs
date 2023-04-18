using System.Runtime.Serialization;

namespace backend.Model.Rest;

[DataContract]
public class ApiError
{
    [DataMember(Name = "error")]
    public string Error { get; set; } = string.Empty;
}