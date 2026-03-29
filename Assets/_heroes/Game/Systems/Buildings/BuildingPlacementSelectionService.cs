using System;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;

namespace Heroes.Game.Systems.Buildings
{
    public class BuildingPlacementSelectionService : IBuildingPlacementSelectionService
    {
        private readonly IGameEventBus _eventBus;

        public string SelectedBuildingDefinitionId { get; private set; }

        public event Action<string> OnSelectedChanged;

        public BuildingPlacementSelectionService(IGameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Select(string buildingDefinitionId)
        {
            SelectedBuildingDefinitionId = buildingDefinitionId;
            _eventBus.Publish(new BuildingSelectionChangedEvent(buildingDefinitionId));
            OnSelectedChanged?.Invoke(SelectedBuildingDefinitionId);
        }

        public void Clear()
        {
            SelectedBuildingDefinitionId = null;
            _eventBus.Publish(new BuildingSelectionChangedEvent(null));
            OnSelectedChanged?.Invoke(null);
        }
    }
}
