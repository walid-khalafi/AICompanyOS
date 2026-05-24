using AICompanyOS.Domain.Primitives;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Events;

/// <summary>
/// Raised when the CEO agent finalizes a decision.
/// </summary>
public sealed record DecisionMadeEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    DecisionId DecisionId,
    AgentId CeoAgentId,
    string Verdict,
    string Reasoning
) : IDomainEvent
{
    public static DecisionMadeEvent Create(
        DecisionId decisionId,
        AgentId ceoAgentId,
        string verdict,
        string reasoning) =>
        new(Guid.NewGuid(), DateTime.UtcNow, decisionId, ceoAgentId, verdict, reasoning);
}
