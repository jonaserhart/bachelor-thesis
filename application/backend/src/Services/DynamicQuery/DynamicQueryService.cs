using backend.Model.Analysis.Queries;
using backend.Model.Exceptions;

namespace backend.Services.DynamicQuery;

public class DynamicQueryService : IDynamicQueryService
{
    private readonly HashSet<BaseQuerySchema> m_schemas;

    public DynamicQueryService()
    {
        m_schemas = new HashSet<BaseQuerySchema>();
    }

    public BaseQuerySchema GetByIdOrThrow(Guid id)
    {
        var schema = m_schemas.FirstOrDefault(x => x.Id == id);
        if (schema == null)
            throw new DbKeyNotFoundException(id, typeof(BaseQuerySchema));

        return schema;
    }

    public List<BaseQuerySchema> GetSchemas() => m_schemas.ToList();

    public void RegisterQuerySchema<T>(T schema) where T : BaseQuerySchema
    {
        m_schemas.Add(schema);
    }
}