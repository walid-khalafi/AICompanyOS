using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ScheduleMeeting;

public sealed class ScheduleMeetingHandler : IRequestHandler<ScheduleMeetingCommand, Result>
{
    private readonly MeetingApplicationService _service;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleMeetingHandler(MeetingApplicationService service, IUnitOfWork unitOfWork)
    {
        _service = service;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ScheduleMeetingCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Orchestration delegated to application service.
            return await _service.ScheduleAsync(
                request.Topic,
                request.OrganizerAgentId,
                request.ParticipantAgentIds,
                cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}



