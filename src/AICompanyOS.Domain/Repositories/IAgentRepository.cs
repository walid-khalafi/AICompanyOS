using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Repositories;

/// <summary>
/// Repository contract for the Agent aggregate.
/// Implementations live in the Infrastructure/Persistence layer.
/// The domain only defines what it needs — not how it is stored.
/// </summary>
public interface IAgentRepository
{
    /// <summary>Retrieves an agent by its unique identifier. Returns null if not found.</summary>
    System.Threading.Tasks.Task<Agent?> GetByIdAsync(AgentId id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all agents with a specific role.</summary>
    System.Threading.Tasks.Task<IReadOnlyList<Agent>> GetByRoleAsync(AgentRole role, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all agents currently in Idle status.</summary>
    System.Threading.Tasks.Task<IReadOnlyList<Agent>> GetAvailableAgentsAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves all agents in the system.</summary>
    System.Threading.Tasks.Task<IReadOnlyList<Agent>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Persists a new agent.</summary>
    System.Threading.Tasks.Task AddAsync(Agent agent, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing agent as modified for persistence.</summary>
    void Update(Agent agent);

    /// <summary>Returns true if an agent with the given ID exists.</summary>
    System.Threading.Tasks.Task<bool> ExistsAsync(AgentId id, CancellationToken cancellationToken = default);
}
