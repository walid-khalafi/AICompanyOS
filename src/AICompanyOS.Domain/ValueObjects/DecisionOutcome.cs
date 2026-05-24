namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Represents the outcome rationale of a CEO decision.
/// Captures both the verdict and the reasoning behind it.
/// </summary>
public sealed record DecisionOutcome
{
    public const int MaxReasoningLength = 2000;

    /// <summary>Short summary of the decision verdict.</summary>
    public string Verdict { get; }

    /// <summary>Detailed reasoning or justification for the decision.</summary>
    public string Reasoning { get; }

    public DecisionOutcome(string verdict, string reasoning)
    {
        if (string.IsNullOrWhiteSpace(verdict))
            throw new ArgumentException("Decision verdict cannot be null or whitespace.", nameof(verdict));

        if (string.IsNullOrWhiteSpace(reasoning))
            throw new ArgumentException("Decision reasoning cannot be null or whitespace.", nameof(reasoning));

        if (reasoning.Length > MaxReasoningLength)
            throw new ArgumentException($"Decision reasoning cannot exceed {MaxReasoningLength} characters.", nameof(reasoning));

        Verdict = verdict.Trim();
        Reasoning = reasoning.Trim();
    }

    public override string ToString() => $"{Verdict}: {Reasoning}";
}
