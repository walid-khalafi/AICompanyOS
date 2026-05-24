using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Repositories;

/// <summary>
/// Repository contract for the Meeting aggregate.
/// </summary>
public interface IMeetingRepository
{
    /// <summary>Retrieves a meeting by its unique identifier. Returns null if not found.</summary>
System.Threading.Tasks.Task<Meeting?> GetByIdAsync(MeetingId id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all meetings a specific agent participated in.</summary>
System.Threading.Tasks.Task<IReadOnlyList<Meeting>> GetByParticipantAsync(AgentId agentId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all meetings in a given status.</summary>
System.Threading.Tasks.Task<IReadOnlyList<Meeting>> GetByStatusAsync(MeetingStatus status, CancellationToken cancellationToken = default);

    /// <summary>Persists a new meeting.</summary>
System.Threading.Tasks.Task AddAsync(Meeting meeting, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing meeting as modified for persistence.</summary>
    void Update(Meeting meeting);
}
