using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Plans.Dtos;
using Account.Application.Features.Plans.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Plans.Queries.GetPlans;

public class GetPlansHandler : IRequestHandler<GetPlansQuery, Result<IReadOnlyList<PlanDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPlansHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var planRepo = _unitOfWork.GetRepository<Plan>();
        var plans = request.ActiveOnly
            ? await planRepo.GetAllAsync(p => p.IsActive, cancellationToken)
            : await planRepo.GetAllAsync(null, cancellationToken);

        var dtos = plans
            .OrderBy(p => p.Price)
            .ThenBy(p => p.DurationMonths)
            .Select(p => p.ToPlanDto())
            .ToList();

        return Result<IReadOnlyList<PlanDto>>.Success(dtos);
    }
}
