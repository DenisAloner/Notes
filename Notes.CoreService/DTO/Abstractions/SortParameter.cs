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