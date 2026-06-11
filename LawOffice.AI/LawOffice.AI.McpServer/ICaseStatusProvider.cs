namespace LawOffice.AI.McpServer;

/// <summary>
/// Reads a case's status from the system of record. This is the "tools for structured truth" half of the:
/// case status is authoritative structured data, so the assistant should call an API for it
/// rather than retrieve it from embedded text (which is for unstructured knowledge).
/// </summary>
/// <remarks>
/// The demo backs this with an in-memory stub (<see cref="InMemoryCaseStatusProvider"/>) so the server
/// stays self-contained and the AI library keeps no dependency on the CaseManagement service. In
/// production this would call CaseManagement's <c>ICaseService.Get(caseId, officeId)</c> (which maps
/// <c>Case.Active</c> to a status), keeping the same <c>officeId</c>-scoped contract.
/// </remarks>
public interface ICaseStatusProvider
{
    /// <summary>
    /// Returns the status of <paramref name="caseId"/> within <paramref name="officeId"/>, or
    /// <see langword="null"/> if no such case exists for that office. The office scope is mandatory and
    /// supplied by the caller's identity, so a case belonging to another office is never returned.
    /// </summary>
    Task<string?> GetStatusAsync(string officeId, string caseId, CancellationToken cancellationToken = default);
}
