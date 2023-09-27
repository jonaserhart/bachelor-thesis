
using backend.Services.DevOps;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace backend.HealthChecks;

public class HealthCheck : IHealthCheck
{
    private IDevOpsProviderService m_devOpsProviderService;

    public HealthCheck(IDevOpsProviderService devOpsProviderService)
    {
        m_devOpsProviderService = devOpsProviderService;
    }
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!m_devOpsProviderService.HasValidConfiguration())
        {
            return Task.FromResult(HealthCheckResult.Degraded("Dev ops service has invalid configuration"));
        }

        var queries = m_devOpsProviderService.GetQueries();
        if (!queries.Any())
        {
            return Task.FromResult(HealthCheckResult.Degraded("No queries are available"));
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}