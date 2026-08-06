namespace Humanas.Profile.Api.Services;

public sealed class ProfileValidationException : Exception
{
    public ProfileValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; }
}
