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
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!m_devOpsProviderService.HasValidConfiguration())
        {
            return HealthCheckResult.Degraded("Dev ops service has invalid configuration");
        }

        return HealthCheckResult.Healthy();
    }
}