using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;
using Heroes.Game.Domain.Buildings;
using Heroes.Game.Domain.Resources;
using UnityEngine;
using EntityId = Heroes.Game.Core.EntityId;

namespace Heroes.Game.Systems.Buildings
{
    public class BuildingSystem : IBuildingSystem, IBuildingPlacementService
    {
        private readonly IBuildingCatalog _buildingCatalog;
        private readonly IBuildingPlacementSelectionService _selection;
        private readonly KingdomResources _resources;
        private readonly IGameEventBus _eventBus;

        private readonly List<Building> _buildings = new();
        private int _nextId = 1;

        public IReadOnlyList<Building> Buildings => _buildings;

        public BuildingSystem(
            IBuildingCatalog buildingCatalog,
            IBuildingPlacementSelectionService selection,
            KingdomResources resources,
            IGameEventBus eventBus)
        {
            _buildingCatalog = buildingCatalog;
            _selection = selection;
            _resources = resources;
            _eventBus = eventBus;
        }

        public bool TryPlaceSelectedBuilding(Vector2 position)
        {
            var selectedId = _selection.SelectedBuildingDefinitionId;
            
            if (string.IsNullOrWhiteSpace(selectedId))
            {
                return false;
            }

            var definition = _buildingCatalog.GetById(selectedId);
            
            if (definition == null)
            {
                return false;
            }

            if (!_resources.TrySpendGold(definition.GoldCost))
            {
                return false;
            }

            var building = new Building(
                new EntityId(_nextId++),
                definition,
                position,
                true);
            _buildings.Add(building);

            _eventBus.Publish(new ResourcesChangedEvent(_resources.Gold));
            _eventBus.Publish(new BuildingPlacedEvent(building));

            return true;
        }

        public bool HasBuildingInCategory(Content.Definitions.Buildings.BuildingCategory category)
        {
            foreach (var building in _buildings)
            {
                if (building.Category == category)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
