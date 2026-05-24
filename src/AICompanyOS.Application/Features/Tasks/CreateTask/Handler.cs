using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Entities;

using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace AICompanyOS.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;

    public CreateTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var title = new TaskTitle(request.Title);
            var agentId = new AgentId(request.CreatedByAgentId);
            var priority = (AICompanyOS.Domain.Enums.Priority)request.Priority;

            var task = AICompanyOS.Domain.Entities.Task.Create(title, agentId, priority, request.Description);


            await _taskRepository.AddAsync(task, cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

