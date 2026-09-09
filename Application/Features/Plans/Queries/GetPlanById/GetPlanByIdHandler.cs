using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Plans.Dtos;
using Account.Application.Features.Plans.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Plans.Queries.GetPlanById;

public class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPlanByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
    {
        var planRepo = _unitOfWork.GetRepository<Plan>();
        var plan = await planRepo.GetByIdAsync(request.Id, cancellationToken);
        if (plan == null)
        {
            return Result<PlanDto>.Failure(new Error("PLAN_NOT_FOUND", "Plan not found."));
        }

        return Result<PlanDto>.Success(plan.ToPlanDto());
    }
}
