using FluentValidation;

namespace AICompanyOS.Application.Features.Decisions.FinalizeDecision;

public sealed class FinalizeDecisionValidator : AbstractValidator<FinalizeDecisionCommand>
{
    public FinalizeDecisionValidator()
    {
        RuleFor(x => x.DecisionId).NotEqual(Guid.Empty);
        RuleFor(x => x.FinalizingAgentId).NotEqual(Guid.Empty);
        RuleFor(x => x.Verdict).NotEmpty();
        RuleFor(x => x.Reasoning).NotEmpty();
        RuleFor(x => x.FinalizingAgentRole).IsInEnum();
    }
}


