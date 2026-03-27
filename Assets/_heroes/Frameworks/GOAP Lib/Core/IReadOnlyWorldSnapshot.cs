namespace Heroes.GOAP.Core
{
    public interface IReadOnlyWorldSnapshot
    {
        int Version { get; }
        bool IsValid { get; }
    }
}
