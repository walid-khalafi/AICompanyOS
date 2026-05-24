namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Represents the title of a task. Enforces length and content constraints.
/// </summary>
public sealed record TaskTitle
{
    public const int MaxLength = 200;
    public const int MinLength = 3;

    public string Value { get; }

    public TaskTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Task title cannot be null or whitespace.", nameof(value));

        value = value.Trim();

        if (value.Length < MinLength)
            throw new ArgumentException($"Task title must be at least {MinLength} characters.", nameof(value));

        if (value.Length > MaxLength)
            throw new ArgumentException($"Task title cannot exceed {MaxLength} characters.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
