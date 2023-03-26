using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Notes.CoreService.Options;
using Notes.CoreService.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

namespace Notes.CoreService.Extensions;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterSwagger(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .RegisterOptions<KeyCloakOptions, KeyCloakOptionsValidator>(configuration);

        services.AddOptions<SwaggerGenOptions>().Configure<IOptions<KeyCloakOptions>>(
            (options, keyCloakOptions) =>
            {
                var host = keyCloakOptions.Value.Host;
                var realm = keyCloakOptions.Value.Realm;
                const string schemeName = "OAuth2";

                options.AddSecurityDefinition(schemeName, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{host}realms/{realm}/protocol/openid-connect/auth"),
                            TokenUrl = new Uri($"{host}realms/{realm}/protocol/openid-connect/token")
                        }
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = schemeName
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);

                options.OperationFilter<ErrorOperationFilter>();
                options.DescribeAllParametersInCamelCase();
            });


        services
            .AddSwaggerGen()
            .AddFluentValidationRulesToSwagger();

        return services;
    }
}