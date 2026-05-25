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
            
            
            var buildings = UnityEngine.Object.FindObjectsByType<BuildingFacade>(FindObjectsSortMode.None);

            foreach (var building in buildings)
            {
                if (building == null || building.Definition == null)
                {
                    continue;
                }

                
                Registry<BuildingFacade>.TryAdd(building);

                State.RegisterLocation(new Location
                {
                    ID = building.Id,
                    Position = new Vector2(building.DoorWorldPosition.x, building.DoorWorldPosition.z),
                    DefinitionId = building.Definition.Id,
                    Radius = EstimateRadius(building),
                });
            }
        }

        private static float EstimateRadius(BuildingFacade building)
        {
            if (building == null)
            {
                return 2f;
            }

            
            var col = building.GetComponentInChildren<Collider>();
            if (col != null)
            {
                var e = col.bounds.extents;
                return Mathf.Max(1f, Mathf.Max(e.x, e.z));
            }

            return 2f;
        }

        private void HandleBuildingPlaced(BuildingPlacedEvent @event)
        {
            
            Vector2 pos;
            BuildingFacade placed = null;
            foreach (var b in Registry.Registry<BuildingFacade>.All())
            {
                if (b != null && b.IsAlive && b.Model != null && b.Model.InstanceId == @event.InstanceId)
                {
                    placed = b;
                    break;
                }
            }

            var radius = 2f;

            if (placed != null)
            {
                var p = placed.DoorWorldPosition;
                pos = new Vector2(p.x, p.z);
                radius = EstimateRadius(placed);
            }
            else
            {
                pos = new Vector2(@event.Position.x, @event.Position.z);
            }

            State.RegisterLocation(new Location { ID = @event.InstanceId, Position = pos, DefinitionId = @event.DefinitionId, Radius = radius });
        }

        private void HandleBuildingDestroyed(BuildingDestroyedEvent @event)
        {
            State.RemoveLocation(@event.DefinitionId, @event.InstanceId);
        }
    }
}


