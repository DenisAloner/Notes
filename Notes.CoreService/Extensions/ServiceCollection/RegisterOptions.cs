using FluentValidation;

namespace Notes.CoreService.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterOptions<TOptions, TValidator>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where TOptions : class
        where TValidator : AbstractValidator<TOptions>
    {
        services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(typeof(TOptions).Name))
            .Validate<TValidator>((options, validator) =>
            {
                var result = validator.Validate(options);
                if (!result.IsValid) throw new ValidationException(result.ToString());
                return true;
            })
            .ValidateOnStart();
        return services;
    }
}