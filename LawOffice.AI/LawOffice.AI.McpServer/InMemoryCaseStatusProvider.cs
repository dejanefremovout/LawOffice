using System.Collections.Concurrent;

namespace LawOffice.AI.McpServer;

/// <summary>
/// In-memory stub for <see cref="ICaseStatusProvider"/>, seeded with a few cases per office. Lookups are
/// keyed by (officeId, caseId), so a case id that exists under one office is invisible to another — the
/// same tenant-isolation discipline the vector store enforces, applied to structured data.
/// </summary>
public sealed class InMemoryCaseStatusProvider : ICaseStatusProvider
{
    // Key: "{officeId}/{caseId}" (case-insensitive) -> status.
    private readonly ConcurrentDictionary<string, string> _cases = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Creates a provider with a default demo seed (office-a and office-b cases).</summary>
    public static InMemoryCaseStatusProvider WithDemoSeed()
    {
        InMemoryCaseStatusProvider provider = new();
        provider.Seed("office-a", "CASE-001", "Active");
        provider.Seed("office-a", "CASE-002", "Closed");
        provider.Seed("office-a", "CASE-003", "Pending hearing");
        provider.Seed("office-b", "CASE-001", "Active");
        return provider;
    }

    /// <summary>Adds or overwrites the status of a case under an office.</summary>
    public void Seed(string officeId, string caseId, string status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        _cases[Key(officeId, caseId)] = status;
    }

    public Task<string?> GetStatusAsync(string officeId, string caseId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        ArgumentException.ThrowIfNullOrWhiteSpace(caseId);

        string? status = _cases.TryGetValue(Key(officeId, caseId), out string? value) ? value : null;
        return Task.FromResult(status);
    }

    private static string Key(string officeId, string caseId) => $"{officeId}/{caseId}";
}
