using Microsoft.AspNetCore.Mvc.ModelBinding;
using Notes.CoreService.Enums;

namespace Notes.CoreService.DTO.Abstractions;

public readonly record struct SortParameter<T>(T Field, SortOrder Order)
    where T : struct, Enum
{
    public static string? TryParse(string input, out SortParameter<T> value)
    {
        var parts = input.Split(':');
        if (parts.Length != 2) throw new Exception("Invalid format");
        SortOrder order;
        var field = Enum.Parse<T>(parts[0],true);
        switch (parts[1])
        {
            case "asc":
                order = SortOrder.Asc;
                break;
            case "desc":
                order = SortOrder.Desc;
                break;
            default:
                value = default;
                return "Invalid format";
        }
        value = new SortParameter<T>(field, order);
        return null;
    }
}

public class SortParametersConverterModelBinder<T> : IModelBinder where T : struct, Enum
{
    public static string? TryParse(string input, out IReadOnlyList<SortParameter<T>> result)
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

        result = new List<SortParameter<T>>(values);
        return null;
    }

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

        var message = TryParse(value, out var model);
        if (message != null)
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