using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using OneJS;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.UI.BuildingPanel
{
    public partial class BuildingPanelPresenter : MonoBehaviour
    {
        private BuildingCatalog _buildingCatalog;
        private BuildingPlacementSelectionService _buildingPlacementSelectionService;
        private EventBinding<BuildingPlacementSelectedChangedEvent> _buildingPlacementSelectedChangedEvent;

        [EventfulProperty] private string _selected;
        [EventfulProperty] private BuildingDTO[] _buildings = System.Array.Empty<BuildingDTO>();

        [Inject]
        public void Construct(BuildingCatalog buildingCatalog, BuildingPlacementSelectionService buildingPlacementSelectionService)
        {
            _buildingCatalog = buildingCatalog;
            _buildingPlacementSelectionService = buildingPlacementSelectionService;

            _buildingPlacementSelectedChangedEvent = new EventBinding<BuildingPlacementSelectedChangedEvent>(HandleSelectionEvent);
            EventBus<BuildingPlacementSelectedChangedEvent>.Register(_buildingPlacementSelectedChangedEvent);

            Selected = _buildingPlacementSelectionService.Selected;

            Refresh();
        }

        private void OnDestroy()
        {
            EventBus<BuildingPlacementSelectedChangedEvent>.Unregister(_buildingPlacementSelectedChangedEvent);
        }

        public void SelectBuilding(string buildingId)
        {
            _buildingPlacementSelectionService.Select(buildingId);
        }

        private void HandleSelectionEvent(BuildingPlacementSelectedChangedEvent @event)
        {
            Selected = @event.Value;
        }

        private void Refresh()
        {
            var defs = _buildingCatalog.GetAll();

            var count = defs.Count;
            var items = new BuildingDTO[count];

            for (var i = 0; i < count; i++)
            {
                items[i] = new BuildingDTO(defs[i]);
            }

            Buildings = items;
        }
    }
}
