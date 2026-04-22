using System;
using Heroes.Content.Buildings;
using Heroes.Content.Buildings.UpgradeEffects;
using Heroes.Game.Buildings;
using Heroes.Game.Core;
using NUnit.Framework;
using UnityEngine;

namespace Heroes.Tests.Core
{
    public class BuildingUpgradeServiceTests
    {
        [Test]
        public void TryQueueSelectedUpgrade_EnqueuesUpgradeAndSpendsGold()
        {
            var selectionService = new SelectionService();
            var kingdomModel = new KingdomModel(500);
            var kingdomService = new KingdomService(kingdomModel);
            var service = new BuildingUpgradeService(selectionService, kingdomService);

            var building = CreateBuilding(Guid.NewGuid().ToString(), 100f, CreateUpgrade("upgrade-a", duration: 3f, goldCost: 125f));

            try
            {
                selectionService.Select(building);

                var result = service.TryQueueSelectedUpgrade("upgrade-a");

                Assert.IsTrue(result);
                Assert.That(building.Model.UpgradeQueue.Queue, Does.Contain("upgrade-a"));
                Assert.AreEqual(375, kingdomModel.Gold);
            }
            finally
            {
                CleanupBuilding(building);
            }
        }

        [Test]
        public void ApplyEffects_PutsBuildingBackIntoConstruction_WhenMaxHpIncreases()
        {
            var selectionService = new SelectionService();
            var kingdomModel = new KingdomModel(500);
            var kingdomService = new KingdomService(kingdomModel);
            _ = new BuildingUpgradeService(selectionService, kingdomService);
            var effect = ScriptableObject.CreateInstance<TestUpgradeEffect>();
            effect.HealthMultiplier = 2f;
            var upgrade = CreateUpgrade("upgrade-a", duration: 3f, goldCost: 125f, effect);
            var building = CreateBuilding(Guid.NewGuid().ToString(), 100f, upgrade);

            try
            {
                building.Model.SyncFromHealth();

                var applyEffects = typeof(BuildingUpgradeService).GetMethod("ApplyEffects", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                applyEffects.Invoke(null, new object[] { building, upgrade });

                Assert.AreEqual(200f, building.Model.Health.Max);
                Assert.AreEqual(BuildingState.UnderConstruction, building.Model.State);
            }
            finally
            {
                CleanupBuilding(building);
                UnityEngine.Object.DestroyImmediate(effect);
            }
        }

        [Test]
        public void TryQueueSelectedUpgrade_AllowsQueueingSameUpgradeUntilUsageLimit()
        {
            var selectionService = new SelectionService();
            var kingdomModel = new KingdomModel(500);
            var kingdomService = new KingdomService(kingdomModel);
            var service = new BuildingUpgradeService(selectionService, kingdomService);
            var upgrade = CreateUpgrade("upgrade-a", duration: 3f, goldCost: 50f);
            upgrade.UsageLimit = 3;
            var building = CreateBuilding(Guid.NewGuid().ToString(), 100f, upgrade);

            try
            {
                selectionService.Select(building);

                Assert.IsTrue(service.TryQueueSelectedUpgrade("upgrade-a"));
                Assert.IsTrue(service.TryQueueSelectedUpgrade("upgrade-a"));
                Assert.IsTrue(service.TryQueueSelectedUpgrade("upgrade-a"));
                Assert.IsFalse(service.TryQueueSelectedUpgrade("upgrade-a"));
                Assert.AreEqual(3, building.Model.UpgradeQueue.Queue.Count);
            }
            finally
            {
                CleanupBuilding(building);
            }
        }

        [Test]
        public void GetQueueLockReason_ReturnsReasonForGoldAndRequirements()
        {
            var selectionService = new SelectionService();
            var kingdomModel = new KingdomModel(50);
            var kingdomService = new KingdomService(kingdomModel);
            var service = new BuildingUpgradeService(selectionService, kingdomService);
            var requiredUpgrade = CreateUpgrade("required", duration: 1f, goldCost: 10f);
            var expensiveUpgrade = CreateUpgrade("expensive", duration: 1f, goldCost: 100f);
            expensiveUpgrade.UpgradeRequirements = new[] { requiredUpgrade };
            var building = CreateBuilding(Guid.NewGuid().ToString(), 100f, requiredUpgrade, expensiveUpgrade);

            try
            {
                Assert.AreEqual("Requires required", service.GetQueueLockReason(building, "expensive"));

                building.Model.UpgradeQueue.Enqueue(requiredUpgrade.Id, requiredUpgrade.Duration);
                building.Model.UpgradeQueue.Progress(requiredUpgrade.Duration);

                Assert.AreEqual("Not enough gold", service.GetQueueLockReason(building, "expensive"));
            }
            finally
            {
                CleanupBuilding(building);
            }
        }

        private static BuildingFacade CreateBuilding(string instanceId, float maxHp, params BuildingUpgradeDefinition[] upgrades)
        {
            var definition = ScriptableObject.CreateInstance<BuildingDefinition>();
            definition.Id = $"building-{instanceId}";
            definition.DisplayName = "Test Building";
            definition.Description = "Test";
            definition.MaxHp = maxHp;
            definition.StartHp = maxHp;
            definition.BuildHpPerSecond = 1f;
            definition.AvailableUpgrades = upgrades;

            var gameObject = new GameObject($"building-{instanceId}");
            var facade = gameObject.AddComponent<BuildingFacade>();
            facade.Initialize(definition, instanceId);
            return facade;
        }

        private static BuildingUpgradeDefinition CreateUpgrade(string id, float duration, float goldCost, BuildingUpgradeEffect effect = null, params BuildingUpgradeDefinition[] unlocks)
        {
            var upgrade = ScriptableObject.CreateInstance<BuildingUpgradeDefinition>();
            upgrade.Id = id;
            upgrade.Name = id;
            upgrade.Duration = duration;
            upgrade.GoldCost = goldCost;
            upgrade.UsageLimit = 1;
            upgrade.Effects = effect == null ? Array.Empty<BuildingUpgradeEffect>() : new[] { effect };
            upgrade.UnlocksUpgrades = unlocks ?? Array.Empty<BuildingUpgradeDefinition>();
            upgrade.UpgradeRequirements = Array.Empty<BuildingUpgradeDefinition>();
            return upgrade;
        }

        private static void CleanupBuilding(BuildingFacade building)
        {
            if (building == null)
            {
                return;
            }

            var definition = building.Definition;
            var sourceUpgrades = definition?.AvailableUpgrades;
            var upgrades = sourceUpgrades == null ? Array.Empty<BuildingUpgradeDefinition>() : Array.FindAll(sourceUpgrades, item => item != null);
            UnityEngine.Object.DestroyImmediate(building.gameObject);

            foreach (var upgrade in upgrades)
            {
                if (upgrade.UnlocksUpgrades != null)
                {
                    foreach (var unlocked in upgrade.UnlocksUpgrades)
                    {
                        if (unlocked != null)
                        {
                            UnityEngine.Object.DestroyImmediate(unlocked);
                        }
                    }
                }

                UnityEngine.Object.DestroyImmediate(upgrade);
            }

            if (definition != null)
            {
                UnityEngine.Object.DestroyImmediate(definition);
            }
        }

        private sealed class TestUpgradeEffect : BuildingUpgradeEffect
        {
            public float HealthMultiplier = 1f;

            public override void ApplyEffect(BuildingModel model)
            {
                model.Health.SetMax(model.Health.Max * HealthMultiplier);
            }
        }

    }
}
