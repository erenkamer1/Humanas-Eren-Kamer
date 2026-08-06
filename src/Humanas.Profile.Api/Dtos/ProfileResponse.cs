namespace Humanas.Profile.Api.Dtos;

public sealed class ProfileResponse
{
    public required string Name { get; init; }
    public required string Headline { get; init; }
    public required IReadOnlyList<string> Skills { get; init; }
}
