using Heroes.Game.Abstractions;

namespace Heroes.Game.Core.Events
{
    public readonly struct BuildingPlacedEvent
    {
        public IBuilding Building { get; }

        public BuildingPlacedEvent(IBuilding building)
        {
            Building = building;
        }
    }
}