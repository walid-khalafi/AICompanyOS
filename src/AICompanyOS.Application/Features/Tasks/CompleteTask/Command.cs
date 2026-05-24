using AICompanyOS.Application.Common.Result;
using MediatR;

namespace AICompanyOS.Application.Features.Tasks.CompleteTask;

public sealed record CompleteTaskCommand(
    Guid TaskId,
    string? CompletionResult) : IRequest<Result>;

