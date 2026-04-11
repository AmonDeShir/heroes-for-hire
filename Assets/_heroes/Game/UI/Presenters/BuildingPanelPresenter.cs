using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using OneJS;
using UnityEngine;
using VContainer;

namespace Heroes.Presentation.UI.BuildingPanel
{
    public partial class BuildingPanelPresenter : MonoBehaviour
    {
        private BuildingCatalog _buildingCatalog;
        private BuildingPlacementSelectionService _buildingPlacementSelectionService;

        [EventfulProperty] private string _selected;
        [EventfulProperty] private BuildingDTO[] _buildings = System.Array.Empty<BuildingDTO>();

        [Inject]
        public void Construct(BuildingCatalog buildingCatalog, BuildingPlacementSelectionService buildingPlacementSelectionService)
        {
            _buildingCatalog = buildingCatalog;
            _buildingPlacementSelectionService = buildingPlacementSelectionService;

            _buildingPlacementSelectionService.OnSelectedChanged += HandleSelectionEvent;

            Refresh();
        }

        private void OnDestroy()
        {
            _buildingPlacementSelectionService.OnSelectedChanged -= HandleSelectionEvent;
        }

        public void SelectBuilding(string buildingId)
        {
            _buildingPlacementSelectionService.Select(buildingId);
        }

        private void HandleSelectionEvent(string value)
        {
            Selected = value;
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
