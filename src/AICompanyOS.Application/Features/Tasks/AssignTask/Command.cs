using MediatR;
using AICompanyOS.Application.Common.Result;

namespace AICompanyOS.Application.Features.Tasks.AssignTask;

public sealed record AssignTaskCommand(
    Guid TaskId,
    Guid AgentId) : IRequest<Result>;

