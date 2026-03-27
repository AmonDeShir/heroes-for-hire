
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;

namespace Heroes.Presentation.World
{
    public class BuildingWorldPresenter
    {
        private readonly IGameEventBus _eventBus;

        public BuildingWorldPresenter(IGameEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<BuildingPlacedEvent>(OnBuildingPlaced);
        }

        private void OnBuildingPlaced(BuildingPlacedEvent evt)
        {
            var prefab = evt.Building.Definition.Prefab;
            var pos = evt.Building.Position;
        }
    }
}