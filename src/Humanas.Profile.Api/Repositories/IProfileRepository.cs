using Humanas.Profile.Api.Models;

namespace Humanas.Profile.Api.Repositories;

public interface IProfileRepository
{
    Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
