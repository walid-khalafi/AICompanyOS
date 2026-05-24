using MediatR;
using AICompanyOS.Application.Common.Result;

namespace AICompanyOS.Application.Features.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    Guid CreatedByAgentId,
    int Priority,
    string? Description) : IRequest<Result>;

