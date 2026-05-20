namespace Heroes.Game.Buildings
{
    public sealed class BuildingConstructionLogic
    {
        private readonly BuildingModel _model;
        private readonly float _buildHpPerSecond;
        private readonly Core.Health.HealLogic _heal;

        public BuildingConstructionLogic(BuildingModel model, float buildHpPerSecond)
        {
            _model = model;
            _buildHpPerSecond = buildHpPerSecond;
            _heal = new Core.Health.HealLogic(model.Health);
        }

        public void Tick(float deltaTime)
        {
            if (_model.State != BuildingState.UnderConstruction)
            {
                return;
            }

            _heal.Apply(_buildHpPerSecond * deltaTime);
            _model.SyncFromHealth();
        }
    }
}


