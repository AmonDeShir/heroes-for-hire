using Heroes.GOAP.Core;

namespace Heroes.Game.AI
{
    public readonly struct GameWorldSnapshot : IReadOnlyWorldSnapshot
    {
        public int Version { get; }
        public bool IsValid { get; }
        public Locations Locations { get; }

        public GameWorldSnapshot(int version, bool isValid, Locations locations)
        {
            Version = version;
            IsValid = isValid;
            Locations = locations;
        }
    }
}


