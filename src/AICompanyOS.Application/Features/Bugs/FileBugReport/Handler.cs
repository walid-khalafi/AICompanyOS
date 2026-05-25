using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Events;
using AICompanyOS.Application.Common.Outbox;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Bugs.FileBugReport;

public sealed class FileBugReportHandler : IRequestHandler<FileBugReportCommand, Result>
{
    private readonly BugReportApplicationService _service;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IOutboxWriter _outboxWriter;
    private readonly IUnitOfWork _unitOfWork;

    public FileBugReportHandler(
        BugReportApplicationService service,
        IDomainEventDispatcher dispatcher,
        IOutboxWriter outboxWriter,
        IUnitOfWork unitOfWork)
    {
        _service = service;
        _dispatcher = dispatcher;
        _outboxWriter = outboxWriter;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FileBugReportCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (serviceResult, report) = await _service.FileAsync(
                request.Title,
                request.Description,
                request.Severity,
                request.ReportingAgentId,
                request.ReportingAgentRole,
                cancellationToken);

            if (!serviceResult.IsSuccess)
                return serviceResult;

            if (report is null)
                return Result.Fail("Bug report creation produced no aggregate instance.");

            var occurredOnUtc = DateTime.UtcNow;
            var outboxMessages = DomainEventOutboxMapper.MapToOutboxMessages(report.DomainEvents, occurredOnUtc);
            if (outboxMessages.Count > 0)
            {
                await _outboxWriter.AddAsync(outboxMessages, cancellationToken);
            }

            await _dispatcher.DispatchAsync(report.DomainEvents, cancellationToken);
            report.ClearDomainEvents();

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


