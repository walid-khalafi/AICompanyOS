using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Events;
using AICompanyOS.Application.Common.Outbox;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ScheduleMeeting;

public sealed class ScheduleMeetingHandler : IRequestHandler<ScheduleMeetingCommand, Result>
{
    private readonly MeetingApplicationService _service;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly IOutboxWriter _outboxWriter;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleMeetingHandler(
        MeetingApplicationService service,
        IDomainEventDispatcher dispatcher,
        IOutboxWriter outboxWriter,
        IUnitOfWork unitOfWork)
    {
        _service      = service;
        _dispatcher   = dispatcher;
        _outboxWriter = outboxWriter;
        _unitOfWork   = unitOfWork;
    }

    public async Task<Result> Handle(ScheduleMeetingCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var (serviceResult, meeting) = await _service.ScheduleAsync(
                request.Topic,
                request.OrganizerAgentId,
                request.ParticipantAgentIds,
                cancellationToken);

            if (!serviceResult.IsSuccess)
                return serviceResult;

            if (meeting is null)
                return Result.Fail("Meeting scheduling produced no aggregate instance.");

            var occurredOnUtc = DateTime.UtcNow;
            var outboxMessages = DomainEventOutboxMapper.MapToOutboxMessages(meeting.DomainEvents, occurredOnUtc);
            if (outboxMessages.Count > 0)
                await _outboxWriter.AddAsync(outboxMessages, cancellationToken);

            await _dispatcher.DispatchAsync(meeting.DomainEvents, cancellationToken);
            meeting.ClearDomainEvents();

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
