namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a Meeting aggregate.
/// </summary>
public sealed record MeetingId
{
    public Guid Value { get; }

    public MeetingId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("MeetingId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static MeetingId New() => new(Guid.NewGuid());

    public static MeetingId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid MeetingId.", nameof(value));

        return new MeetingId(guid);
    }

    public override string ToString() => Value.ToString();
}
