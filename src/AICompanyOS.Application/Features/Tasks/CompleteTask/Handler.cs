using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Tasks.CompleteTask;

public sealed class CompleteTaskHandler : IRequestHandler<CompleteTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;

    public CompleteTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var task = await _taskRepository.GetByIdAsync(new TaskId(request.TaskId), cancellationToken);
            if (task is null)
            {
                return Result.Fail($"Task not found: {request.TaskId}");
            }

            // Business rules enforced in Domain aggregate method.
            task.Complete(request.CompletionResult);

            _taskRepository.Update(task);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

