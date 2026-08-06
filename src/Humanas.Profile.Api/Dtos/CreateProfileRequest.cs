using System.ComponentModel.DataAnnotations;

namespace Humanas.Profile.Api.Dtos;

public sealed class CreateProfileRequest : IValidatableObject
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name must be at most 100 characters.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Headline is required.")]
    [MaxLength(200, ErrorMessage = "Headline must be at most 200 characters.")]
    public string Headline { get; init; } = string.Empty;

    [Required(ErrorMessage = "Skills are required.")]
    [MinLength(1, ErrorMessage = "At least one skill is required.")]
    [MaxLength(50, ErrorMessage = "At most 50 skills are allowed.")]
    public List<string> Skills { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Skills.Count == 0)
        {
            yield return new ValidationResult(
                "At least one skill is required.",
                [nameof(Skills)]);
            yield break;
        }

        for (var i = 0; i < Skills.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(Skills[i]))
            {
                yield return new ValidationResult(
                    "Skills cannot contain empty items.",
                    [$"{nameof(Skills)}[{i}]"]);
            }
            else if (Skills[i].Trim().Length > 50)
            {
                yield return new ValidationResult(
                    "Each skill must be at most 50 characters.",
                    [$"{nameof(Skills)}[{i}]"]);
            }
        }
    }
}
