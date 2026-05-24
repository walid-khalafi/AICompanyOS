using AICompanyOS.Application.Common.Result;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ScheduleMeeting;

public sealed record ScheduleMeetingCommand(
    string Topic,
    Guid OrganizerAgentId,
    IReadOnlyList<Guid> ParticipantAgentIds) : IRequest<Result>;

