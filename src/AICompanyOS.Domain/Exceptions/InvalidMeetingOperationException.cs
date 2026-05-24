namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Thrown when a meeting operation violates scheduling or participation rules.
/// </summary>
public sealed class InvalidMeetingOperationException(string message) : DomainException(message)
{
}
