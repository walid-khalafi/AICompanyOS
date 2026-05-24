using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.ValueObjects;

using TaskEntity = AICompanyOS.Domain.Entities.Task;

namespace AICompanyOS.Domain.Repositories;

/// <summary>
/// Repository contract for the canonical Task aggregate.
/// </summary>
public interface ITaskRepository
{
    /// <summary>Retrieves a task by its unique identifier. Returns null if not found.</summary>
    System.Threading.Tasks.Task<TaskEntity?> GetByIdAsync(TaskId id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all tasks assigned to a specific agent.</summary>
    System.Threading.Tasks.Task<IReadOnlyList<TaskEntity>> GetByAgentAsync(AgentId agentId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all tasks in a given status.</summary>
    System.Threading.Tasks.Task<IReadOnlyList<TaskEntity>> GetByStatusAsync(AICompanyOS.Domain.Enums.TaskStatus status, CancellationToken cancellationToken = default);

    /// <summary>Retrieves all pending (unassigned) tasks ordered by priority descending.</summary>
    System.Threading.Tasks.Task<IReadOnlyList<TaskEntity>> GetPendingTasksAsync(CancellationToken cancellationToken = default);

    /// <summary>Persists a new task.</summary>
    System.Threading.Tasks.Task AddAsync(TaskEntity task, CancellationToken cancellationToken = default);


    /// <summary>Marks an existing task as modified for persistence.</summary>
    void Update(TaskEntity task);
}
