using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Enums;
using MediatR;


namespace AICompanyOS.Application.Features.Decisions.FinalizeDecision;

public sealed record FinalizeDecisionCommand(
    Guid DecisionId,
    string Verdict,
    string Reasoning,
    Guid FinalizingAgentId,
    AgentRole FinalizingAgentRole) : IRequest<Result>;


