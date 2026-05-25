using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Events;
using AICompanyOS.Application.Common.Outbox;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result>
{
    private readonly TaskApplicationService _service;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IOutboxWriter _outboxWriter;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskHandler(TaskApplicationService service, IDomainEventDispatcher dispatcher, IOutboxWriter outboxWriter, IUnitOfWork unitOfWork)
    {
        _service = service;
        _dispatcher = dispatcher;
        _outboxWriter = outboxWriter;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (serviceResult, task) = await _service.CreateAsync(
                request.Title,
                request.CreatedByAgentId,
                request.Priority,
                request.Description,
                cancellationToken);

            if (!serviceResult.IsSuccess)
                return serviceResult;

            if (task is null)
                return Result.Fail("Task creation produced no aggregate instance.");

            var occurredOnUtc = DateTime.UtcNow;
            var outboxMessages = DomainEventOutboxMapper.MapToOutboxMessages(task.DomainEvents, occurredOnUtc);
            if (outboxMessages.Count > 0)
            {
                await _outboxWriter.AddAsync(outboxMessages, cancellationToken);
            }

            await _dispatcher.DispatchAsync(task.DomainEvents, cancellationToken);
            task.ClearDomainEvents();

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

