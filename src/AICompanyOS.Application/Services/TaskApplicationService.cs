using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Application.Services;

public sealed class TaskApplicationService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IAgentRepository _agentRepository;

    public TaskApplicationService(ITaskRepository taskRepository, IAgentRepository agentRepository)
    {
        _taskRepository = taskRepository;
        _agentRepository = agentRepository;
    }

    public async Task<(Result Result, AICompanyOS.Domain.Entities.Task? Task)> CreateAsync(
        string title,
        Guid createdByAgentId,
        int priority,
        string? description,
        CancellationToken cancellationToken)
    {
        var task = AICompanyOS.Domain.Entities.Task.Create(
            new TaskTitle(title),
            new AgentId(createdByAgentId),
            (AICompanyOS.Domain.Enums.Priority)priority,
            description);

        await _taskRepository.AddAsync(task, cancellationToken);
        _taskRepository.Update(task);

        return (Result.Ok(), task);
    }

    public async Task<(Result Result, AICompanyOS.Domain.Entities.Task? Task)> AssignAsync(
        Guid taskId,
        Guid agentId,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(new TaskId(taskId), cancellationToken);
        if (task is null)
            return (Result.Fail($"Task not found: {taskId}"), null);

        var agent = await _agentRepository.GetByIdAsync(new AgentId(agentId), cancellationToken);
        if (agent is null)
            return (Result.Fail($"Agent not found: {agentId}"), null);

        task.AssignTo(agent.Id, agent.GetTaskAcceptanceEligibility());
        _taskRepository.Update(task);

        return (Result.Ok(), task);
    }

    public async Task<(Result Result, AICompanyOS.Domain.Entities.Task? Task)> CompleteAsync(
        Guid taskId,
        string? completionResult,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(new TaskId(taskId), cancellationToken);
        if (task is null)
            return (Result.Fail($"Task not found: {taskId}"), null);

        task.Complete(completionResult);
        _taskRepository.Update(task);

        return (Result.Ok(), task);
    }
}








