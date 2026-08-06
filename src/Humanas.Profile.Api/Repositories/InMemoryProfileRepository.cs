using System.Collections.Concurrent;
using Humanas.Profile.Api.Models;

namespace Humanas.Profile.Api.Repositories;

public sealed class InMemoryProfileRepository : IProfileRepository
{
    private readonly ConcurrentDictionary<Guid, UserProfile> _profiles = new();

    public Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        _profiles[profile.Id] = profile;
        return Task.CompletedTask;
    }

    public Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _profiles.TryGetValue(id, out var profile);
        return Task.FromResult(profile);
    }
}
