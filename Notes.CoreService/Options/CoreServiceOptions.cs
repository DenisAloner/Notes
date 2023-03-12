using FluentValidation;

namespace Notes.CoreService.Options;

public class CoreServiceOptions
{
    public required string ConnectionString { get; set; }
}

public class CoreServiceOptionsValidator : AbstractValidator<CoreServiceOptions>
{
    public CoreServiceOptionsValidator()
    {
        RuleFor(p => p.ConnectionString)
            .NotEmpty();
    }
}