using System.Collections.Generic;
using Heroes.Content.Definitions.Buildings;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;

namespace Heroes.Game.AI
{
    public interface IGoapWorldStateManager
    {
        bool HasBuilding(BuildingType type);
        int KingdomGold { get; }
    }

    public class GOAPWorldStateManager : IGoapWorldStateManager
    {
        private readonly HashSet<BuildingType> _buildings = new();

        public int KingdomGold { get; private set; }

        public GOAPWorldStateManager(IGameEventBus eventBus)
        {
            eventBus.Subscribe<BuildingPlacedEvent>(OnBuildingPlaced);
            eventBus.Subscribe<ResourcesChangedEvent>(OnResourcesChanged);
        }

        private void OnBuildingPlaced(BuildingPlacedEvent evt)
        {
            _buildings.Add(evt.Building.Type);
        }

        private void OnResourcesChanged(ResourcesChangedEvent evt)
        {
            KingdomGold = evt.Gold;
        }

        public bool HasBuilding(BuildingType type)
        {
            return _buildings.Contains(type);
        }
    }
}