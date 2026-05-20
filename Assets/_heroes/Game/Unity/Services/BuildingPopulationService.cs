using EventBus;
using Heroes.Game.Core.Events;
using Registry;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Heroes.Game.Buildings
{
    
    public sealed class BuildingPopulationService
    {
        private KingdomService _kingdom;
        private readonly EventBinding<BuildingPlacedEvent> _placed;
        private readonly EventBinding<BuildingDestroyedEvent> _destroyed;

        private readonly HashSet<string> _appliedCosts = new();

        public BuildingPopulationService(KingdomService kingdom)
        {
            _kingdom = kingdom;

            _placed = new EventBinding<BuildingPlacedEvent>(HandlePlaced);
            _destroyed = new EventBinding<BuildingDestroyedEvent>(HandleDestroyed);

            EventBus<BuildingPlacedEvent>.Register(_placed);
            EventBus<BuildingDestroyedEvent>.Register(_destroyed);

            
            SyncExisting();
        }

        private void SyncExisting()
        {
            if (_kingdom == null)
            {
                return;
            }

            foreach (var b in Registry<BuildingFacade>.All())
            {
                if (b?.Definition == null || b.Model == null || !b.IsAlive)
                {
                    continue;
                }

                ApplyPopulationForBuilding(b);
            }
        }

        private void HandlePlaced(BuildingPlacedEvent e)
        {
            if (_kingdom == null || string.IsNullOrWhiteSpace(e.InstanceId))
            {
                return;
            }

            var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(item => item != null && item.Id == e.InstanceId));
            if (building?.Definition == null || building.Model == null)
            {
                return;
            }

            ApplyPopulationForBuilding(building);
        }

        private void HandleDestroyed(BuildingDestroyedEvent e)
        {
            if (_kingdom == null || string.IsNullOrWhiteSpace(e.InstanceId))
            {
                return;
            }

            var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(item => item != null && item.Id == e.InstanceId));
            if (building?.Definition != null)
            {
                var popCost = Mathf.Max(0, building.Definition.PopulationCost);
                if (popCost > 0 && _appliedCosts.Remove(e.InstanceId))
                {
                    _kingdom.AddPeople(popCost);
                }
            }

            _kingdom.RemovePopulationContribution(e.InstanceId);
        }

        private void ApplyPopulationForBuilding(BuildingFacade building)
        {
            if (_kingdom == null || building?.Definition == null)
            {
                return;
            }

            var popCost = Mathf.Max(0, building.Definition.PopulationCost);
            if (popCost > 0 && _appliedCosts.Add(building.Id))
            {
                _kingdom.RemovePeople(popCost);
            }

            var provided = building.Definition.PopulationProvided;
            building.Model?.SetPopulationProvided(provided);
            if (provided > 0)
            {
                _kingdom.SetPopulationContribution(building.Id, provided);
            }
        }
    }
}


