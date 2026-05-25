using EventBus;
using Heroes.Content.Heroes;
using Heroes.Game.Abstractions;
using Heroes.Game.AI;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Health;
using Heroes.Game.Runtime;
using Heroes.Game.Combat;
using Registry;
using UnityEngine;
using UnityEngine.AI;
using Heroes.Content.Heroes.ItemEffects;

namespace Heroes.Game.Heroes
{
    public class HeroFacade : MonoBehaviour, ISelectable, IDamageable
    {
        [SerializeField] private HeroAgent heroAgent;
        [Header("Sensors")]
        [SerializeField] private HeroEnemySensor enemySensor;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private GameObject aliveVisuals;
        [SerializeField] private GameObject graveVisuals;

        private DamageLogic _damageLogic;
        private HealLogic _healLogic;
        private TimedEffectRunner _timedEffects;

        private float _regenCarry;

        public HeroDefinition Definition { get; private set; }
        public HeroModel Model { get; private set; }

        public string Id => Model?.InstanceId ?? string.Empty;
        public string Name => Definition != null ? Definition.DisplayName : string.Empty;
        public string Description => Definition != null ? Definition.Description : string.Empty;
        public string Icon => Definition != null ? Definition.IconPath : string.Empty;
        public float Health => Model != null ? Model.Health.Current : 0f;
        public float MaxHealth => Model != null ? Model.Health.Max : 0f;
        public bool IsAlive => Model != null && Model.IsAlive;

        public HeroEnemySensor EnemySensor => enemySensor;

        private IDamageable _currentCombatTarget;
        public IDamageable CurrentCombatTarget => _currentCombatTarget;

        public void Initialize(HeroDefinition definition, string instanceId, string homeBuildingInstanceId, GameWorldStateManager worldStateManager)
        {
            Definition = definition;
            Model = new HeroModel(instanceId, definition, homeBuildingInstanceId);

            _damageLogic = new DamageLogic(Model.Health);
            _healLogic = new HealLogic(Model.Health);

            _timedEffects = gameObject.GetComponent<TimedEffectRunner>();
            _timedEffects.Initialize(this, _damageLogic);

            RefreshLifeState();

            if (heroAgent != null)
            {
                heroAgent.Initialize(this, worldStateManager);
            }
        }

        private void Update()
        {
            if (Model == null || !Model.IsAlive)
            {
                return;
            }

            UpdateHomeState();

            
            if (navMeshAgent != null && Definition != null)
            {
                navMeshAgent.speed = Mathf.Max(0.1f, Definition.Speed + Model.EquipmentSpeed + Model.TimedSpeed);
            }

            TickRegeneration();
        }

        private void TickRegeneration()
        {
            if (_healLogic == null || Definition == null || Model?.Health == null)
            {
                return;
            }

            if (Model.IsInHome)
            {
                return;
            }

            if (Model.Health.Current >= Model.Health.Max)
            {
                _regenCarry = 0f;
                return;
            }

            var regenPerSec = Definition.HpRegeneration + Model.EquipmentHpRegeneration + Model.TimedHpRegeneration;
            if (regenPerSec <= 0f)
            {
                return;
            }

            _regenCarry += regenPerSec * Time.deltaTime;
            if (_regenCarry <= 0f)
            {
                return;
            }

            
            const float chunk = 0.25f;
            if (_regenCarry < chunk)
            {
                return;
            }

            var amount = Mathf.Floor(_regenCarry / chunk) * chunk;
            _regenCarry -= amount;
            _healLogic.Apply(amount);
        }

        public void ApplyDamage(float amount)
        {
            if (Model == null || !Model.IsAlive || Model.IsInHome)
            {
                return;
            }

            if (amount > 0f && CombatRuntimeConfig.Service != null)
            {
                var current = Model.Health.Current;
                if (current - amount <= 0.001f)
                {
                    CombatRuntimeConfig.Service.TryUseHealingConsumable(this);
                }
            }

            if (amount > 0f)
            {
                EventBus<HeroAttackedEvent>.Invoke(new HeroAttackedEvent
                {
                    HeroInstanceId = Model.InstanceId,
                    Position = transform.position,
                    Damage = amount,
                });
            }

            _damageLogic.Apply(amount);

            RefreshLifeState();
        }

        public void ReviveAt(Vector3 worldPosition, float hpPct)
        {
            if (Model == null)
            {
                return;
            }

            var max = Model.Health.Max;
            var hp = Mathf.Clamp(max * Mathf.Clamp01(hpPct), 0.1f, max);
            Model.Health.SetCurrent(hp);
            Model.SetInHome(false);

            if (navMeshAgent != null)
            {
                if (NavMesh.SamplePosition(worldPosition, out var hit, 6f, NavMesh.AllAreas))
                {
                    navMeshAgent.Warp(hit.position);
                }
                else
                {
                    navMeshAgent.Warp(worldPosition);
                }
            }
            else
            {
                transform.position = worldPosition;
            }

            RefreshLifeState();
            EventBus<HealthChangedEvent>.Invoke(new HealthChangedEvent { Id = Model.InstanceId, Value = hp });
        }

        public void SetCombatTarget(IDamageable target)
        {
            _currentCombatTarget = target;
        }

        public void ClearCombatTarget()
        {
            _currentCombatTarget = null;
        }

        public void AddGold(int amount)
        {
            if (Model == null)
            {
                return;
            }

            Model.SetGold(Model.Gold + amount);
        }

        public void SetGearLevel(float value)
        {
            Model?.SetGearLevel(value);
        }

        private void UpdateHomeState()
        {
            if (heroAgent == null)
            {
                return;
            }

            var isInHome = heroAgent.IsInsideHome();
            if (Model.IsInHome != isInHome)
            {
                Model.SetInHome(isInHome);
            }
        }

        private void RefreshLifeState()
        {
            var isAlive = Model != null && Model.IsAlive;
            if (!isAlive)
            {
                Model.SetInHome(false);
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = isAlive;
            }

            if (heroAgent != null)
            {
                heroAgent.enabled = isAlive;
            }

            if (aliveVisuals != null)
            {
                aliveVisuals.SetActive(isAlive);
            }

            if (graveVisuals != null)
            {
                graveVisuals.SetActive(!isAlive);
            }

            
        }

        private void OnDestroy()
        {
            Registry<HeroFacade>.Remove(this);
        }

        public void ApplyItemEffects(ItemDefinition item, ItemEffectTrigger trigger, HeroFacade target = null)
        {
            if (item == null || item.Effects == null || item.Effects.Length == 0)
            {
                return;
            }

            for (var i = 0; i < item.Effects.Length; i++)
            {
                var entry = item.Effects[i];
                if (entry.Effect == null || entry.Trigger != trigger)
                {
                    continue;
                }

                entry.Effect.Apply(new ItemEffectContext
                {
                    User = this,
                    Target = target,
                    Item = item,
                    Trigger = trigger,
                });
            }
        }
    }
}


