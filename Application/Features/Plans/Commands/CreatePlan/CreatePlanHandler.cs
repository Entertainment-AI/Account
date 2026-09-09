using Account.Application.Common;
using Account.Application.Common.Interfaces;
using Account.Application.Features.Plans.Dtos;
using Account.Application.Features.Plans.Mappers;
using Account.Domain.Entities;
using MediatR;

namespace Account.Application.Features.Plans.Commands.CreatePlan;

public class CreatePlanHandler : IRequestHandler<CreatePlanCommand, Result<PlanDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlanHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PlanDto>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var planRepo = _unitOfWork.GetRepository<Plan>();

        // Check if code already exists
        var existingPlan = await planRepo.GetAsync(p => p.Code == request.Code.Trim().ToUpperInvariant(), cancellationToken);
        if (existingPlan != null)
        {
            return Result<PlanDto>.Failure(new Error("PLAN_CODE_EXISTS", $"A plan with code '{request.Code}' already exists."));
        }

        var plan = Plan.Create(
            name: request.Name,
            code: request.Code,
            price: request.Price,
            durationMonths: request.DurationMonths,
            description: request.Description,
            features: request.Features,
            isActive: request.IsActive
        );

        await planRepo.AddAsync(plan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PlanDto>.Success(plan.ToPlanDto());
    }
}
