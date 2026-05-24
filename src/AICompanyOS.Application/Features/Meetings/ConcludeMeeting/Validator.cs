using FluentValidation;

namespace AICompanyOS.Application.Features.Meetings.ConcludeMeeting;

public sealed class ConcludeMeetingValidator : AbstractValidator<ConcludeMeetingCommand>
{
    public ConcludeMeetingValidator()
    {
        RuleFor(x => x.MeetingId).NotEqual(Guid.Empty);
    }
}

