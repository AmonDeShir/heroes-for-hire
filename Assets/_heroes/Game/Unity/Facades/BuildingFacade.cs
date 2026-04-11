using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Events;
using UnityEngine;
using Registry;

namespace Heroes.Game.Buildings
{
    public class BuildingFacade : MonoBehaviour, IDamageable
    {
        [SerializeField] private BuildingVisuals constructionVisuals;
        [SerializeField] private BuildingDestructionVisuals destructionVisuals;

        public BuildingDefinition Definition { get; private set; }
        public BuildingModel Model { get; private set; }

        private BuildingConstructionLogic _constructionLogic;
        private Core.Health.DamageLogic _damageLogic;

        public bool IsAlive => Model.State != BuildingState.Destroyed;

        public void Initialize(BuildingDefinition definition, string instanceId)
        {
            Definition = definition;
            Model = new BuildingModel(instanceId, definition.Id, definition.MaxHp, definition.StartHp);
            _constructionLogic = new BuildingConstructionLogic(Model, definition.BuildHpPerSecond);
            _damageLogic = new Core.Health.DamageLogic(Model.Health);

            Model.SyncFromHealth();

            constructionVisuals.RefreshImmediate(Model);
            destructionVisuals.Refresh(Model);
        }

        private void Update()
        {
            var previousState = Model.State;

            _constructionLogic.Tick(Time.deltaTime);

            constructionVisuals.Refresh(Model);
            destructionVisuals.Refresh(Model);

            if (previousState != BuildingState.Destroyed && Model.State == BuildingState.Destroyed)
            {
                EventBus<BuildingDestroyedEvent>.Invoke(new BuildingDestroyedEvent
                {
                    InstanceId = Model.InstanceId,
                    DefinitionId = Model.DefinitionId
                });
            }
        }

        public void ApplyDamage(float amount)
        {
            var previousState = Model.State;

            _damageLogic.Apply(amount);
            Model.SyncFromHealth();

            constructionVisuals.Refresh(Model);
            destructionVisuals.Refresh(Model);

            if (previousState != BuildingState.Destroyed && Model.State == BuildingState.Destroyed)
            {
                EventBus<BuildingDestroyedEvent>.Invoke(new BuildingDestroyedEvent
                {
                    InstanceId = Model.InstanceId,
                    DefinitionId = Model.DefinitionId
                });
            }
        }

        private void OnDestroy()
        {
            Registry<BuildingFacade>.Remove(this);
        }
    }
}
