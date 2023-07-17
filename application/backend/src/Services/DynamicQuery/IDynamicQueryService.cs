using backend.Model.Analysis.Queries;

namespace backend.Services.DynamicQuery;

public interface IDynamicQueryService
{
    public void RegisterQuerySchema<T>(T schema) where T : BaseQuerySchema;
    public List<BaseQuerySchema> GetSchemas();
    public BaseQuerySchema GetByIdOrThrow(Guid id);
}