using AICompanyOS.Application.Common.Events;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Tasks.CompleteTask;

public sealed class CompleteTaskHandler : IRequestHandler<CompleteTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IDomainEventDispatcher _dispatcher;

    public CompleteTaskHandler(ITaskRepository taskRepository, IDomainEventDispatcher dispatcher)
    {
        _taskRepository = taskRepository;
        _dispatcher = dispatcher;
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

            await _dispatcher.DispatchAsync(task.DomainEvents, cancellationToken);
            task.ClearDomainEvents();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

