using AICompanyOS.Application.Common.Result;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ConcludeMeeting;

public sealed record ConcludeMeetingCommand(
    Guid MeetingId,
    string? Summary) : IRequest<Result>;

