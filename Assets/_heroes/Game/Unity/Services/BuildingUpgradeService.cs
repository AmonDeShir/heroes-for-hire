using System.Collections.Generic;
using System.Linq;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Content.Buildings.UpgradeEffects;
using Heroes.Game.Core.Events;
using Heroes.Game.Heroes;
using Registry;

namespace Heroes.Game.Buildings
{
    public class BuildingUpgradeService
    {
        private readonly SelectionService _selectionService;
        private readonly KingdomService _kingdomService;
        private readonly HeroSpawnService _heroSpawnService;
        private readonly EventBinding<UpgradeQueueUpgradeCompletedEvent> _upgradeCompletedEvent;

        public BuildingUpgradeService(SelectionService selectionService, KingdomService kingdomService, HeroSpawnService heroSpawnService = null)
        {
            _selectionService = selectionService;
            _kingdomService = kingdomService;
            _heroSpawnService = heroSpawnService;

            _upgradeCompletedEvent = new EventBinding<UpgradeQueueUpgradeCompletedEvent>(HandleUpgradeCompleted);
            EventBus<UpgradeQueueUpgradeCompletedEvent>.Register(_upgradeCompletedEvent);
        }

        public bool TryQueueSelectedUpgrade(string upgradeId)
        {
            if (_selectionService.Selected is not BuildingFacade building)
            {
                return false;
            }

            return TryQueueUpgrade(building, upgradeId);
        }

        public bool TryQueueUpgrade(string buildingId, string upgradeId)
        {
            var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(item => item != null && item.Id == buildingId));
            return TryQueueUpgrade(building, upgradeId);
        }

        public bool TryQueueUpgrade(BuildingFacade building, string upgradeId)
        {
            if (building == null || building.Model == null || building.Definition == null || string.IsNullOrWhiteSpace(upgradeId))
            {
                return false;
            }

            var upgrade = GetUpgradeDefinition(building.Definition, upgradeId);
            if (upgrade == null || !CanQueueUpgrade(building, upgrade))
            {
                return false;
            }

            if (!_kingdomService.TrySpendGold((int)upgrade.GoldCost))
            {
                return false;
            }

            var canRepeat = upgrade.UsageLimit > 1;
            building.Model.UpgradeQueue.Enqueue(upgrade.Id, upgrade.Duration, canRepeat);
            return true;
        }

        public bool CanQueueSelectedUpgrade(string upgradeId)
        {
            return _selectionService.Selected is BuildingFacade building && CanQueueUpgrade(building, upgradeId);
        }

        public bool CanQueueUpgrade(BuildingFacade building, string upgradeId)
        {
            var upgrade = building?.Definition != null ? GetUpgradeDefinition(building.Definition, upgradeId) : null;
            return upgrade != null && CanQueueUpgrade(building, upgrade);
        }

        public string GetQueueLockReason(BuildingFacade building, string upgradeId)
        {
            var upgrade = building?.Definition != null ? GetUpgradeDefinition(building.Definition, upgradeId) : null;
            return upgrade == null ? "Unavailable" : GetQueueLockReason(building, upgrade);
        }

        private bool CanQueueUpgrade(BuildingFacade building, BuildingUpgradeDefinition upgrade)
        {
            return string.IsNullOrEmpty(GetQueueLockReason(building, upgrade));
        }

        private string GetQueueLockReason(BuildingFacade building, BuildingUpgradeDefinition upgrade)
        {
            if (building == null || building.Model == null || upgrade == null)
            {
                return "Unavailable";
            }

            if (!building.Model.UpgradeQueue.Available.Contains(upgrade.Id))
            {
                return "Locked";
            }

            var requirementReason = GetRequirementsLockReason(building, upgrade);
            if (!string.IsNullOrEmpty(requirementReason))
            {
                return requirementReason;
            }

            if (upgrade.GoldCost > _kingdomService.Gold)
            {
                return "Not enough gold";
            }

            if (upgrade.UsageLimit <= 0)
            {
                return "Unavailable";
            }

            return GetReservedCount(building, upgrade.Id) >= upgrade.UsageLimit ? "Usage limit reached" : string.Empty;
        }

        private string GetRequirementsLockReason(BuildingFacade building, BuildingUpgradeDefinition upgrade)
        {
            if (upgrade.UpgradeRequirements == null || upgrade.UpgradeRequirements.Length == 0)
            {
                return string.Empty;
            }

            var missingRequirements = new List<string>();

            foreach (var requirement in upgrade.UpgradeRequirements)
            {
                if (requirement == null)
                {
                    continue;
                }

                if (building.Model.UpgradeQueue.GetCompletedCount(requirement.Id) <= 0)
                {
                    missingRequirements.Add(requirement.Name);
                }
            }

            return missingRequirements.Count > 0
                ? $"Requires {string.Join(", ", missingRequirements)}"
                : string.Empty;
        }

        private int GetReservedCount(BuildingFacade building, string upgradeId)
        {
            if (building?.Model == null || string.IsNullOrWhiteSpace(upgradeId))
            {
                return 0;
            }

            var completedCount = building.Model.UpgradeQueue.GetCompletedCount(upgradeId);
            var queuedCount = building.Model.UpgradeQueue.Queue.Count(item => item == upgradeId);
            var activeCount = building.Model.UpgradeQueue.active == upgradeId ? 1 : 0;
            return completedCount + queuedCount + activeCount;
        }

        private void HandleUpgradeCompleted(UpgradeQueueUpgradeCompletedEvent @event)
        {
            if (string.IsNullOrWhiteSpace(@event.Id) || string.IsNullOrWhiteSpace(@event.Value))
            {
                return;
            }

            var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(item => item != null && item.Id == @event.Id));
            if (building?.Definition == null || building.Model == null)
            {
                return;
            }

            var upgrade = GetUpgradeDefinition(building.Definition, @event.Value);
            if (upgrade == null)
            {
                return;
            }

            ApplyEffects(building, upgrade);
            SpawnHeroes(building, upgrade);
            UnlockUpgrades(building, upgrade);
            building.Model.SyncFromHealth();
        }

        private void SpawnHeroes(BuildingFacade building, BuildingUpgradeDefinition upgrade)
        {
            if (_heroSpawnService == null || upgrade?.Effects == null)
            {
                return;
            }

            foreach (var effect in upgrade.Effects)
            {
                if (effect is not SpawnHeroEffect spawnEffect || spawnEffect.Hero == null)
                {
                    continue;
                }

                var count = spawnEffect.Count <= 0 ? 1 : spawnEffect.Count;
                for (var i = 0; i < count; i++)
                {
                    _heroSpawnService.Spawn(spawnEffect.Hero, building);
                }
            }
        }

        private static void ApplyEffects(BuildingFacade building, BuildingUpgradeDefinition upgrade)
        {
            if (upgrade.Effects == null)
            {
                return;
            }

            var previousMaxHp = building.Model.Health.Max;

            foreach (var effect in upgrade.Effects)
            {
                if (effect == null)
                {
                    continue;
                }

                effect.ApplyEffect(building.Model);
            }

            if (building.Model.Health.Max > previousMaxHp && building.Model.Health.Current < building.Model.Health.Max)
            {
                building.Model.RestartConstruction();
            }

            if (!UnityEngine.Mathf.Approximately(previousMaxHp, building.Model.Health.Max))
            {
                EventBus<MaxHealthChangedEvent>.Invoke(new MaxHealthChangedEvent
                {
                    Id = building.Model.InstanceId,
                    Value = building.Model.Health.Max
                });
            }
        }

        private static void UnlockUpgrades(BuildingFacade building, BuildingUpgradeDefinition upgrade)
        {
            if (upgrade.UnlocksUpgrades == null || upgrade.UnlocksUpgrades.Length == 0)
            {
                return;
            }

            var unlockIds = new List<string>();
            foreach (var unlockedUpgrade in upgrade.UnlocksUpgrades)
            {
                if (unlockedUpgrade == null || string.IsNullOrWhiteSpace(unlockedUpgrade.Id))
                {
                    continue;
                }

                unlockIds.Add(unlockedUpgrade.Id);
            }

            if (unlockIds.Count > 0)
            {
                building.Model.UpgradeQueue.MakeAvailable(unlockIds);
            }
        }

        private static BuildingUpgradeDefinition GetUpgradeDefinition(BuildingDefinition definition, string upgradeId)
        {
            if (definition == null || string.IsNullOrWhiteSpace(upgradeId))
            {
                return null;
            }

            foreach (var upgrade in EnumerateUpgrades(definition.AvailableUpgrades))
            {
                if (upgrade != null && upgrade.Id == upgradeId)
                {
                    return upgrade;
                }
            }

            return null;
        }

        private static IEnumerable<BuildingUpgradeDefinition> EnumerateUpgrades(IEnumerable<BuildingUpgradeDefinition> upgrades)
        {
            if (upgrades == null)
            {
                yield break;
            }

            var visited = new HashSet<string>();
            var stack = new Stack<BuildingUpgradeDefinition>(upgrades.Where(item => item != null));

            while (stack.Count > 0)
            {
                var upgrade = stack.Pop();
                if (upgrade == null || !visited.Add(upgrade.Id))
                {
                    continue;
                }

                yield return upgrade;

                if (upgrade.UnlocksUpgrades == null)
                {
                    continue;
                }

                foreach (var unlockedUpgrade in upgrade.UnlocksUpgrades)
                {
                    if (unlockedUpgrade != null)
                    {
                        stack.Push(unlockedUpgrade);
                    }
                }
            }
        }
    }
}
