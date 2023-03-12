using System.Text.Json;
using System.Text.Json.Serialization;
using Notes.CoreService.DTO.Abstractions;

namespace Notes.CoreService.Services;

public class OptionalStringJsonConverter : JsonConverter<Optional<string>>
{
    public override Optional<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Optional<string>(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, Optional<string> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}