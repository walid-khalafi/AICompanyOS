using FluentValidation;

namespace AICompanyOS.Application.Features.Decisions.DraftDecision;

public sealed class DraftDecisionValidator : AbstractValidator<DraftDecisionCommand>
{
    public DraftDecisionValidator()
    {
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.DraftingAgentId).NotEqual(Guid.Empty);
        RuleFor(x => x.RelatedMeetingId)
            .Must(id => id is null || id != Guid.Empty)
            .WithMessage("RelatedMeetingId cannot be empty GUID when provided");
    }
}

