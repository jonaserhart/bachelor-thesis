using backend.Model.Analysis.Queries;
using backend.Services.DynamicQuery;

namespace backend.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        public static WebApplicationBuilder UseQuerySchemas(this WebApplicationBuilder builder, params BaseQuerySchema[] querySchemas)
        {
            builder.Services.AddSingleton<IDynamicQueryService>((x) =>
            {
                var service = new DynamicQueryService();
                foreach (var querySchema in querySchemas)
                {
                    service.RegisterQuerySchema(querySchema);
                }
                return service;
            });

            return builder;
        }

    }
}