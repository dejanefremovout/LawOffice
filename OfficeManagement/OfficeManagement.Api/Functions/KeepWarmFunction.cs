using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using OfficeManagement.Application.Services;

namespace OfficeManagement.Api.Functions;

/// <summary>
/// Keeps the Function App warm for latency-sensitive Entra callouts.
/// </summary>
public class KeepWarmFunction(ILawyerService lawyerService,
    ILogger<KeepWarmFunction> logger)
{
    private readonly ILawyerService _lawyerService = lawyerService;
    private readonly ILogger<KeepWarmFunction> _logger = logger;

    [Function("KeepWarmFunction")]
    public async Task Run([TimerTrigger("0 */4 * * * *")] TimerInfo? timerInfo)
    {
        try
        {
            // Prime the same Cosmos/DI path used by UserSignUpFunction without side effects.
            _ = await _lawyerService.UserWithEmailExist("warmup@lawoffice.invalid");
            _logger.LogInformation("KeepWarmFunction completed at {UtcNow}.", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            // Warmup failures should never crash host execution.
            _logger.LogWarning(ex, "KeepWarmFunction warmup probe failed.");
        }
    }
}
