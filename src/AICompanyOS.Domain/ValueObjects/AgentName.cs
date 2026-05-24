namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Represents the name of an AI agent.
/// Enforces naming rules as a value object — immutable and self-validating.
/// </summary>
public sealed record AgentName
{
    public const int MaxLength = 100;
    public const int MinLength = 2;

    public string Value { get; }

    public AgentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Agent name cannot be null or whitespace.", nameof(value));

        value = value.Trim();

        if (value.Length < MinLength)
            throw new ArgumentException($"Agent name must be at least {MinLength} characters.", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"Agent name cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
