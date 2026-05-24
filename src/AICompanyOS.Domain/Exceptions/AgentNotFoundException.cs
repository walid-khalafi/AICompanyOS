using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Exceptions;

/// <summary>
/// Thrown when an agent lookup fails — agent does not exist in the system.
/// </summary>
public sealed class AgentNotFoundException(AgentId agentId) : DomainException($"Agent with ID '{agentId}' was not found.")
{
}
