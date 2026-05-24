using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when a QA agent files a new bug report.
/// </summary>
public sealed record BugReportedEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    BugReportId BugReportId,
    AgentId ReportedByAgentId,
    string Title,
    Priority Severity
) : IDomainEvent
{
    public static BugReportedEvent Create(
        BugReportId bugReportId,
        AgentId reportedBy,
        string title,
        Priority severity) =>
        new(Guid.NewGuid(), DateTime.UtcNow, bugReportId, reportedBy, title, severity);
}
