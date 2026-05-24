using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Events;
using AICompanyOS.Domain.Exceptions;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Aggregate Root — represents a bug report filed by a QA agent.
///
/// Business rules (enforced locally, no cross-aggregate dependencies):
///   - Only an agent with the QA role may file a bug report.
///   - A resolved bug report is immutable.
///
/// Role enforcement uses AgentRole (an enum/value) rather than a live Agent
/// aggregate. The Application layer resolves the agent and passes its
/// Id and Role into the factory method.
/// </summary>
public sealed class BugReport : AggregateRoot<BugReportId>
{
    /// <summary>Short title describing the bug.</summary>
    public string Title { get; private set; }

    /// <summary>Detailed description of the bug, steps to reproduce, etc.</summary>
    public string Description { get; private set; }

    /// <summary>Severity/priority of the bug.</summary>
    public Priority Severity { get; private set; }

    /// <summary>The AgentId of the QA agent who filed this report.</summary>
    public AgentId ReportedByAgentId { get; private init; }

    /// <summary>The developer agent assigned to fix this bug, if any.</summary>
    public AgentId? AssignedToAgentId { get; private set; }

    /// <summary>The task created to track the fix, if any.</summary>
    public TaskId? LinkedTaskId { get; private set; }

    /// <summary>Whether the bug has been resolved.</summary>
    public bool IsResolved { get; private set; }

    /// <summary>Resolution notes written when the bug is closed.</summary>
    public string? ResolutionNotes { get; private set; }

    /// <summary>The agent who resolved this bug report, if resolved.</summary>
    public AgentId? ResolvedByAgentId { get; private set; }

    /// <summary>UTC timestamp when the bug was reported.</summary>
    public DateTime ReportedOnUtc { get; private init; }

    /// <summary>UTC timestamp when the bug was resolved.</summary>
    public DateTime? ResolvedOnUtc { get; private set; }

#pragma warning disable CS8618
    private BugReport() { } // ORM — properties are set by the persistence layer
#pragma warning restore CS8618

    private BugReport(
        BugReportId id,
        string title,
        string description,
        Priority severity,
        AgentId reportedBy)
        : base(id)
    {
        Title             = title;
        Description       = description;
        Severity          = severity;
        ReportedByAgentId = reportedBy;
        IsResolved        = false;
        ReportedOnUtc     = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method — files a new bug report.
    ///
    /// Accepts AgentId and AgentRole rather than a full Agent aggregate.
    /// The Application layer resolves the agent and passes these primitives in.
    ///
    /// Invariant: only the QA role may file bug reports.
    /// </summary>
    public static BugReport File(
        string title,
        string description,
        Priority severity,
        AgentId reportingAgentId,
        AgentRole reportingAgentRole)
    {
        if (reportingAgentRole != AgentRole.QA)
            throw new UnauthorizedAgentOperationException(reportingAgentRole, "file a bug report");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Bug report title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Bug report description cannot be empty.", nameof(description));

        var report = new BugReport(
            BugReportId.New(),
            title.Trim(),
            description.Trim(),
            severity,
            reportingAgentId);

        report.RaiseDomainEvent(
            BugReportedEvent.Create(report.Id, reportingAgentId, title.Trim(), severity));

        return report;
    }

    // -------------------------------------------------------------------------
    // Behavior
    // -------------------------------------------------------------------------

    /// <summary>
    /// Assigns this bug to a developer agent for fixing.
    /// Invariant: resolved bugs cannot be reassigned.
    /// </summary>
    public void AssignTo(AgentId developerAgentId)
    {
        if (IsResolved)
            throw new DomainException("Cannot reassign a resolved bug report.");

        AssignedToAgentId = developerAgentId;
    }

    /// <summary>
    /// Links this bug report to a task that tracks the fix.
    /// Invariant: resolved bugs cannot be linked to new tasks.
    /// </summary>
    public void LinkToTask(TaskId taskId)
    {
        if (IsResolved)
            throw new DomainException("Cannot link a task to a resolved bug report.");

        LinkedTaskId = taskId;
    }

    /// <summary>
    /// Marks the bug as resolved with mandatory resolution notes.
    /// Raises <see cref="BugResolvedEvent"/> so the Application layer
    /// can close linked tasks and notify stakeholders.
    ///
    /// Invariant: resolution notes are required.
    /// </summary>
    public void Resolve(AgentId resolvedByAgentId, string resolutionNotes)
    {
        if (IsResolved)
            return; // idempotent

        if (string.IsNullOrWhiteSpace(resolutionNotes))
            throw new ArgumentException(
                "Resolution notes are required to close a bug report.", nameof(resolutionNotes));

        IsResolved        = true;
        ResolutionNotes   = resolutionNotes.Trim();
        ResolvedByAgentId = resolvedByAgentId;
        ResolvedOnUtc     = DateTime.UtcNow;

        RaiseDomainEvent(BugResolvedEvent.Create(Id, resolvedByAgentId, resolutionNotes.Trim()));
    }

    /// <summary>
    /// Updates the severity of an unresolved bug.
    /// Invariant: resolved bugs are immutable.
    /// </summary>
    public void UpdateSeverity(Priority severity)
    {
        if (IsResolved)
            throw new DomainException("Cannot update severity of a resolved bug report.");

        Severity = severity;
    }
}
