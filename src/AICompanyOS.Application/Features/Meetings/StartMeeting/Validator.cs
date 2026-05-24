using FluentValidation;

namespace AICompanyOS.Application.Features.Meetings.StartMeeting;

public sealed class StartMeetingValidator : AbstractValidator<StartMeetingCommand>
{
    public StartMeetingValidator()
    {
        RuleFor(x => x.MeetingId).NotEqual(Guid.Empty);
    }
}

