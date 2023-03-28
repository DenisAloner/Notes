using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;

namespace Notes.CoreService.Services;

public class OperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var stringOpenApiMediaType = new OpenApiMediaType
            { Schema = context.SchemaGenerator.GenerateSchema(typeof(string), context.SchemaRepository) };

        if (operation.Parameters != null || operation.RequestBody != null)
            operation.Responses.Add(HttpStatusCode.BadRequest.ToString("D"), new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        MediaTypeNames.Text.Plain,
                        stringOpenApiMediaType
                    }
                }
            });

        operation.Responses.Add(HttpStatusCode.InternalServerError.ToString("D"), new OpenApiResponse
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                {
                    MediaTypeNames.Text.Plain,
                    stringOpenApiMediaType
                }
            }
        });

        var methodName = context.ApiDescription.HttpMethod;

        if (methodName != null && methodName == HttpMethod.Patch.ToString())
            operation.Responses.Add(HttpStatusCode.NotFound.ToString("D"), new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        MediaTypeNames.Text.Plain,
                        stringOpenApiMediaType
                    }
                }
            });

        var authAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AuthorizeAttribute>() ?? Enumerable.Empty<AuthorizeAttribute>();

        var allowAnonymousAttributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<AllowAnonymousAttribute>() ?? Enumerable.Empty<AllowAnonymousAttribute>();

        if (authAttributes.Any() && !allowAnonymousAttributes.Any())
        {
            operation.Responses.Add(HttpStatusCode.Unauthorized.ToString("D"), new OpenApiResponse
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    {
                        MediaTypeNames.Text.Plain,
                        stringOpenApiMediaType
                    }
                }
            });

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = Constants.AuthSchemeName
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
}