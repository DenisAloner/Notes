using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Notes.CoreService.Options;
using Notes.CoreService.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Any;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Enums;

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
                
                options.AddSecurityDefinition(Constants.AuthSchemeName, new OpenApiSecurityScheme
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

                options.MapType(typeof(Optional<string>), () => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("string")
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath, true);
                options.ParameterFilter<SortQueryArrayParameterFilter>();
                options.OperationFilter<OperationFilter>();
                options.SchemaFilter<SortParameterSchemaFilter>();
                options.DescribeAllParametersInCamelCase();
               
            });


        services
            .AddSwaggerGen()
            .AddFluentValidationRulesToSwagger();

        return services;
    }
}