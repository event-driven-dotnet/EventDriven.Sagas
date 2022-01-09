namespace EventDriven.Sagas.Abstractions.Mapping;

/// <summary>
/// Saga command type resolver.
/// </summary>
public interface ISagaCommandTypeResolver
{
    /// <summary>
    /// Resolve saga command type.
    /// </summary>
    /// <param name="source">Stringified version of a saga command.</param>
    /// <returns></returns>
    Type? ResolveCommandType(string source);
}