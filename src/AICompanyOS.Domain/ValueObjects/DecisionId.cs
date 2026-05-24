namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a Decision entity.
/// </summary>
public sealed record DecisionId
{
    public Guid Value { get; }

    public DecisionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DecisionId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static DecisionId New() => new(Guid.NewGuid());

    public static DecisionId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid DecisionId.", nameof(value));

        return new DecisionId(guid);
    }

    public override string ToString() => Value.ToString();
}
