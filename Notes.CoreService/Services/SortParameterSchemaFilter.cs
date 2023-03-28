using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Notes.CoreService.DTO.Abstractions;
using Notes.CoreService.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.CoreService.Services;

public class SortParameterSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsGenericType || context.Type.GetGenericTypeDefinition() != typeof(SortParameter<>)) return;
        var argumentType = context.Type.GetGenericArguments().First();
        var baseSchemaName = $"{argumentType.Name.ToCamelCase()}SortParameter";
        var values = Enum.GetNames(argumentType).Select(x => x.ToCamelCase()).ToArray();
        var baseSchema = new OpenApiSchema
        {
            Type = baseSchemaName,
            Example = new OpenApiString($"{values[0]}:asc"),
            Pattern = $"^({string.Join('|', values)}):(asc|desc)$"
        };
        context.SchemaRepository.AddDefinition(baseSchemaName, baseSchema);
        schema.Type = "string";
        schema.Reference = new OpenApiReference { Id = $"{baseSchemaName}", Type = ReferenceType.Schema };
    }
}