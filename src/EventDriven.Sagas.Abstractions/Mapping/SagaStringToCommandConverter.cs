using System.Text.Json;
using AutoMapper;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Mapping;

/// <summary>
/// String to saga command converter.
/// </summary>
public class SagaStringToCommandConverter : ITypeConverter<string, SagaCommand>
{
    private readonly ISagaCommandTypeResolver? _commandTypeResolver;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="commandTypeResolver">Command type resolver.</param>
    public SagaStringToCommandConverter(ISagaCommandTypeResolver commandTypeResolver)
    {
        _commandTypeResolver = commandTypeResolver;
    }

    /// <inheritdoc />
    public SagaCommand Convert(string source, SagaCommand destination, ResolutionContext context)
    {
        var type = _commandTypeResolver?.ResolveCommandType(source);
        if (type == null) return null!;
        return (JsonSerializer.Deserialize(source, type) as SagaCommand)!;
    }
}