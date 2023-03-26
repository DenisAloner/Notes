using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notes.CoreService.DataAccess;
using Notes.CoreService.Options;

namespace Notes.CoreService.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDbContextFactory(
        this IServiceCollection services,
        string schema
    )
    {
        services.AddPooledDbContextFactory<ApplicationDbContext>(
            (provider, options) =>
            {
                var coreServiceOptions = provider.GetRequiredService<IOptions<CoreServiceOptions>>().Value;
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                options
                    .UseNpgsql(coreServiceOptions.ConnectionString,
                        builder => { builder.MigrationsHistoryTable("migrations_history", schema); })
                    .UseLoggerFactory(loggerFactory)
                    .UseSnakeCaseNamingConvention();
            });

        LinqToDBForEFTools.Initialize();

        return services;
    }
}