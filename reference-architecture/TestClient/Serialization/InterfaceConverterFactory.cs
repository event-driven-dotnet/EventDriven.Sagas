using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestClient.Serialization;

public class InterfaceConverterFactory<TImplementation, TInterface> : JsonConverterFactory
{
    public Type ImplementationType { get; }
    public Type InterfaceType { get; }

    public InterfaceConverterFactory()
    {
        ImplementationType = typeof(TImplementation);
        InterfaceType = typeof(TInterface);
    }

    public override bool CanConvert(Type typeToConvert)
        => typeToConvert == InterfaceType;

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(InterfaceConverter<,>).MakeGenericType(ImplementationType, InterfaceType);
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}