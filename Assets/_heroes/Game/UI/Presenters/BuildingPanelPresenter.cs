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
        private KingdomService _kingdom;
        private EventBinding<BuildingPlacementSelectedChangedEvent> _buildingPlacementSelectedChangedEvent;
        private EventBinding<UnlockedBuildingsChangedEvent> _unlockedBuildingsChangedEvent;

        [EventfulProperty] private string _selected;
        [EventfulProperty] private BuildingDTO[] _buildings = System.Array.Empty<BuildingDTO>();

        [Inject]
        public void Construct(BuildingCatalog buildingCatalog, BuildingPlacementSelectionService buildingPlacementSelectionService, KingdomService kingdom)
        {
            _buildingCatalog = buildingCatalog;
            _buildingPlacementSelectionService = buildingPlacementSelectionService;
            _kingdom = kingdom;

            _buildingPlacementSelectedChangedEvent = new EventBinding<BuildingPlacementSelectedChangedEvent>(HandleSelectionEvent);
            EventBus<BuildingPlacementSelectedChangedEvent>.Register(_buildingPlacementSelectedChangedEvent);

            _unlockedBuildingsChangedEvent = new EventBinding<UnlockedBuildingsChangedEvent>(_ => Refresh());
            EventBus<UnlockedBuildingsChangedEvent>.Register(_unlockedBuildingsChangedEvent);

            Selected = _buildingPlacementSelectionService.Selected;

            Refresh();
        }

        private void OnDestroy()
        {
            EventBus<BuildingPlacementSelectedChangedEvent>.Unregister(_buildingPlacementSelectedChangedEvent);
            EventBus<UnlockedBuildingsChangedEvent>.Unregister(_unlockedBuildingsChangedEvent);
        }

        public void SelectBuilding(string buildingId)
        {
            
            var def = _buildingCatalog != null ? _buildingCatalog.GetById(buildingId) : null;
            if (def == null || !def.IsPlayerBuildable)
            {
                return;
            }

            var castleLevel = _kingdom != null ? _kingdom.CastleLevel : 1;
            if (castleLevel < Mathf.Max(1, def.RequiredCastleLevel))
            {
                return;
            }

            if (_kingdom != null && _kingdom.Population < Mathf.Max(0, def.PopulationCost))
            {
                return;
            }

            if (_kingdom != null && !_kingdom.CanAfford(def.GoldCost))
            {
                return;
            }

            _buildingPlacementSelectionService.Select(buildingId);
        }

        private void HandleSelectionEvent(BuildingPlacementSelectedChangedEvent @event)
        {
            Selected = @event.Value;
        }

        private void Refresh()
        {
            var defs = _buildingCatalog.GetAll();

            var items = new System.Collections.Generic.List<BuildingDTO>();

            var castleLevel = _kingdom != null ? _kingdom.CastleLevel : 1;
            for (var i = 0; i < defs.Count; i++)
            {
                var def = defs[i];
                if (def == null || !def.IsPlayerBuildable)
                {
                    continue;
                }

                var lockReason = string.Empty;
                if (castleLevel < Mathf.Max(1, def.RequiredCastleLevel))
                {
                    lockReason = $"Requires Castle Lv {Mathf.Max(1, def.RequiredCastleLevel)}";
                }
                else if (_kingdom != null && _kingdom.Population < Mathf.Max(0, def.PopulationCost))
                {
                    lockReason = "Not enough population";
                }
                else if (_kingdom != null && !_kingdom.CanAfford(def.GoldCost))
                {
                    lockReason = "Not enough gold";
                }

                var canBuild = string.IsNullOrEmpty(lockReason);
                items.Add(new BuildingDTO(def, canBuild, lockReason));
            }

            Buildings = items.ToArray();
        }
    }
}


