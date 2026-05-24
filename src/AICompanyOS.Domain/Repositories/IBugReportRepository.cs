using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Domain.Repositories;

/// <summary>
/// Repository contract for BugReport entities.
/// </summary>
public interface IBugReportRepository
{
    /// <summary>Retrieves a bug report by its unique identifier. Returns null if not found.</summary>
System.Threading.Tasks.Task<BugReport?> GetByIdAsync(BugReportId id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all unresolved bug reports ordered by severity descending.</summary>
System.Threading.Tasks.Task<IReadOnlyList<BugReport>> GetUnresolvedAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves all bug reports filed by a specific QA agent.</summary>
System.Threading.Tasks.Task<IReadOnlyList<BugReport>> GetByReporterAsync(AgentId agentId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all bug reports assigned to a specific developer agent.</summary>
System.Threading.Tasks.Task<IReadOnlyList<BugReport>> GetByAssigneeAsync(AgentId agentId, CancellationToken cancellationToken = default);

    /// <summary>Persists a new bug report.</summary>
System.Threading.Tasks.Task AddAsync(BugReport bugReport, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing bug report as modified for persistence.</summary>
    void Update(BugReport bugReport);
}
