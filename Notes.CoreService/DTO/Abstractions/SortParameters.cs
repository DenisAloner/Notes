using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Notes.CoreService.DTO.Abstractions;

public class SortParameters<T>
    where T : struct, Enum
{
    public readonly IReadOnlyList<SortParameter<T>> Values;

    private SortParameters(IReadOnlyList<SortParameter<T>> values)
    {
        Values = values;
    }


    public static string? TryParse(string input,out SortParameters<T> result)
    {
        var parts = input.Split(',');
        if (!parts.Any()) throw new Exception("Invalid format");
        var values = new List<SortParameter<T>>(); 
        foreach (var part in parts)
        {
            var message = SortParameter<T>.TryParse(part, out var value);
            if (message == null)
            {
                values.Add(value);
            }
            else
            {
                result = default;
                return message;
            }
        }

        result = new SortParameters<T>(values);
        return null;
    }
}

public class SortParametersConverterModelBinder<T> : IModelBinder where T : struct, Enum
{ 
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;

        // Try to fetch the value of the argument by name
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var message = SortParameters<T>.TryParse(value, out var model);
        if (message!=null)
        {
            // Non-integer arguments result in model state errors
            bindingContext.ModelState.TryAddModelError(
                modelName, message);

            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}