using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Net.Mime;

namespace Notes.CoreService.Services;

public class ErrorOperationFilter : IOperationFilter
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
    }
}