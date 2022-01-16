using EventDriven.Sagas.Abstractions;

namespace OrderService.Helpers;

public static class SagaLockedExceptionExtensions
{
    public static Dictionary<string, string[]> ToErrors(this SagaLockedException error)
    {
        var errors = new Dictionary<string, string[]>
        {
            { nameof(SagaLockedException), new []{ error.Message } }
        };
        return errors;
    }
}
