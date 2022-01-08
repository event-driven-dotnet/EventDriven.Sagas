namespace EventDriven.Sagas.Abstractions.Mapping;

/// <inheritdoc />
public class SagaCommandTypeResolver : ISagaCommandTypeResolver
{
    private const string NameIdentifier = "Name";

    /// <inheritdoc />
    public Type? ResolveCommandType(string source)
    {
        var len1 = $"\"{NameIdentifier}\":".Length;
        var pos = source.IndexOf(NameIdentifier, StringComparison.Ordinal);
        var start = pos + len1;
        var len2 = source.IndexOf("\"", start, StringComparison.Ordinal);
        var name = source.Substring(start, len2 - start);
        return Type.GetType(name);
    }
}