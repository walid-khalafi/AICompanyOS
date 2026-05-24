namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for an Agent aggregate.
/// Immutable by design — value objects never change after creation.
/// </summary>
public sealed record AgentId
{
    public Guid Value { get; }

    public AgentId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("AgentId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    /// <summary>Creates a new unique AgentId.</summary>
    public static AgentId New() => new(Guid.NewGuid());

    /// <summary>Parses an AgentId from a string representation.</summary>
    public static AgentId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid AgentId.", nameof(value));

        return new AgentId(guid);
    }

    public override string ToString() => Value.ToString();
}
