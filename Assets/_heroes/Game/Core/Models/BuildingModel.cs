using System.Collections.Generic;
using System;
using Heroes.Game.Core.Models;
using EventBus;
using Heroes.Game.Core.Events;

namespace Heroes.Game.Buildings
{
    public sealed class BuildingModel
    {
        public string InstanceId { get; }
        public string DefinitionId { get; }

        public Core.Health.HealthModel Health { get; }
        public float MaxHp => Health.Max;
        public int ConstructionStage { get; private set; }
        public BuildingState State { get; private set; }
        public bool IsCompleted { get; private set; }

        public float GoldIncomeMultiplier { get; private set; }

        public int PopulationProvided { get; private set; }

        public QueueModel UpgradeQueue { get; private set; }

        private readonly HashSet<string> _unlockedSellItems = new();

        public BuildingModel(string instanceId, string definitionId, List<string> upgrades, float maxHp, float startHp)
        {
            InstanceId = instanceId;
            DefinitionId = definitionId;

            Health = new Core.Health.HealthModel(InstanceId, maxHp, startHp);
            ConstructionStage = 0;
            State = BuildingState.UnderConstruction;

            UpgradeQueue = new QueueModel(upgrades);
            GoldIncomeMultiplier = 1f;
            PopulationProvided = 0;
        }

        public void MultiplyGoldIncome(float multiplier)
        {
            if (multiplier <= 0f || float.IsNaN(multiplier) || float.IsInfinity(multiplier))
            {
                return;
            }

            GoldIncomeMultiplier *= multiplier;
            if (GoldIncomeMultiplier < 0.01f)
            {
                GoldIncomeMultiplier = 0.01f;
            }
        }

        public void SetPopulationProvided(int value)
        {
            PopulationProvided = value < 0 ? 0 : value;
        }

        public bool IsSellItemUnlocked(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && _unlockedSellItems.Contains(itemId);
        }

        public void UnlockSellItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return;
            }

            _unlockedSellItems.Add(itemId);
        }

        public IReadOnlyCollection<string> GetUnlockedSellItems()
        {
            return _unlockedSellItems;
        }

        

        public void SyncFromHealth()
        {
            var hp = Health.Current;
            var max = Health.Max;

            ConstructionStage = CalculateStage(hp, max);

            if (max > 0f && hp <= 0f)
            {
                State = BuildingState.Destroyed;
            }
            else if (max > 0f && hp >= max)
            {
                State = BuildingState.Completed;
                IsCompleted = true;
            }
            else
            {
                State = IsCompleted ? BuildingState.Damaged : BuildingState.UnderConstruction;
            }
        }

        public void RestartConstruction()
        {
            IsCompleted = false;
            SyncFromHealth();
        }

        private static int CalculateStage(float hp, float maxHp)
        {
            if (maxHp <= 0f)
            {
                return 0;
            }

            var normalized = hp / maxHp;
            
            if (normalized < 0f)
            {
                normalized = 0f;
            }

            if (normalized > 1f)
            {
                normalized = 1f;
            }

            return (int)System.Math.Round(normalized * 10f);
        }
    }
}


