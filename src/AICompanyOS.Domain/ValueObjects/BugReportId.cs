namespace AICompanyOS.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a BugReport entity.
/// </summary>
public sealed record BugReportId
{
    public Guid Value { get; }

    public BugReportId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("BugReportId cannot be an empty GUID.", nameof(value));

        Value = value;
    }

    public static BugReportId New() => new(Guid.NewGuid());

    public static BugReportId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"'{value}' is not a valid BugReportId.", nameof(value));

        return new BugReportId(guid);
    }

    public override string ToString() => Value.ToString();
}
