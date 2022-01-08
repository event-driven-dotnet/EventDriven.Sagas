namespace EventDriven.Sagas.Abstractions.Mapping;

/// <inheritdoc />
public class SagaCommandTypeResolver : ISagaCommandTypeResolver
{
    private readonly string? _assemblyName;
    private const string NameIdentifier = "Name";

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="assemblyName">Name of assembly containing saga commands.</param>
    public SagaCommandTypeResolver(string? assemblyName)
    {
        _assemblyName = assemblyName;
    }

    /// <inheritdoc />
    public Type? ResolveCommandType(string source)
    {
        var len1 = $"\"{NameIdentifier}\":".Length;
        var pos = source.IndexOf(NameIdentifier, StringComparison.Ordinal);
        var start = pos + len1;
        var len2 = source.IndexOf("\"", start, StringComparison.Ordinal);
        var typeName = source.Substring(start, len2 - start);
        var fullTypeName = typeName;
        if (_assemblyName != null)
            fullTypeName = $"{typeName}, {_assemblyName}";
        var type = Type.GetType(fullTypeName, true, true);
        return type;
    }
}