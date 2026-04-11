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
        private bool _wasCompleted;

        public BuildingModel(string instanceId, string definitionId, float maxHp, float startHp)
        {
            InstanceId = instanceId;
            DefinitionId = definitionId;

            Health = new Core.Health.HealthModel(maxHp, startHp);
            ConstructionStage = 0;
            State = BuildingState.UnderConstruction;
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
                _wasCompleted = true;
            }
            else
            {
                State = _wasCompleted ? BuildingState.Damaged : BuildingState.UnderConstruction;
            }
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
