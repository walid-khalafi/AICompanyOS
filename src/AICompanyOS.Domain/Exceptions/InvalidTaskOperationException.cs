namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Thrown when an operation is attempted on a task that violates
/// its current lifecycle state or business rules.
/// </summary>
public sealed class InvalidTaskOperationException(string message) : DomainException(message)
{
}
