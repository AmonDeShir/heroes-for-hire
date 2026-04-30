using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Registry;
using UnityEngine;

namespace Heroes.Game.AI
{
    public class GameWorldStateManager : MonoBehaviour
    {
        [SerializeField] private BuildingCatalog buildingCatalog;

        private EventBinding<BuildingPlacedEvent> _buildingPlacedEvent;
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyedEvent;

        public GameWorldState State { get; private set; }

        private void Awake()
        {
            State = new GameWorldState();
            SyncExistingBuildings();

            _buildingPlacedEvent = new EventBinding<BuildingPlacedEvent>(HandleBuildingPlaced);
            _buildingDestroyedEvent = new EventBinding<BuildingDestroyedEvent>(HandleBuildingDestroyed);

            EventBus<BuildingPlacedEvent>.Register(_buildingPlacedEvent);
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyedEvent);
        }

        private void OnDestroy()
        {
            EventBus<BuildingPlacedEvent>.Unregister(_buildingPlacedEvent);
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyedEvent);
        }

        private void SyncExistingBuildings()
        {
            foreach (var building in Registry<BuildingFacade>.All())
            {
                if (building == null || building.Definition == null)
                {
                    continue;
                }

                State.RegisterLocation(new Location
                {
                    ID = building.Id,
                    Position = new Vector2(building.transform.position.x, building.transform.position.z),
                    Definition = building.Definition,
                });
            }
        }

        private void HandleBuildingPlaced(BuildingPlacedEvent @event)
        {
            var definition = buildingCatalog != null ? buildingCatalog.GetById(@event.DefinitionId) : null;
            if (definition == null)
            {
                return;
            }

            State.RegisterLocation(new Location
            {
                ID = @event.InstanceId,
                Position = new Vector2(@event.Position.x, @event.Position.z),
                Definition = definition,
            });
        }

        private void HandleBuildingDestroyed(BuildingDestroyedEvent @event)
        {
            State.RemoveLocation(@event.DefinitionId, @event.InstanceId);
        }
    }
}
