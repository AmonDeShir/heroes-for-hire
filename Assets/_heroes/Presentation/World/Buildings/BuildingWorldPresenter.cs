using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.World
{
    public class BuildingWorldPresenter : MonoBehaviour
    {
        private IGameEventBus _eventBus;

        [Inject]
        public void Construct(IGameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            _eventBus.Subscribe<BuildingPlacedEvent>(OnBuildingPlaced);
        }

        private void OnDestroy()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<BuildingPlacedEvent>(OnBuildingPlaced);
            }
        }

        private void OnBuildingPlaced(BuildingPlacedEvent evt)
        {
            var prefab = evt.Building.Definition.Prefab;
            if (prefab == null)
            {
                return;
            }

            var position = new Vector3(evt.Building.Position.x, 0f, evt.Building.Position.y);
            
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
