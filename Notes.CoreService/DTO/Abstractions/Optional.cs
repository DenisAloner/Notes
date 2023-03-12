namespace Notes.CoreService.DTO.Abstractions;

public readonly struct Optional<T>
    where T : notnull
{
    public readonly bool HasValue;

    public readonly T? Value;

    public Optional()
    {
        HasValue = false;
        Value = default;
    }

    public Optional(T? value)
    {
        HasValue = true;
        Value = value;
    }
}