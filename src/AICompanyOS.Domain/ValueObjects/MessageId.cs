namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a Message entity.
/// </summary>
public sealed record MessageId
{
    public Guid Value { get; }

    public MessageId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("MessageId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static MessageId New() => new(Guid.NewGuid());

    public static MessageId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid MessageId.", nameof(value));

        return new MessageId(guid);
    }

    public override string ToString() => Value.ToString();
}
