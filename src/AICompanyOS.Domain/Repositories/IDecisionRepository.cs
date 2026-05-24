using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Repositories;

/// <summary>
/// Repository contract for Decision entities.
/// </summary>
public interface IDecisionRepository
{
    /// <summary>Retrieves a decision by its unique identifier. Returns null if not found.</summary>
System.Threading.Tasks.Task<Decision?> GetByIdAsync(DecisionId id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all finalized decisions made by a specific CEO agent.</summary>
System.Threading.Tasks.Task<IReadOnlyList<Decision>> GetFinalizedByAgentAsync(AgentId agentId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all pending (non-finalized) decisions.</summary>
System.Threading.Tasks.Task<IReadOnlyList<Decision>> GetPendingAsync(CancellationToken cancellationToken = default);

    /// <summary>Persists a new decision.</summary>
System.Threading.Tasks.Task AddAsync(Decision decision, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing decision as modified for persistence.</summary>
    void Update(Decision decision);
}
