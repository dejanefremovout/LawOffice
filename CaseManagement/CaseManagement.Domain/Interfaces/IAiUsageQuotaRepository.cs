namespace CaseManagement.Domain.Interfaces;

/// <summary>
/// Persists and tracks per-office daily AI request usage.
/// </summary>
public interface IAiUsageQuotaRepository
{
    Task<int> IncrementAndGetUsage(string officeId, DateOnly usageDate);
}
