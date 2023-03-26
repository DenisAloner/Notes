using System.Text.Json.Serialization;

namespace Notes.CoreService.DTO;

public class KeyCloakRealmInformation
{
    [JsonPropertyName("public_key")]
    public required string PublicKey { get; init; }
}