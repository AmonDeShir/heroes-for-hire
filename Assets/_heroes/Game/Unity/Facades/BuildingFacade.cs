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

        [Header("Navigation")]
        [Tooltip("Optional. If set, this is the preferred approach point for units (door/entrance).")]
        [SerializeField] private Transform doorPoint;

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
        private bool _destroyQueued;

        private float _nextAttackedEventAt;
        private float _lastAttackedEventHp;
        
        private QueueLogic<
            UpgradeQueueProgressChangedEvent, 
            UpgradeQueueChangedEvent, 
            UpgradeQueueActiveChangedEvent, 
            UpgradeQueueAvailableListChangedEvent,
            UpgradeQueueUpgradeCompletedEvent
        > _upgradeQueueLogic;
        
        public bool IsAlive => Model.State != BuildingState.Destroyed;

        public Vector3 DoorWorldPosition
        {
            get
            {
                if (doorPoint != null)
                {
                    return doorPoint.position;
                }

                
                var t = transform.Find("Door");
                if (t != null)
                {
                    doorPoint = t;
                    return t.position;
                }

                
                
                var col = GetComponentInChildren<Collider>();
                if (col != null)
                {
                    var b = col.bounds;
                    var forward = transform.forward;
                    var ext = Mathf.Max(b.extents.x, b.extents.z);
                    return b.center + forward * (ext + 0.25f);
                }

                return transform.position;
            }
        }

        public void Initialize(BuildingDefinition definition, string instanceId)
        {
            var upgrades = definition.AvailableUpgrades.Select(item => item.Id).ToList();
            
            Definition = definition;
            Model = new BuildingModel(instanceId, definition.Id, upgrades, definition.MaxHp, definition.StartHp);
            _constructionLogic = new BuildingConstructionLogic(Model, definition.BuildHpPerSecond);
            _damageLogic = new Core.Health.DamageLogic(Model.Health);
            _upgradeQueueLogic = new(Model.UpgradeQueue, Model.InstanceId);
            
            Model.SyncFromHealth();

            Registry<BuildingFacade>.TryAdd(this);

            constructionVisuals?.RefreshImmediate(Model);
            destructionVisuals?.Refresh(Model);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            var p = DoorWorldPosition;
            Gizmos.DrawSphere(p, 0.2f);
            Gizmos.DrawLine(transform.position, p);
        }
#endif

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

            _nextAttackedEventAt = 0f;
            _lastAttackedEventHp = Model.Health.Current;

            if (amount > 0f && previousState != BuildingState.Destroyed && Model.State != BuildingState.Destroyed)
            {
                var now = Time.unscaledTime;
                var max = Model.Health.Max;
                var hp = Model.Health.Current;
                var step = max > 0.001f ? max * 0.10f : 0f;

                var crossedStep = step > 0.001f && (_lastAttackedEventHp - hp) >= step;
                if (now >= _nextAttackedEventAt || crossedStep)
                {
                    _nextAttackedEventAt = now + 0.5f;
                    _lastAttackedEventHp = hp;

                    EventBus<BuildingAttackedEvent>.Invoke(new BuildingAttackedEvent
                    {
                        InstanceId = Model.InstanceId,
                        DefinitionId = Model.DefinitionId,
                        Position = transform.position,
                        Damage = amount,
                    });
                }
            }

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

            if (!_destroyQueued)
            {
                _destroyQueued = true;
                Destroy(gameObject, 0.25f);
            }
        }

        private void OnDestroy()
        {
            Registry<BuildingFacade>.Remove(this);
        }
    }
}


