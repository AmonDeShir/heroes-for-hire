using System.Linq;
using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;
using UnityEngine;
using VContainer;
using OneJS;

namespace Heroes.Presentation.UI.BuildingPanel
{
    public partial class BuildingPanelViewModel : MonoBehaviour
    {
        private IBuildingCatalog _buildingCatalog;
        private IBuildingPlacementSelectionService _buildingPlacementSelectionService;

        [EventfulProperty] private string _selected;
        [EventfulProperty] private BuildingDTO[] _buildings = System.Array.Empty<BuildingDTO>();
        
        [Inject]
        public void Construct(IBuildingCatalog buildingCatalog, IBuildingPlacementSelectionService buildingPlacementSelectionService)
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
            // TODO - Replace LINQ with a more efficient lib
            Buildings = _buildingCatalog.GetAll().Select(x => new BuildingDTO(x)).ToArray();
        }
    }
}
