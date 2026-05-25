namespace AICompanyOS.Application.Common.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
    public DateTime OccurredOnUtc { get; init; }
    public bool Processed { get; init; }
    public DateTime? ProcessedOnUtc { get; init; }
}

