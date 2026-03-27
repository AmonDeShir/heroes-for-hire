namespace Heroes.GOAP.Core
{
    public abstract class WorldStateBase<TSnapshot> : IWorldState<TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        private int version;

        public int Version => version;

        protected void BumpVersion()
        {
            version++;
        }

        public abstract TSnapshot CreateSnapshot();
    }
}
