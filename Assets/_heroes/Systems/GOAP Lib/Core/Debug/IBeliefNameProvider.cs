namespace Heroes.GOAP.Core.Debug
{
    public interface IBeliefNameProvider
    {
        bool TryGetBeliefName(int index, out string name);
    }
}
