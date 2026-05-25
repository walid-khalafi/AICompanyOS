using AICompanyOS.Application.Abstractions.Persistence;
using AICompanyOS.Application.Common.Result;
using AICompanyOS.Application.Services;
using MediatR;

namespace AICompanyOS.Application.Features.Meetings.ConcludeMeeting;

public sealed class ConcludeMeetingHandler : IRequestHandler<ConcludeMeetingCommand, Result>
{
    private readonly MeetingApplicationService _service;
    private readonly IUnitOfWork _unitOfWork;

    public ConcludeMeetingHandler(MeetingApplicationService service, IUnitOfWork unitOfWork)
    {
        _service = service;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConcludeMeetingCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            return await _service.ConcludeAsync(request.MeetingId, request.Summary, cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}




