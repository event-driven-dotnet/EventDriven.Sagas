using EventDriven.Sagas.Persistence.Abstractions.Tests.Fakes;

namespace EventDriven.Sagas.Persistence.Abstractions.Tests.Helpers;

public class FakeMetadataEqualityComparer : IEqualityComparer<FakeMetadata>
{
    public bool Equals(FakeMetadata? x, FakeMetadata? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Name == y.Name && x.Age == y.Age;
    }

    public int GetHashCode(FakeMetadata obj) => HashCode.Combine(obj.Name, obj.Age);
}