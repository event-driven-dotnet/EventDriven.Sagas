using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestClient.Serialization;

public class InterfaceConverter<TImplementation, TInterface> : JsonConverter<TInterface>
    where TImplementation : class, TInterface
{
    public override TInterface? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<TImplementation>(ref reader, options);

    public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value as TImplementation, options);
}