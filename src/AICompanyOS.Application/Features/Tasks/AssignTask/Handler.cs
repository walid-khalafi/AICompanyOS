using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;
using AICompanyOS.Domain.Repositories;
using MediatR;
using AICompanyOS.Domain.ValueObjects;

namespace AICompanyOS.Application.Features.Tasks.AssignTask;

public sealed class AssignTaskHandler : IRequestHandler<AssignTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IAgentRepository _agentRepository;

    public AssignTaskHandler(ITaskRepository taskRepository, IAgentRepository agentRepository)
    {
        _taskRepository = taskRepository;
        _agentRepository = agentRepository;
    }

    public async Task<Result> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(new TaskId(request.TaskId), cancellationToken);
            var agent = await _agentRepository.GetByIdAsync(new AgentId(request.AgentId), cancellationToken);

            if (task is null)
            {
                return Result.Fail($"Task not found: {request.TaskId}");
            }

            if (agent is null)
            {
                return Result.Fail($"Agent not found: {request.AgentId}");
            }

            // Business rules enforced in Domain aggregate method.
            task.AssignTo(agent.Id, agent.GetTaskAcceptanceEligibility());


            _taskRepository.Update(task);


            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

