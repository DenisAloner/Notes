using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notes.CoreService.DataAccess;
using Notes.CoreService.DataAccess.Entities;
using Notes.CoreService.Extensions;
using Notes.CoreService.Middlewares;
using Notes.CoreService.Options;
using Notes.CoreService.Repositories;
using Notes.CoreService.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Singleton)
        .RegisterTypeMapping()
        .RegisterOptions<CoreServiceOptions, CoreServiceOptionsValidator>(builder.Configuration)
        .RegisterDbContextFactory(Constants.Schema)
        .AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        })
        .AddSingleton<IRepository<Note>, NoteRepository>()
        .RegisterSwagger(builder.Configuration)
        ;

    builder.Services
        .AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        })
        .AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new OptionalStringJsonConverter());
        });

    //builder.Services.AddCors();

    builder.Services
        .AddSingleton<IClaimsTransformation, KeyCloakClaimsTransformation>()
        .ConfigureOptions<ConfigureJwtBearerOptions>()
        .AddAuthentication()
        .AddJwtBearer();


    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressInferBindingSourcesForParameters = true;
    });

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
    );

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var dbContext = factory.CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        var clientId = app.Services.GetRequiredService<IOptions<KeyCloakOptions>>().Value.ClientId;
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.OAuthClientId(clientId);
            options.OAuthUsePkce();
        });
    }

    app.UseMiddleware<ErrorHandlerMiddleware>();

    //app.UseCors(corsPolicyBuilder => corsPolicyBuilder.WithOrigins("localhost")
    //    .AllowAnyHeader()
    //    .AllowAnyMethod()
    //    .AllowCredentials()
    //);

    app
        .UseAuthentication()
        .UseAuthorization();

    app.MapControllers();

    Log.Information("Starting web application");

    await app.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}