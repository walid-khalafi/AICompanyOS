namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a Task aggregate.
/// </summary>
public sealed record TaskId
{
    public Guid Value { get; }

    public TaskId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("TaskId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static TaskId New() => new(Guid.NewGuid());

    public static TaskId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid TaskId.", nameof(value));

        return new TaskId(guid);
    }

    public override string ToString() => Value.ToString();
}
