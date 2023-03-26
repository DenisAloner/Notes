using FluentValidation;

namespace Notes.CoreService.Options;

public class KeyCloakOptions
{
    public required Uri Host { get; set; }
    public required string Realm { get; set; }
    public required string ClientId { get; set; }
}

public class KeyCloakOptionsValidator : AbstractValidator<KeyCloakOptions>
{
    public KeyCloakOptionsValidator()
    {
        RuleFor(p => p.Realm)
            .NotEmpty();

        RuleFor(p => p.Realm)
            .NotEmpty();

        RuleFor(p => p.ClientId)
            .NotEmpty();
    }
}