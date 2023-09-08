using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace backend.Model.Rest;

public class UpdateKPIsOfGraphicalItemSubmission
{
    [JsonProperty("kpis")]
    public List<Guid> KPIs { get; set; } = new List<Guid>();
}