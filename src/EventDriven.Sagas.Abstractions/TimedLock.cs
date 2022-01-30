namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Timed lock for synchronizing multi-threaded access to a code block.
/// </summary>
public class TimedLock
{
    private readonly SemaphoreSlim _toLock;

    /// <summary>
    /// Constructor
    /// </summary>
    public TimedLock()
    {
        _toLock = new SemaphoreSlim(1, 1);
    }

    /// <summary>
    /// Call with a using statement to synchronize multi-threaded access to a code block.
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a LockReleaser that releases a lock.
    /// </returns>
    /// <exception cref="TimeoutException"></exception>
    public async Task<LockReleaser> Lock(TimeSpan timeout)
    {
        if(await _toLock.WaitAsync(timeout))
        {
            return new LockReleaser(_toLock);
        }
        throw new TimeoutException();
    }

    /// <summary>
    /// Lock releaser.
    /// </summary>
    public readonly struct LockReleaser : IDisposable
    {
        private readonly SemaphoreSlim _toRelease;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="toRelease">Semaphore to release.</param>
        public LockReleaser(SemaphoreSlim toRelease)
        {
            _toRelease = toRelease;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _toRelease.Release();
        }
    }
}