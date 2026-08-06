using Humanas.Profile.Api.Dtos;
using Humanas.Profile.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Humanas.Profile.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public sealed class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProfile(
        [FromBody] CreateProfileRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _profileService.CreateAsync(request, cancellationToken);
            return Created(response.ShareUrl, response);
        }
        catch (ProfileValidationException ex)
        {
            foreach (var (key, messages) in ex.Errors)
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError(key, message);
                }
            }

            return ValidationProblem(ModelState);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var profileId))
        {
            return NotFound(new ProblemDetails
            {
                Title = "Profile not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        var profile = await _profileService.GetByIdAsync(profileId, cancellationToken);
        if (profile is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Profile not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(profile);
    }
}
