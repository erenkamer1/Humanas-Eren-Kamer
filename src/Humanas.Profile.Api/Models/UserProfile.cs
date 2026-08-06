namespace Humanas.Profile.Api.Models;

public sealed class UserProfile
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Headline { get; init; }
    public required IReadOnlyList<string> Skills { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
