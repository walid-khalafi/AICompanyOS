using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.Events;
using AICompanyOS.Domain.Exceptions;
using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Entities;

/// <summary>
/// Aggregate Root — represents a formal decision made by the CEO agent.
///
/// Business rules (enforced locally, no cross-aggregate dependencies):
///   - Only an agent with the CEO role can create a decision.
///   - Only the CEO agent who drafted it can finalize it.
///   - A decision is immutable once finalized.
///
/// Role enforcement uses AgentRole (an enum/value) rather than a live Agent
/// aggregate, keeping this aggregate self-contained.
/// The Application layer is responsible for loading the agent and passing
/// its Id and Role into these methods.
/// </summary>
public sealed class Decision : AggregateRoot<DecisionId>
{
    /// <summary>The subject or context of the decision.</summary>
    public string Subject { get; private init; }

    /// <summary>The outcome — verdict and reasoning. Null until finalized.</summary>
    public DecisionOutcome? Outcome { get; private set; }

    /// <summary>The AgentId of the CEO who created this decision.</summary>
    public AgentId MadeByAgentId { get; private init; }

    /// <summary>Whether this decision has been finalized.</summary>
    public bool IsFinalized { get; private set; }

    /// <summary>UTC timestamp when the decision was drafted.</summary>
    public DateTime CreatedOnUtc { get; private init; }

    /// <summary>UTC timestamp when the decision was finalized.</summary>
    public DateTime? FinalizedOnUtc { get; private set; }

    /// <summary>Optional reference to a meeting where this decision was made.</summary>
    public MeetingId? RelatedMeetingId { get; private init; }

#pragma warning disable CS8618
    private Decision() { } // ORM — properties are set by the persistence layer
#pragma warning restore CS8618

    private Decision(DecisionId id, string subject, AgentId madeBy, MeetingId? relatedMeetingId)
        : base(id)
    {
        Subject = subject;
        MadeByAgentId = madeBy;
        IsFinalized = false;
        CreatedOnUtc = DateTime.UtcNow;
        RelatedMeetingId = relatedMeetingId;
    }

    /// <summary>
    /// Factory method — drafts a new decision.
    ///
    /// Accepts AgentId and AgentRole rather than a full Agent aggregate.
    /// The Application layer resolves the agent and passes these primitives in,
    /// keeping this aggregate free of cross-aggregate dependencies.
    ///
    /// Invariant: only the CEO role may draft decisions.
    /// </summary>
    public static Decision Draft(
        string subject,
        AgentId ceoAgentId,
        AgentRole ceoAgentRole,
        MeetingId? relatedMeetingId = null)
    {
        if (ceoAgentRole != AgentRole.CEO)
            throw new UnauthorizedAgentOperationException(ceoAgentRole, "draft a decision");

        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Decision subject cannot be empty.", nameof(subject));

        return new Decision(DecisionId.New(), subject.Trim(), ceoAgentId, relatedMeetingId);
    }

    // -------------------------------------------------------------------------
    // Behavior
    // -------------------------------------------------------------------------

    /// <summary>
    /// Finalizes the decision with a verdict and reasoning.
    ///
    /// Invariants:
    ///   - Only the CEO role may finalize.
    ///   - Only the agent who drafted it may finalize it.
    ///   - A finalized decision is immutable.
    ///
    /// The Application layer resolves the agent and passes its Id and Role in.
    /// </summary>
    public void Finalize(DecisionOutcome outcome, AgentId finalizingAgentId, AgentRole finalizingAgentRole)
    {
        if (finalizingAgentRole != AgentRole.CEO)
            throw new UnauthorizedAgentOperationException(finalizingAgentRole, "finalize a decision");

        if (finalizingAgentId != MadeByAgentId)
            throw new UnauthorizedAgentOperationException(
                finalizingAgentRole,
                "finalize a decision that was drafted by a different agent");

        if (IsFinalized)
            throw new DomainException($"Decision '{Subject}' is already finalized and cannot be modified.");

        Outcome = outcome;
        IsFinalized = true;
        FinalizedOnUtc = DateTime.UtcNow;

        RaiseDomainEvent(DecisionMadeEvent.Create(Id, MadeByAgentId, outcome.Verdict, outcome.Reasoning));
    }
}
