using System;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Core.Events;
using UnityEngine;
using Registry;

namespace Heroes.Game.Buildings
{
    public sealed class BuildingPlacementService
    {
        private readonly KingdomService _kingdom;

        public BuildingPlacementService(KingdomService kingdom)
        {
            _kingdom = kingdom;
        }

        public bool CanBuild(BuildingDefinition definition)
        {
            return definition != null && definition.Prefab != null && _kingdom.CanAfford(definition.GoldCost);
        }

        public bool TryPlace(BuildingDefinition definition, Vector3 position, Quaternion rotation, out BuildingFacade building)
        {
            building = null;

            if (!CanBuild(definition))
            {
                return false;
            }
            
            if (!_kingdom.TrySpendGold(definition.GoldCost))
            {
                return false;
            }
            
            building = UnityEngine.Object.Instantiate(definition.Prefab, position, rotation);
            
            var instanceId = Guid.NewGuid().ToString();
            building.Initialize(definition, instanceId);

            Registry<BuildingFacade>.TryAdd(building);

            EventBus<BuildingPlacedEvent>.Invoke(new BuildingPlacedEvent
            {
                InstanceId = instanceId,
                DefinitionId = definition.Id,
                Position = position
            });
            
            return true;
        }
    }
}
