namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// An exception that is thrown when a saga is locked.
/// </summary>
public class SagaLockedException : Exception
{
    private const string ExceptionMessage = "The saga is locked.";
    
    /// <summary>
    /// Constructor.
    /// </summary>
    public SagaLockedException() : base(ExceptionMessage) { }
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SagaLockedException(string message) : base(message) { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public SagaLockedException(string message, Exception inner) : base(message, inner) { }
}