using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ConcludeMeeting;

public sealed class ConcludeMeetingHandler : IRequestHandler<ConcludeMeetingCommand, Result>
{
    private readonly IMeetingRepository _meetingRepository;

    public ConcludeMeetingHandler(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<Result> Handle(ConcludeMeetingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var meeting = await _meetingRepository.GetByIdAsync(new MeetingId(request.MeetingId), cancellationToken);
            if (meeting is null)
            {
                return Result.Fail($"Meeting not found: {request.MeetingId}");
            }

            // Business rules enforced in Domain aggregate method.
            meeting.Conclude(request.Summary);

            _meetingRepository.Update(meeting);

            // Domain events dispatching is expected via existing integration abstraction.
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

