namespace Heroes.GOAP.Core
{
    public interface IWorldState<TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        TSnapshot CreateSnapshot();
    }
}


