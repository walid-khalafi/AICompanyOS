using AICompanyOS.Application.Common.Result;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.StartMeeting;

public sealed record StartMeetingCommand(Guid MeetingId) : IRequest<Result>;

