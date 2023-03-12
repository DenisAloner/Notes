using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notes.CoreService.DataAccess;
using Notes.CoreService.Options;

namespace Notes.CoreService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterOptions<TOptions, TValidator>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where TOptions : class
        where TValidator : AbstractValidator<TOptions> {
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

    public static IServiceCollection RegisterDbContextFactory(
        this IServiceCollection services
    ) {
        services.AddPooledDbContextFactory<ApplicationDbContext>(
            (provider, options) =>
            {
                var coreServiceOptions = provider.GetRequiredService<IOptions<CoreServiceOptions>>().Value;
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                options
                    .UseNpgsql(coreServiceOptions.ConnectionString,
                        builder => { builder.MigrationsHistoryTable("migrations_history", Constants.Schema); })
                    .UseLoggerFactory(loggerFactory)
                    .UseSnakeCaseNamingConvention();
            });

        return services;
    }
}