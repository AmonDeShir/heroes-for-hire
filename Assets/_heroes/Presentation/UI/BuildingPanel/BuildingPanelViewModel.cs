using System.Linq;
using Heroes.Content.Abstractions;
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
        private IGameEventBus _eventBus;

        [EventfulProperty] private string _selected;
        [EventfulProperty] private BuildingDTO[] _buildings = System.Array.Empty<BuildingDTO>();
        
        [Inject]
        public void Construct(IBuildingCatalog buildingCatalog, IGameEventBus eventBus)
        {
            _buildingCatalog = buildingCatalog;
            _eventBus = eventBus;
            
            OnSelectedChanged += HandleSelection;
            Refresh();
        }

        private void OnDestroy()
        {
            OnSelectedChanged -= HandleSelection;
        }

        private void HandleSelection(string value)
        {
            _eventBus.Publish(new BuildingSelectionChangedEvent(value));
        }

        private void Refresh()
        {
            // TODO - Replace LINQ with a more efficient lib
            Buildings = _buildingCatalog.GetAll().Select(x => new BuildingDTO(x)).ToArray();
        }
    }
}
