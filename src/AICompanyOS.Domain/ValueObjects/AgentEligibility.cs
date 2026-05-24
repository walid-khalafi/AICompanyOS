using System;

namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Value object encapsulating whether an Agent can accept work.
/// This avoids leaking raw agent status primitives into Task method signatures
/// while still enforcing invariants inside the Task aggregate.
/// </summary>
public sealed record AgentEligibility
{
    public bool CanAcceptWork { get; }
    public string Reason { get; }

    private AgentEligibility(bool canAcceptWork, string reason)
    {
        CanAcceptWork = canAcceptWork;
        Reason = reason;
    }

    public static AgentEligibility Acceptable()
        => new(true, "");

    public static AgentEligibility NotAcceptable(string reasonIfNotAcceptable)
    {
        if (string.IsNullOrWhiteSpace(reasonIfNotAcceptable))
            throw new ArgumentException("Reason must be provided.", nameof(reasonIfNotAcceptable));

        return new(false, reasonIfNotAcceptable.Trim());
    }
}

