using System;
using System.Threading.Tasks;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Core;
using Heroes.Game.Core.Events;
using UnityEngine;
using Registry;

namespace Heroes.Game.Buildings
{
    public sealed class BuildingPlacementService
    {
        private readonly KingdomModel _kingdom;

        public BuildingPlacementService(KingdomModel kingdom)
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

            var oldGold = _kingdom.Gold;

            if (!_kingdom.TrySpendGold(definition.GoldCost))
            {
                return false;
            }

            var newGold = _kingdom.Gold;
            
            building = UnityEngine.Object.Instantiate(definition.Prefab, position, rotation);
            
            var instanceId = Guid.NewGuid().ToString();
            building.Initialize(definition, instanceId);

            Registry<BuildingFacade>.TryAdd(building);

            EventBus<GoldChangedEvent>.Invoke(new GoldChangedEvent
            {
                OldValue = oldGold,
                NewValue = newGold
            });

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
