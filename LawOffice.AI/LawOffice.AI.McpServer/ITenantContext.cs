namespace LawOffice.AI.McpServer;

/// <summary>
/// The tenant (office) the current MCP session acts on behalf of. This is the security spine of the
/// server: every tool resolves its <c>officeId</c> from here — derived from the server's authenticated
/// identity — and <b>never</b> from a parameter the model supplies. The model is given no slot to name a
/// tenant, so a malicious or confused prompt cannot reach another office's data even if it tries.
/// </summary>
/// <remarks>
/// In this demo the identity comes from configuration (see <see cref="ConfiguredTenantContext"/>),
/// standing in for the authenticated user's session/OAuth claim in a real deployment. Swapping the
/// implementation for one that reads the claim off the request principal (HTTP transport + OAuth) is the
/// only change needed to make this production-grade — the tools stay identical.
/// </remarks>
public interface ITenantContext
{
    /// <summary>The office id all tool calls in this session are scoped to.</summary>
    string OfficeId { get; }
}
