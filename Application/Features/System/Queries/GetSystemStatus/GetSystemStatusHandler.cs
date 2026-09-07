using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.System.Dtos;
using Account.Domain.Common.DateTimes;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.System.Queries.GetSystemStatus;

public class GetSystemStatusHandler : IRequestHandler<GetSystemStatusQuery, Result<SystemStatusDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSystemStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SystemStatusDto>> Handle(GetSystemStatusQuery request, CancellationToken cancellationToken)
    {
        var userRepo = _unitOfWork.GetRepository<User>();
        _ = await userRepo.GetAsync(u => !u.Deleted, cancellationToken);

        return Result<SystemStatusDto>.Success(new SystemStatusDto(
            "OPERATIONAL",
            "Nyxoris Account Service",
            Clock.Now
        ));
    }
}
