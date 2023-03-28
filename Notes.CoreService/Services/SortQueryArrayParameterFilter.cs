using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Notes.CoreService.Services;

public class SortQueryArrayParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (parameter.In is not ParameterLocation.Query)
            return;

        if (parameter.Schema?.Type != "array" || !parameter.Name.Equals("sort")) return;
        
        parameter.Extensions.TryGetValue("explode", out var value);

        if (value == null)
            parameter.Extensions.Add("explode", new OpenApiBoolean(false));
    }
}