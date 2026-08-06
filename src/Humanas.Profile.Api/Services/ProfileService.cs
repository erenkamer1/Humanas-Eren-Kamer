using Humanas.Profile.Api.Dtos;
using Humanas.Profile.Api.Models;
using Humanas.Profile.Api.Repositories;

namespace Humanas.Profile.Api.Services;

public sealed class ProfileService : IProfileService
{
    private readonly IProfileRepository _repository;
    private readonly ILogger<ProfileService> _logger;

    public ProfileService(IProfileRepository repository, ILogger<ProfileService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateProfileResponse> CreateAsync(
        CreateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(request);
        Validate(normalized);

        var profile = new UserProfile
        {
            Id = Guid.NewGuid(),
            Name = normalized.Name,
            Headline = normalized.Headline,
            Skills = normalized.Skills,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAsync(profile, cancellationToken);

        _logger.LogInformation("Profile created with id {ProfileId}", profile.Id);

        var shareUrl = $"/api/profiles/{profile.Id}";
        return new CreateProfileResponse
        {
            Id = profile.Id.ToString(),
            ShareUrl = shareUrl
        };
    }

    public async Task<ProfileResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var profile = await _repository.GetByIdAsync(id, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        return new ProfileResponse
        {
            Name = profile.Name,
            Headline = profile.Headline,
            Skills = profile.Skills
        };
    }

    private static NormalizedProfileRequest Normalize(CreateProfileRequest request)
    {
        return new NormalizedProfileRequest
        {
            Name = request.Name.Trim(),
            Headline = request.Headline.Trim(),
            Skills = request.Skills
                .Select(skill => skill.Trim())
                .Where(skill => !string.IsNullOrWhiteSpace(skill))
                .ToList()
        };
    }

    private static void Validate(NormalizedProfileRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors[nameof(CreateProfileRequest.Name)] = ["Name is required."];
        }
        else if (request.Name.Length > 100)
        {
            errors[nameof(CreateProfileRequest.Name)] = ["Name must be at most 100 characters."];
        }

        if (string.IsNullOrWhiteSpace(request.Headline))
        {
            errors[nameof(CreateProfileRequest.Headline)] = ["Headline is required."];
        }
        else if (request.Headline.Length > 200)
        {
            errors[nameof(CreateProfileRequest.Headline)] = ["Headline must be at most 200 characters."];
        }

        if (request.Skills.Count == 0)
        {
            errors[nameof(CreateProfileRequest.Skills)] = ["At least one skill is required."];
        }
        else if (request.Skills.Count > 50)
        {
            errors[nameof(CreateProfileRequest.Skills)] = ["At most 50 skills are allowed."];
        }
        else if (request.Skills.Any(skill => skill.Length > 50))
        {
            errors[nameof(CreateProfileRequest.Skills)] = ["Each skill must be at most 50 characters."];
        }

        if (errors.Count > 0)
        {
            throw new ProfileValidationException(errors);
        }
    }

    private sealed class NormalizedProfileRequest
    {
        public required string Name { get; init; }
        public required string Headline { get; init; }
        public required List<string> Skills { get; init; }
    }
}
