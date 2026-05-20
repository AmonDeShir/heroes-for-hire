namespace Heroes.GOAP.Core.Debug
{
    public interface IGoapDebugSource
    {
        bool TryGetSnapshot(out GoapDebugSnapshot snapshot);
    }
}


