namespace LawOffice.AI.McpServer;

/// <summary>
/// Resolves the session tenant from configuration (section "McpServer:OfficeId", overridable via the
/// <c>McpServer__OfficeId</c> environment variable). This represents the authenticated identity under
/// which the stdio server process was launched — e.g. the office id a real host would pass as an OAuth
/// claim. Because it is fixed at startup and never sourced from tool arguments, the model cannot change it.
/// </summary>
public sealed class ConfiguredTenantContext : ITenantContext
{
    public ConfiguredTenantContext(string officeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(officeId);
        OfficeId = officeId;
    }

    public string OfficeId { get; }
}
