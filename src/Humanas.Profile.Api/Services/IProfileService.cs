using Humanas.Profile.Api.Dtos;

namespace Humanas.Profile.Api.Services;

public interface IProfileService
{
    Task<CreateProfileResponse> CreateAsync(CreateProfileRequest request, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
