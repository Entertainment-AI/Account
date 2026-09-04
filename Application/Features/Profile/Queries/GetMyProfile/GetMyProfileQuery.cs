using Account.Application.Common;
using Account.Application.Features.Profile.Dtos;
using MediatR;

namespace Account.Application.Features.Profile.Queries.GetMyProfile;

public record GetMyProfileQuery(Guid UserId) : IRequest<Result<ProfileDto>>;