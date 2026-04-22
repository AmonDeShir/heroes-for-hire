using System.Collections.Generic;
using Heroes.Game.Core.Models;

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

        public QueueModel UpgradeQueue { get; private set; }

        public BuildingModel(string instanceId, string definitionId, List<string> upgrades, float maxHp, float startHp)
        {
            InstanceId = instanceId;
            DefinitionId = definitionId;

            Health = new Core.Health.HealthModel(InstanceId, maxHp, startHp);
            ConstructionStage = 0;
            State = BuildingState.UnderConstruction;

            UpgradeQueue = new QueueModel(upgrades);
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
