using Heroes.GOAP;
using Heroes.GOAP.Core;
using Heroes.GOAP.Core.Debug;
using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using EventBus;
using Heroes.Game.Core.Events;
using Heroes.Game.AI.Strategies;
using Heroes.Game.Buildings;
using Heroes.Game.Monsters;
using Heroes.Game.Quests;
using Registry;
using System.Linq;
using UnityEngine;

namespace Heroes.Game.AI
{
    public class HeroAgent : Agent<GameWorldSnapshot, HeroAnimationController>, IBeliefNameProvider
    {
        private HeroFacade _hero;
        private GameWorldStateManager _worldStateManager;

        private bool _eventsRegistered;
        private EventBinding<HealthChangedEvent> _healthChanged;
        private EventBinding<HeroGoldChangedEvent> _goldChanged;
        private EventBinding<BuildingPlacedEvent> _buildingPlaced;
        private EventBinding<BuildingDestroyedEvent> _buildingDestroyed;
        private EventBinding<UnlockedBuildingsChangedEvent> _unlockedBuildingsChanged;
        private EventBinding<BuildingAttackedEvent> _buildingAttacked;
        private EventBinding<QuestCreatedEvent> _questCreated;
        private EventBinding<QuestUpdatedEvent> _questUpdated;
        private EventBinding<QuestCompletedEvent> _questCompleted;

        private int _lastHpBracket = 100;
        private int _lastGold;
        private bool _inCombat;

        private IActionStrategy _fallback;

        public bool IsPlanning
        {
            get
            {
                return executor is PlanExecutor<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> pe && pe.IsPlanning;
            }
        }

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
                Consts.ENEMIES_NEARBY => "Enemies Nearby",
                Consts.CONSUMABLES => "Consumables",
                Consts.WEAPON_TIER => "Weapon Tier",
                Consts.ARMOR_TIER => "Armor Tier",
                Consts.AMULET_TIER => "Amulet Tier",
                Consts.NOW => "Now",
                Consts.DEFEND_ACTIVE => "Defend Active",
                Consts.DEFEND_X => "Defend X",
                Consts.DEFEND_Z => "Defend Z",
                Consts.HEALTH_PCT => "Health %",
                Consts.BEST_QUEST_EXISTS => "Best Quest Exists",
                Consts.BEST_QUEST_SCORE => "Best Quest Score",
                Consts.BEST_QUEST_TARGET_X => "Best Quest X",
                Consts.BEST_QUEST_TARGET_Z => "Best Quest Z",
                Consts.BEST_QUEST_TARGET_KIND => "Best Quest Kind",
                Consts.HAS_ACTIVE_QUEST => "Has Active Quest",
                Consts.ACTIVE_QUEST_TARGET_X => "Quest X",
                Consts.ACTIVE_QUEST_TARGET_Z => "Quest Z",
                Consts.BEST_QUEST_SHARE => "Best Quest Share",
                _ => string.Empty,
            };

            return !string.IsNullOrWhiteSpace(name);
        }

        public new void Update()
        {
            SyncStateFromModel();
            SyncCombatLockFromPlan();

            if (TickFallback())
            {
                return;
            }

            base.Update();
        }

        private bool TickFallback()
        {
            if (_fallback == null)
            {
                return false;
            }

            _fallback.Update(Time.deltaTime);
            if (_fallback.Complete)
            {
                _fallback.Stop();
                _fallback = null;
                RequestImmediateReplan();
            }

            return true;
        }

        private bool HasNoPlan()
        {
            return executor is PlanExecutor<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> pe && pe.CurrentPlan == null && !pe.IsPlanning;
        }

        private void SyncCombatLockFromPlan()
        {
            _inCombat = _hero != null && _hero.CombatController != null && _hero.CombatController.IsLocked;
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
            var enemiesNearby = _hero != null && _hero.EnemySensor != null ? _hero.EnemySensor.GetEnemyCount() : 0;
            var now = Time.unscaledTime;

            var best = QuestRuntimeConfig.Service != null ? QuestRuntimeConfig.Service.GetBestQuestForHero(_hero) : default;
            var bestExists = best.Exists;
            var bestScore = 0f;
            var bestShare = 0f;
            
            if (bestExists)
            {
                var dps = Mathf.Max(0.1f, (_hero.Definition != null ? _hero.Definition.Attack : 1f) + _hero.Model.EquipmentAttack + _hero.Model.TimedAttack);
                var seconds = best.TargetHp > 0.01f ? best.TargetHp / dps : 0.01f;
                
                bestShare = (float)best.PoolGold / (best.Participants + 1);
                bestScore = bestShare / Mathf.Max(0.01f, seconds);
            }

            var hasActiveQuest = !string.IsNullOrWhiteSpace(_hero.Model.ActiveQuestId) && !string.IsNullOrWhiteSpace(_hero.Model.ActiveQuestTargetInstanceId);
            var qx = 0f;
            var qz = 0f;
            
            if (hasActiveQuest && QuestRuntimeConfig.Service != null)
            {
                if (QuestRuntimeConfig.Service.TryGetById(_hero.Model.ActiveQuestId, out var q) && q != null)
                {
                    if (q.TargetKind == QuestTargetKind.Building)
                    {
                        var b = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(x => x != null && x.Id == q.TargetInstanceId));
                        if (b != null)
                        {
                            var p = b.transform.position;
                            qx = p.x;
                            qz = p.z;
                        }
                    }
                    else
                    {
                        var m = Registry<MonsterFacade>.Get(items => items.FirstOrDefault(x => x != null && x.InstanceId == q.TargetInstanceId));
                        if (m != null)
                        {
                            var p = m.transform.position;
                            qx = p.x;
                            qz = p.z;
                        }
                    }
                }
            }

            var defendActive = !string.IsNullOrWhiteSpace(_hero.Model.DefendBuildingInstanceId) && now <= _hero.Model.DefendBuildingUntilTime;
            var defendX = 0f;
            var defendZ = 0f;
            
            if (defendActive && _worldStateManager != null && _worldStateManager.State != null)
            {
                var snap = _worldStateManager.State.CreateSnapshot();
                if (snap.Locations.TryGetPositionByInstanceId(_hero.Model.DefendBuildingInstanceId, out var pos2d))
                {
                    defendX = pos2d.x;
                    defendZ = pos2d.y;
                }
            }

            PlanExecutor.Context.MutateState((ref AgentState state) =>
            {
                state.SetLocation(transform.position);
                state.SetBelieve(Consts.GOLD, _hero.Model.Gold);
                state.SetBelieve(Consts.HEALTH, _hero.Model.Health.Current);
                
                var maxHp = _hero.Model.Health.Max;
                
                state.SetBelieve(Consts.HEALTH_PCT, maxHp > 0.001f ? _hero.Model.Health.Current / maxHp : 1f);
                state.SetBelieve(Consts.ENEMIES_NEARBY, enemiesNearby);
                state.SetBelieve(Consts.CONSUMABLES, consumables);
                state.SetBelieve(Consts.WEAPON_TIER, _hero.Model.WeaponTier);
                state.SetBelieve(Consts.ARMOR_TIER, _hero.Model.ArmorTier);
                state.SetBelieve(Consts.AMULET_TIER, _hero.Model.AmuletTier);
                state.SetBelieve(Consts.NOW, now);
                state.SetBelieve(Consts.DEFEND_ACTIVE, defendActive ? 1f : 0f);
                state.SetBelieve(Consts.DEFEND_X, defendX);
                state.SetBelieve(Consts.DEFEND_Z, defendZ);

                state.SetBelieve(Consts.BEST_QUEST_EXISTS, bestExists ? 1f : 0f);
                state.SetBelieve(Consts.BEST_QUEST_SCORE, bestScore);
                state.SetBelieve(Consts.BEST_QUEST_TARGET_X, bestExists ? best.TargetPosition.x : 0f);
                state.SetBelieve(Consts.BEST_QUEST_TARGET_Z, bestExists ? best.TargetPosition.z : 0f);
                state.SetBelieve(Consts.BEST_QUEST_TARGET_KIND, bestExists ? (best.TargetKind == QuestTargetKind.Monster ? 1f : 0f) : 0f);
                state.SetBelieve(Consts.BEST_QUEST_SHARE, bestShare);

                state.SetBelieve(Consts.HAS_ACTIVE_QUEST, hasActiveQuest ? 1f : 0f);
                state.SetBelieve(Consts.ACTIVE_QUEST_TARGET_X, qx);
                state.SetBelieve(Consts.ACTIVE_QUEST_TARGET_Z, qz);
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
            _inCombat = false;

            _healthChanged = new EventBinding<HealthChangedEvent>(OnHealthChanged);
            _goldChanged = new EventBinding<HeroGoldChangedEvent>(OnGoldChanged);
            _buildingPlaced = new EventBinding<BuildingPlacedEvent>(_ => RequestDeferredReplan());
            _buildingDestroyed = new EventBinding<BuildingDestroyedEvent>(_ => RequestDeferredReplan());
            _unlockedBuildingsChanged = new EventBinding<UnlockedBuildingsChangedEvent>(_ => RequestDeferredReplan());
            _buildingAttacked = new EventBinding<BuildingAttackedEvent>(OnBuildingAttacked);
            _questCreated = new EventBinding<QuestCreatedEvent>(_ => RequestDeferredReplan());
            _questUpdated = new EventBinding<QuestUpdatedEvent>(_ => RequestDeferredReplan());
            _questCompleted = new EventBinding<QuestCompletedEvent>(OnQuestCompleted);

            EventBus<HealthChangedEvent>.Register(_healthChanged);
            EventBus<HeroGoldChangedEvent>.Register(_goldChanged);
            EventBus<BuildingPlacedEvent>.Register(_buildingPlaced);
            EventBus<BuildingDestroyedEvent>.Register(_buildingDestroyed);
            EventBus<UnlockedBuildingsChangedEvent>.Register(_unlockedBuildingsChanged);
            EventBus<BuildingAttackedEvent>.Register(_buildingAttacked);
            EventBus<QuestCreatedEvent>.Register(_questCreated);
            EventBus<QuestUpdatedEvent>.Register(_questUpdated);
            EventBus<QuestCompletedEvent>.Register(_questCompleted);

            _eventsRegistered = true;
        }

        private void UnregisterEvents()
        {
            if (!_eventsRegistered)
            {
                return;
            }

            EventBus<HealthChangedEvent>.Unregister(_healthChanged);
            EventBus<HeroGoldChangedEvent>.Unregister(_goldChanged);
            EventBus<BuildingPlacedEvent>.Unregister(_buildingPlaced);
            EventBus<BuildingDestroyedEvent>.Unregister(_buildingDestroyed);
            EventBus<UnlockedBuildingsChangedEvent>.Unregister(_unlockedBuildingsChanged);
            EventBus<BuildingAttackedEvent>.Unregister(_buildingAttacked);
            EventBus<QuestCreatedEvent>.Unregister(_questCreated);
            EventBus<QuestUpdatedEvent>.Unregister(_questUpdated);
            EventBus<QuestCompletedEvent>.Unregister(_questCompleted);

            _eventsRegistered = false;
        }

        private void RequestDeferredReplan()
        {
            if (_inCombat)
            {
                return;
            }
            if (executor is PlanExecutor<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> pe)
            {
                pe.RequestReplan(immediate: false);
            }
        }

        private void RequestImmediateReplan()
        {
            if (_inCombat)
            {
                return;
            }
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

        public void NotifyThreat(MonsterFacade attacker)
        {
            if (_hero?.Model == null || attacker == null || !attacker.IsAlive)
            {
                return;
            }

            var controller = _hero.CombatController;
            if (controller == null)
            {
                return;
            }

            var response = controller.HandleThreat(attacker);
            if (response == HeroCombatController.ThreatResponse.None)
            {
                return;
            }

            if (response == HeroCombatController.ThreatResponse.StartedNewCombat)
            {
                StartCombatFallback();
            }
        }

        private void OnBuildingAttacked(BuildingAttackedEvent e)
        {
            if (_hero?.Model == null || string.IsNullOrWhiteSpace(e.InstanceId))
            {
                return;
            }

            _hero.Model.SetDefendBuilding(e.InstanceId, Time.unscaledTime + 12f);

            if (HasNoPlan())
            {
                TryStartFallbackDefend();
                return;
            }

            RequestImmediateReplan();
        }

        private void OnQuestCompleted(QuestCompletedEvent e)
        {
            if (_hero?.Model == null)
            {
                return;
            }

            _hero.Model.ClearActiveQuest(e.Value);
            RequestDeferredReplan();
        }

        private void TryStartFallbackFight()
        {
            if (_fallback != null || _hero == null || _hero.Model == null || !_hero.Model.IsAlive)
            {
                return;
            }

            var sensor = _hero.EnemySensor;
            if (sensor == null || !sensor.TryGetNearestEnemy(transform.position, out var t) || t == null)
            {
                return;
            }

            var monster = t.GetComponentInParent<MonsterFacade>();
            if (monster == null || !monster.IsAlive)
            {
                return;
            }

            _hero.CombatController?.StartCombat(monster, HeroCombatIntent.SelfDefense);
            StartCombatFallback();
        }

        private void TryStartFallbackDefend()
        {
            if (_fallback != null || _hero?.Model == null)
            {
                return;
            }

            var id = _hero.Model.DefendBuildingInstanceId;
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            var building = Registry<BuildingFacade>.Get(items => items.FirstOrDefault(b => b != null && b.Id == id));
            if (building == null || !building.IsAlive)
            {
                return;
            }

            if (executor is not PlanExecutor<Agent<GameWorldSnapshot, HeroAnimationController>, GameWorldSnapshot> pe)
            {
                return;
            }

            _fallback = new DefendBuildingStrategy(this, pe.Context, building);
            _fallback.Start();
        }

        private void StartCombatFallback()
        {
            if (_fallback != null || _hero?.CombatController == null || !_hero.CombatController.IsActive)
            {
                return;
            }

            _fallback = new ActiveCombatStrategy(_hero);
            _fallback.Start();
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


