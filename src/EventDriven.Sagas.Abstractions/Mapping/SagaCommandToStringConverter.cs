using System.Text.Json;
using AutoMapper;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Mapping;

/// <summary>
/// Saga command to string converter.
/// </summary>
public class SagaCommandToStringConverter : ITypeConverter<SagaCommand, string>
{
    /// <inheritdoc />
    public string Convert(SagaCommand source, string destination, ResolutionContext context) => 
        JsonSerializer.Serialize(source, source.GetType());
}