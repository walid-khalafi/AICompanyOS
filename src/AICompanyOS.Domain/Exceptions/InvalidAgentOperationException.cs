namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Thrown when an operation is attempted on an agent that violates
/// its current state or role-based business rules.
/// </summary>
public sealed class InvalidAgentOperationException(string message) : DomainException(message)
{
}
