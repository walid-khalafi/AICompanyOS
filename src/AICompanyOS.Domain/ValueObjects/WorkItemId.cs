namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a WorkItem entity.
/// </summary>
public sealed record WorkItemId
{
    public Guid Value { get; }

    public WorkItemId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("WorkItemId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static WorkItemId New() => new(Guid.NewGuid());

    public static WorkItemId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid WorkItemId.", nameof(value));

        return new WorkItemId(guid);
    }

    public override string ToString() => Value.ToString();
}
