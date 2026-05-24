using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a bug report is marked as resolved.
/// The Application layer can use this event to close linked tasks,
/// notify the reporting QA agent, or update dashboards.
/// </summary>
public sealed record BugResolvedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    BugReportId BugReportId,
    AgentId ResolvedByAgentId,
    string ResolutionNotes
) : IDomainEvent
{
    public static BugResolvedEvent Create(
        BugReportId bugReportId,
        AgentId resolvedBy,
        string resolutionNotes) =>
        new(Guid.NewGuid(), DateTime.UtcNow, bugReportId, resolvedBy, resolutionNotes);
}
