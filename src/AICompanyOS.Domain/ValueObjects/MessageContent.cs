namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Represents the content body of a message between agents.
/// Enforces non-empty and maximum length constraints.
/// </summary>
public sealed record MessageContent
{
    public const int MaxLength = 4000;

    public string Value { get; }

    public MessageContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Message content cannot be null or whitespace.", nameof(value));

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new ArgumentException($"Message content cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
