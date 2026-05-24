using AICompanyOS.Domain.Enums;

namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Thrown when an agent attempts an operation that its role does not permit.
/// For example, a Developer agent attempting to finalize a CEO decision.
/// </summary>
public sealed class UnauthorizedAgentOperationException(AgentRole role, string operation) : DomainException($"Agent with role '{role}' is not authorized to perform operation: '{operation}'.")
{
}
