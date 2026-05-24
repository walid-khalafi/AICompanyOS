using FluentValidation;

namespace AICompanyOS.Application.Features.Meetings.ScheduleMeeting;

public sealed class ScheduleMeetingValidator : AbstractValidator<ScheduleMeetingCommand>
{
    public ScheduleMeetingValidator()
    {
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.OrganizerAgentId).NotEqual(Guid.Empty);
        RuleFor(x => x.ParticipantAgentIds)
            .NotNull()
            .Must(x => x.Count >= 2)
            .WithMessage("At least 2 participant agent IDs are required.");

        RuleForEach(x => x.ParticipantAgentIds)
            .NotEqual(Guid.Empty);
    }
}

