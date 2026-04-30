using EventBus;
using Heroes.Content.Heroes;
using Heroes.Game.Abstractions;
using Heroes.Game.AI;
using Heroes.Game.Buildings;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Health;
using Registry;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.Heroes
{
    public class HeroFacade : MonoBehaviour, ISelectable, IDamageable
    {
        [SerializeField] private HeroAgent heroAgent;
        [SerializeField] private HeroDangerEvaluator dangerEvaluator;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private GameObject aliveVisuals;
        [SerializeField] private GameObject graveVisuals;

        private DamageLogic _damageLogic;

        public HeroDefinition Definition { get; private set; }
        public HeroModel Model { get; private set; }

        public string Id => Model?.InstanceId ?? string.Empty;
        public string Name => Definition != null ? Definition.DisplayName : string.Empty;
        public string Description => Definition != null ? Definition.Description : string.Empty;
        public string Icon => Definition != null ? Definition.IconPath : string.Empty;
        public float Health => Model != null ? Model.Health.Current : 0f;
        public float MaxHealth => Model != null ? Model.Health.Max : 0f;
        public bool IsAlive => Model != null && Model.IsAlive;

        public void Initialize(HeroDefinition definition, string instanceId, string homeBuildingInstanceId, GameWorldStateManager worldStateManager)
        {
            Definition = definition;
            Model = new HeroModel(instanceId, definition, homeBuildingInstanceId);
            _damageLogic = new DamageLogic(Model.Health);
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
            if (dangerEvaluator != null)
            {
                Model.SetDangerLevel(dangerEvaluator.Evaluate(this));
            }
        }

        public void ApplyDamage(float amount)
        {
            if (Model == null || !Model.IsAlive || Model.IsInHome)
            {
                return;
            }

            _damageLogic.Apply(amount);
            RefreshLifeState();
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

            if (!isAlive)
            {
                EventBus<HeroDangerChangedEvent>.Invoke(new HeroDangerChangedEvent { Id = Model.InstanceId, Value = 0f });
            }
        }

        private void OnDestroy()
        {
            Registry<HeroFacade>.Remove(this);
        }
    }
}
