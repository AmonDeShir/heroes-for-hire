using Heroes.GOAP.Core;

namespace Heroes.Game.AI
{
    public class GameWorldState : WorldStateBase<GameWorldSnapshot>
    {
        private readonly Locations _locations = new();

        public override GameWorldSnapshot CreateSnapshot()
        {
            return new GameWorldSnapshot(Version, true, _locations.Clone());
        }

        public void RegisterLocation(Location location)
        {
            _locations.RegisterLocation(location);
            BumpVersion();
        }

        public bool RemoveLocation(string definitionId, string id)
        {
            var removed = _locations.RemoveLocation(definitionId, id);
            if (removed)
            {
                BumpVersion();
            }

            return removed;
        }
    }
}
