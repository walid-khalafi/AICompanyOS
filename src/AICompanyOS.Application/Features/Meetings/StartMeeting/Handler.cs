using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Repositories;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.StartMeeting;

public sealed class StartMeetingHandler : IRequestHandler<StartMeetingCommand, Result>
{
    private readonly IMeetingRepository _meetingRepository;

    public StartMeetingHandler(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<Result> Handle(StartMeetingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var meeting = await _meetingRepository.GetByIdAsync(new MeetingId(request.MeetingId), cancellationToken);
            if (meeting is null)
            {
                return Result.Fail($"Meeting not found: {request.MeetingId}");
            }

            // Business rules enforced in Domain aggregate method.
            meeting.Start();

            _meetingRepository.Update(meeting);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}

