using System.Linq;
using EventBus;
using Heroes.Content.Buildings;
using Heroes.Game.Abstractions;
using Heroes.Game.Core.Logic;
using Heroes.Game.Core.Events;
using UnityEngine;
using Registry;

namespace Heroes.Game.Buildings
{
    public class BuildingFacade : MonoBehaviour, IDamageable, ISelectable
    {
        [SerializeField] private BuildingVisuals constructionVisuals;
        [SerializeField] private BuildingDestructionVisuals destructionVisuals;

        public BuildingDefinition Definition { get; private set; }
        public BuildingModel Model { get; private set; }

        public string Id => Model.InstanceId;
        public string Name => Definition.DisplayName;
        public string Description => Definition.Description;
        public string Icon => Definition.IconPath;

        public float Health => Model.Health.Current;
        public float MaxHealth => Model.MaxHp;
        
        private BuildingConstructionLogic _constructionLogic;
        private Core.Health.DamageLogic _damageLogic;
        
        private QueueLogic<
            UpgradeQueueProgressChangedEvent, 
            UpgradeQueueChangedEvent, 
            UpgradeQueueActiveChangedEvent, 
            UpgradeQueueAvailableListChangedEvent,
            UpgradeQueueUpgradeCompletedEvent
        > _upgradeQueueLogic;
        
        public bool IsAlive => Model.State != BuildingState.Destroyed;

        public void Initialize(BuildingDefinition definition, string instanceId)
        {
            var upgrades = definition.AvailableUpgrades.Select(item => item.Id).ToList();
            
            Definition = definition;
            Model = new BuildingModel(instanceId, definition.Id, upgrades, definition.MaxHp, definition.StartHp);
            _constructionLogic = new BuildingConstructionLogic(Model, definition.BuildHpPerSecond);
            _damageLogic = new Core.Health.DamageLogic(Model.Health);
            _upgradeQueueLogic = new(Model.UpgradeQueue, Model.InstanceId);
            
            Model.SyncFromHealth();

            constructionVisuals?.RefreshImmediate(Model);
            destructionVisuals?.Refresh(Model);
        }

        private void Update()
        {
            if (Model == null)
            {
                return;
            }

            var previousState = Model.State;

            TickLogic(Time.deltaTime);
            RefreshVisuals();
            PublishDestroyedEventIfNeeded(previousState);
        }

        public void ApplyDamage(float amount)
        {
            var previousState = Model.State;

            _damageLogic.Apply(amount);
            Model.SyncFromHealth();

            RefreshVisuals();
            PublishDestroyedEventIfNeeded(previousState);
        }

        private void TickLogic(float deltaTime)
        {
            _constructionLogic.Tick(deltaTime);
            _upgradeQueueLogic.Tick(deltaTime);
        }

        private void RefreshVisuals()
        {
            constructionVisuals?.Refresh(Model);
            destructionVisuals?.Refresh(Model);
        }

        private void PublishDestroyedEventIfNeeded(BuildingState previousState)
        {
            if (previousState == BuildingState.Destroyed || Model.State != BuildingState.Destroyed)
            {
                return;
            }

            EventBus<BuildingDestroyedEvent>.Invoke(new BuildingDestroyedEvent
            {
                InstanceId = Model.InstanceId,
                DefinitionId = Model.DefinitionId
            });
        }

        private void OnDestroy()
        {
            Registry<BuildingFacade>.Remove(this);
        }
    }
}
