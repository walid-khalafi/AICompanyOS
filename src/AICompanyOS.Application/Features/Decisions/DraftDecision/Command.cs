using AICompanyOS.Application.Common.Result;
using AICompanyOS.Domain.Enums;
using AICompanyOS.Domain.ValueObjects;
using MediatR;

namespace AICompanyOS.Application.Features.Decisions.DraftDecision;

public sealed record DraftDecisionCommand(
    string Subject,
    Guid DraftingAgentId,
    AgentRole DraftingAgentRole,
    Guid? RelatedMeetingId) : IRequest<Result>;

