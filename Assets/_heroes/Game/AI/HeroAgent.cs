using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.GOAP.Core.Debug;
using Heroes.Game.Heroes;
using EventBus;
using Heroes.Game.Core.Events;
using UnityEngine;

namespace Heroes.Game.AI
{
    public class HeroAgent : Agent<GameWorldSnapshot, HeroAnimationController>, IBeliefNameProvider
    {
        private HeroFacade _hero;
        private GameWorldStateManager _worldStateManager;

        private bool _eventsRegistered;
        private EventBinding<HealthChangedEvent> _healthChanged;
        private EventBinding<HeroDangerChangedEvent> _dangerChanged;
        private EventBinding<HeroGoldChangedEvent> _goldChanged;
        private EventBinding<BuildingPlacedEvent> _buildingPlaced;
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyed;
        private EventBinding<UnlockedBuildingsChangedEvent> _unlockedBuildingsChanged;

        private int _lastHpBracket = 100;
        private bool _dangerHigh;
        private int _lastGold;

        public void Initialize(HeroFacade hero, GameWorldStateManager worldStateManager)
        {
            _hero = hero;
            _worldStateManager = worldStateManager;

            EnsureEventsRegistered();
        }

        protected new void OnDestroy()
        {
            base.OnDestroy();
            UnregisterEvents();
        }

        protected override Archetype<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> CreateArchetype()
        {
            return new HeroArchetype(_hero, GoapRuntimeConfig.Buildings);
        }

        protected override IWorldState<GameWorldSnapshot> CreateWorldState()
        {
            return _worldStateManager != null ? _worldStateManager.State : new GameWorldState();
        }

        public bool TryGetBeliefName(int index, out string name)
        {
            name = index switch
            {
                Consts.GOLD => "Gold",
                Consts.HEALTH => "Health",
                Consts.DANGER_LEVEL => "Danger",
                Consts.CONSUMABLES => "Consumables",
                Consts.WEAPON_TIER => "Weapon Tier",
                Consts.ARMOR_TIER => "Armor Tier",
                Consts.AMULET_TIER => "Amulet Tier",
                _ => string.Empty,
            };

            return !string.IsNullOrWhiteSpace(name);
        }

        public new void Update()
        {
            SyncStateFromModel();
            base.Update();
        }

        public bool IsInsideHome()
        {
            if (_hero?.Model == null || _worldStateManager?.State == null)
            {
                return false;
            }

            var snapshot = _worldStateManager.State.CreateSnapshot();
            if (!snapshot.Locations.TryGetPositionByInstanceId(_hero.Model.HomeBuildingInstanceId, out var homePosition))
            {
                return false;
            }

            var currentPosition = new Vector2(transform.position.x, transform.position.z);
            return Vector2.Distance(currentPosition, homePosition) <= _hero.Model.HomeRadius;
        }

        private void SyncStateFromModel()
        {
            if (_hero?.Model == null || PlanExecutor?.Context == null)
            {
                return;
            }

            var consumables = _hero.Model.EquippedConsumables != null ? _hero.Model.EquippedConsumables.Count : 0;

            PlanExecutor.Context.MutateState((ref AgentState state) =>
            {
                state.SetLocation(transform.position);
                state.SetBelieve(Consts.GOLD, _hero.Model.Gold);
                state.SetBelieve(Consts.HEALTH, _hero.Model.Health.Current);
                state.SetBelieve(Consts.DANGER_LEVEL, _hero.Model.DangerLevel);
                state.SetBelieve(Consts.CONSUMABLES, consumables);
                state.SetBelieve(Consts.WEAPON_TIER, _hero.Model.WeaponTier);
                state.SetBelieve(Consts.ARMOR_TIER, _hero.Model.ArmorTier);
                state.SetBelieve(Consts.AMULET_TIER, _hero.Model.AmuletTier);
            });
        }

        private void EnsureEventsRegistered()
        {
            if (_eventsRegistered || _hero?.Model == null)
            {
                return;
            }

            _lastGold = _hero.Model.Gold;
            _lastHpBracket = GetHpBracket(_hero.Model.Health.Current, _hero.Model.Health.Max);
            _dangerHigh = _hero.Model.DangerLevel >= 0.7f;

            _healthChanged = new EventBinding<HealthChangedEvent>(OnHealthChanged);
            _dangerChanged = new EventBinding<HeroDangerChangedEvent>(OnDangerChanged);
            _goldChanged = new EventBinding<HeroGoldChangedEvent>(OnGoldChanged);
            _buildingPlaced = new EventBinding<BuildingPlacedEvent>(_ => RequestDeferredReplan());
            _buildingDestroyed = new EventBinding<BuildingDestroyedEvent>(_ => RequestDeferredReplan());
            _unlockedBuildingsChanged = new EventBinding<UnlockedBuildingsChangedEvent>(_ => RequestDeferredReplan());

            EventBus<HealthChangedEvent>.Register(_healthChanged);
            EventBus<HeroDangerChangedEvent>.Register(_dangerChanged);
            EventBus<HeroGoldChangedEvent>.Register(_goldChanged);
            EventBus<BuildingPlacedEvent>.Register(_buildingPlaced);
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyed);
            EventBus<UnlockedBuildingsChangedEvent>.Register(_unlockedBuildingsChanged);

            _eventsRegistered = true;
        }

        private void UnregisterEvents()
        {
            if (!_eventsRegistered)
            {
                return;
            }

            EventBus<HealthChangedEvent>.Unregister(_healthChanged);
            EventBus<HeroDangerChangedEvent>.Unregister(_dangerChanged);
            EventBus<HeroGoldChangedEvent>.Unregister(_goldChanged);
            EventBus<BuildingPlacedEvent>.Unregister(_buildingPlaced);
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyed);
            EventBus<UnlockedBuildingsChangedEvent>.Unregister(_unlockedBuildingsChanged);

            _eventsRegistered = false;
        }

        private void RequestDeferredReplan()
        {
            if (executor is PlanExecutor<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> pe)
            {
                pe.RequestReplan(immediate: false);
            }
        }

        private void RequestImmediateReplan()
        {
            if (executor is PlanExecutor<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> pe)
            {
                pe.RequestReplan(immediate: true);
            }
        }

        private void OnHealthChanged(HealthChangedEvent e)
        {
            if (_hero?.Model == null)
            {
                return;
            }

            if (e.Id != _hero.Model.InstanceId)
            {
                return;
            }

            var max = _hero.Model.Health.Max;
            var bracket = GetHpBracket(e.Value, max);
            if (bracket < _lastHpBracket)
            {
                _lastHpBracket = bracket;
                RequestImmediateReplan();
            }
        }

        private void OnDangerChanged(HeroDangerChangedEvent e)
        {
            if (_hero?.Model == null || e.Id != _hero.Model.InstanceId)
            {
                return;
            }

            var high = e.Value >= 0.7f;
            if (high && !_dangerHigh)
            {
                _dangerHigh = true;
                RequestImmediateReplan();
                return;
            }

            _dangerHigh = high;
        }

        private void OnGoldChanged(HeroGoldChangedEvent e)
        {
            if (_hero?.Model == null || e.Id != _hero.Model.InstanceId)
            {
                return;
            }

            if (e.Value >= _lastGold + 200)
            {
                RequestDeferredReplan();
            }

            _lastGold = e.Value;
        }

        private static int GetHpBracket(float current, float max)
        {
            if (max <= 0.001f)
            {
                return 0;
            }

            var pct = (current / max) * 100f;
            if (pct < 10f) return 10;
            if (pct < 20f) return 20;
            if (pct < 35f) return 35;
            if (pct < 50f) return 50;
            if (pct < 80f) return 80;
            return 100;
        }
    }
}


