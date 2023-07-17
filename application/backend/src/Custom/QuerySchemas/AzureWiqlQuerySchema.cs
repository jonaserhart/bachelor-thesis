using backend.Model.Analysis.Queries;
using backend.Model.Enum;
using backend.Services.API;

namespace backend.Custom.QuerySchema;

public class AzureWiqlQuerySchema : IQuerySchema
{
    private readonly IApiClientFactory _factory;

    private readonly Guid _id;



    public AzureWiqlQuerySchema(IApiClientFactory clientFactory)
    {
        _factory = clientFactory;
        _id = Guid.NewGuid();
    }

    public Guid Id { get => _id; }
    public string Description { get => "Azure devops wiql selector query"; }
    public QueryReturnType ReturnType { get => QueryReturnType.WorkItemList; }

    public override Task<Query> CreateQueryAsync(List<QueryParameterValue> createQueryParametersAndValues)
    {

    }

    public override Task<object> ExecuteQueryAsync(Query query, List<QueryParameterValue> runtimeParameters)
    {

    }

    public override async Task<List<QueryParameter>> GetCreateQueryParametersAsync()
    {
        using var client = await _factory.GetApiClientAsync();
        var project = new QueryParameter("project", CreateQueryParameterType.SingleSelect);
        var available = await client.GetProjectsAsync();
        project.Data = available.Select(x => new { x.Id, x.Name }).ToArray();

        var queries = client.GetQueriesAsync()
    }

    public override Task<List<QueryParameter>> GetRuntimeParametersAsync(Query query)
    {

    }

}
