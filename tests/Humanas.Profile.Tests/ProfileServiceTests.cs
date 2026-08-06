using Humanas.Profile.Api.Dtos;
using Humanas.Profile.Api.Repositories;
using Humanas.Profile.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Humanas.Profile.Tests;

public class ProfileServiceTests
{
    private static ProfileService CreateService(IProfileRepository? repository = null)
    {
        return new ProfileService(
            repository ?? new InMemoryProfileRepository(),
            NullLogger<ProfileService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsIdAndShareUrl()
    {
        var service = CreateService();
        var request = new CreateProfileRequest
        {
            Name = "Eren Kamer",
            Headline = "Full Stack Software Developer",
            Skills = ["C#", ".NET", "React"]
        };

        var response = await service.CreateAsync(request);

        Assert.False(string.IsNullOrWhiteSpace(response.Id));
        Assert.True(Guid.TryParse(response.Id, out _));
        Assert.Equal($"/api/profiles/{response.Id}", response.ShareUrl);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingProfile_ReturnsProfile()
    {
        var repository = new InMemoryProfileRepository();
        var service = CreateService(repository);
        var createResponse = await service.CreateAsync(new CreateProfileRequest
        {
            Name = "Eren Kamer",
            Headline = "Full Stack Software Developer",
            Skills = ["C#", ".NET"]
        });

        var profileId = Guid.Parse(createResponse.Id);
        var profile = await service.GetByIdAsync(profileId);

        Assert.NotNull(profile);
        Assert.Equal("Eren Kamer", profile.Name);
        Assert.Equal("Full Stack Software Developer", profile.Headline);
        Assert.Equal(["C#", ".NET"], profile.Skills);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
    {
        var service = CreateService();

        var profile = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(profile);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyName_ThrowsValidationException()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ProfileValidationException>(() =>
            service.CreateAsync(new CreateProfileRequest
            {
                Name = "   ",
                Headline = "Developer",
                Skills = ["C#"]
            }));

        Assert.Contains(nameof(CreateProfileRequest.Name), exception.Errors.Keys);
    }

    [Fact]
    public async Task CreateAsync_WithEmptySkills_ThrowsValidationException()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ProfileValidationException>(() =>
            service.CreateAsync(new CreateProfileRequest
            {
                Name = "Eren Kamer",
                Headline = "Developer",
                Skills = [" ", ""]
            }));

        Assert.Contains(nameof(CreateProfileRequest.Skills), exception.Errors.Keys);
    }
}
