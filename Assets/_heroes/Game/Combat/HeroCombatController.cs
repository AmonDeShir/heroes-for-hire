using Heroes.Game.Abstractions;
using Heroes.Game.AI;
using Heroes.Game.Buildings;
using Heroes.Game.Heroes;
using Heroes.Game.Monsters;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.Combat
{
    public sealed class HeroCombatController
    {
        public enum ThreatResponse
        {
            None = 0,
            StartedNewCombat = 1,
            SwitchedToThreat = 2,
        }

        private const float HealThresholdPct = 0.40f;
        private const float FleeThresholdPct = 0.20f;
        private const float MinAttackCadence = 0.35f;
        private const float BaseHeroAttackRange = 2.05f;
        private const float BuildingAttackRange = 2.15f;
        private const float AttackRangeBuffer = 0.25f;
        private const float FleeDurationSeconds = 8f;
        private const float FleeReachDistance = 2f;
        private const float FleeRerollSeconds = 5f;
        private const float FaceTurnSpeed = 720f;

        private readonly HeroFacade _hero;
        private readonly NavMeshAgent _navAgent;
        private readonly HeroAnimationController _animator;
        private readonly CombatService _combatService;

        private IDamageable _primaryTarget;
        private HeroCombatIntent _primaryIntent;
        private IDamageable _overrideTarget;
        private HeroCombatIntent _overrideIntent;

        private float _nextAttackAt;
        private float _nextHealAt;

        private float _fleeUntil;
        private Vector3 _fleeDestination;
        private float _nextFleeRerollAt;
        private float _lastFleeProgressAt;
        private float _lastFleeRemaining = float.MaxValue;

        public HeroCombatController(HeroFacade hero, NavMeshAgent navAgent, HeroAnimationController animator, CombatService combatService)
        {
            _hero = hero;
            _navAgent = navAgent;
            _animator = animator;
            _combatService = combatService;
            State = HeroCombatState.Idle;
            SyncCombatTarget();
        }

        public HeroCombatState State { get; private set; }
        public bool IsActive => CurrentTarget != null && State != HeroCombatState.Idle && State != HeroCombatState.Dead;
        public bool IsLocked => IsActive;
        public IDamageable CurrentTarget => _overrideTarget ?? _primaryTarget;
        public IDamageable PrimaryTarget => _primaryTarget;
        public HeroCombatIntent CurrentIntent => _overrideTarget != null ? _overrideIntent : _primaryIntent;

        public void StartCombat(IDamageable target, HeroCombatIntent intent)
        {
            if (_hero?.Model == null || !_hero.Model.IsAlive)
            {
                return;
            }

            if (!IsValidTarget(target))
            {
                if (_primaryTarget == null && _overrideTarget == null)
                {
                    CancelCombat();
                }
                return;
            }

            _primaryTarget = target;
            _primaryIntent = intent;
            _overrideTarget = null;
            _overrideIntent = HeroCombatIntent.None;
            _nextAttackAt = 0f;
            EnterApproach();
        }

        public ThreatResponse HandleThreat(MonsterFacade attacker)
        {
            if (!IsValidTarget(attacker))
            {
                return ThreatResponse.None;
            }

            if (!IsActive)
            {
                StartCombat(attacker, HeroCombatIntent.SelfDefense);
                return IsActive ? ThreatResponse.StartedNewCombat : ThreatResponse.None;
            }

            if (CurrentTarget is MonsterFacade)
            {
                return ThreatResponse.None;
            }

            _overrideTarget = attacker;
            _overrideIntent = HeroCombatIntent.SelfDefense;
            EnterApproach();
            return ThreatResponse.SwitchedToThreat;
        }

        public bool HasPrimaryTarget(IDamageable target)
        {
            return ReferenceEquals(_primaryTarget, target);
        }

        public void CancelCombat()
        {
            _primaryTarget = null;
            _overrideTarget = null;
            _primaryIntent = HeroCombatIntent.None;
            _overrideIntent = HeroCombatIntent.None;
            _nextAttackAt = 0f;
            _fleeUntil = 0f;
            State = HeroCombatState.Idle;

            if (_navAgent != null && _navAgent.enabled)
            {
                _navAgent.stoppingDistance = 0f;
                _navAgent.ResetPath();
            }

            SyncCombatTarget();
        }

        public void NotifyDeath()
        {
            CancelCombat();
            State = HeroCombatState.Dead;
        }

        public void NotifyRevive()
        {
            State = HeroCombatState.Idle;
            SyncCombatTarget();
        }

        public void Tick(float deltaTime)
        {
            if (_hero?.Model == null)
            {
                return;
            }

            if (!_hero.Model.IsAlive)
            {
                NotifyDeath();
                return;
            }

            if (State == HeroCombatState.Dead)
            {
                State = HeroCombatState.Idle;
            }

            CleanupTargets();
            
            if (CurrentTarget == null)
            {
                if (State != HeroCombatState.Idle)
                {
                    CancelCombat();
                }
                return;
            }

            var hpPct = _hero.Model.Health.Max > 0.001f ? _hero.Model.Health.Current / _hero.Model.Health.Max : 1f;
            
            if (State != HeroCombatState.TryHeal && State != HeroCombatState.TryBoostBeforeFlee && State != HeroCombatState.Flee)
            {
                if (hpPct <= HealThresholdPct && Time.unscaledTime >= _nextHealAt)
                {
                    State = HeroCombatState.TryHeal;
                }
                else if (hpPct <= FleeThresholdPct)
                {
                    State = HeroCombatState.TryBoostBeforeFlee;
                }
            }

            switch (State)
            {
                case HeroCombatState.Idle:
                    EnterApproach();
                    break;
                case HeroCombatState.Approach:
                    TickApproach();
                    break;
                case HeroCombatState.AttackWindup:
                    TickAttackWindup();
                    break;
                case HeroCombatState.AttackRecover:
                    TickAttackRecover();
                    break;
                case HeroCombatState.TryHeal:
                    TickTryHeal();
                    break;
                case HeroCombatState.TryBoostBeforeFlee:
                    TickTryBoostBeforeFlee();
                    break;
                case HeroCombatState.Flee:
                    TickFlee();
                    break;
            }

            SyncCombatTarget();
        }

        private void TickApproach()
        {
            if (!TryGetCombatGeometry(out var targetPos, out var desiredRange, out var distance))
            {
                CancelCombat();
                return;
            }

            FaceTarget(targetPos);

            if (distance <= desiredRange)
            {
                StopMoving();
                State = HeroCombatState.AttackWindup;
                return;
            }

            if (_navAgent != null && _navAgent.enabled)
            {
                _navAgent.stoppingDistance = Mathf.Max(0.1f, desiredRange - 0.05f);
                _navAgent.SetDestination(targetPos);
            }
        }

        private void TickAttackWindup()
        {
            if (!TryGetCombatGeometry(out var targetPos, out var desiredRange, out var distance))
            {
                CancelCombat();
                return;
            }

            if (distance > desiredRange)
            {
                State = HeroCombatState.Approach;
                return;
            }

            FaceTarget(targetPos);

            var now = Time.unscaledTime;
            if (now < _nextAttackAt)
            {
                State = HeroCombatState.AttackRecover;
                return;
            }

            PerformAttack();
            _nextAttackAt = now + GetAttackCadenceSeconds();
            State = HeroCombatState.AttackRecover;
        }

        private void TickAttackRecover()
        {
            if (!TryGetCombatGeometry(out _, out var desiredRange, out var distance))
            {
                CancelCombat();
                return;
            }

            if (distance > desiredRange + 0.1f)
            {
                State = HeroCombatState.Approach;
                return;
            }

            if (Time.unscaledTime >= _nextAttackAt)
            {
                State = HeroCombatState.AttackWindup;
            }
        }

        private void TickTryHeal()
        {
            if (_combatService?.TryUseHealingConsumable(_hero) == true)
            {
                _nextHealAt = Time.unscaledTime + 2f;
            }

            var hpPct = _hero.Model.Health.Max > 0.001f ? _hero.Model.Health.Current / _hero.Model.Health.Max : 1f;
            State = hpPct <= FleeThresholdPct ? HeroCombatState.TryBoostBeforeFlee : HeroCombatState.Approach;
        }

        private void TickTryBoostBeforeFlee()
        {
            _combatService?.TryUseSpeedConsumable(_hero);

            _fleeUntil = Time.unscaledTime + FleeDurationSeconds;
            _fleeDestination = PickFleePoint(_hero.transform.position);
            _nextFleeRerollAt = Time.unscaledTime + FleeRerollSeconds;
            _lastFleeProgressAt = Time.unscaledTime;
            _lastFleeRemaining = float.MaxValue;

            if (_navAgent != null && _navAgent.enabled)
            {
                _navAgent.stoppingDistance = 0f;
                _navAgent.SetDestination(_fleeDestination);
            }

            State = HeroCombatState.Flee;
        }

        private void TickFlee()
        {
            if (_navAgent == null || !_navAgent.enabled)
            {
                CancelCombat();
                return;
            }

            var now = Time.unscaledTime;
            if (now >= _fleeUntil)
            {
                CancelCombat();
                return;
            }

            if (_navAgent.pathPending)
            {
                return;
            }

            var remaining = _navAgent.remainingDistance;
            if (remaining + 0.05f < _lastFleeRemaining)
            {
                _lastFleeRemaining = remaining;
                _lastFleeProgressAt = now;
            }

            if (remaining <= FleeReachDistance)
            {
                CancelCombat();
                return;
            }

            if (now >= _nextFleeRerollAt || (now - _lastFleeProgressAt) >= FleeRerollSeconds || !_navAgent.hasPath)
            {
                _fleeDestination = PickFleePoint(_hero.transform.position);
                _navAgent.SetDestination(_fleeDestination);
                _nextFleeRerollAt = now + FleeRerollSeconds;
                _lastFleeProgressAt = now;
                _lastFleeRemaining = float.MaxValue;
            }
        }

        private void PerformAttack()
        {
            var current = CurrentTarget;
            if (!IsValidTarget(current))
            {
                return;
            }

            _animator?.PlayAttack();

            var atk = _hero.Definition != null ? _hero.Definition.Attack : 1f;
            var damage = Mathf.Max(0.1f, atk + _hero.Model.EquipmentAttack + _hero.Model.TimedAttack);

            if (current is MonsterFacade monster)
            {
                monster.ApplyDamageFrom(_hero.Model.InstanceId, damage);
                return;
            }

            current.ApplyDamage(damage);
        }

        private float GetAttackCadenceSeconds()
        {
            var fromAnimation = _animator != null ? _animator.GetAttackDuration() : 1.5f;
            
            
            return Mathf.Max(MinAttackCadence, fromAnimation);
        }

        private void EnterApproach()
        {
            if (CurrentTarget == null)
            {
                CancelCombat();
                return;
            }

            State = HeroCombatState.Approach;
        }

        private void CleanupTargets()
        {
            if (!IsValidTarget(_overrideTarget))
            {
                _overrideTarget = null;
                _overrideIntent = HeroCombatIntent.None;
            }

            if (!IsValidTarget(_primaryTarget))
            {
                _primaryTarget = null;
                _primaryIntent = HeroCombatIntent.None;
            }
        }

        private void SyncCombatTarget()
        {
            _hero?.SetCombatTarget(CurrentTarget);
            if (CurrentTarget == null)
            {
                _hero?.ClearCombatTarget();
            }
        }

        private bool TryGetCombatGeometry(out Vector3 targetPos, out float desiredRange, out float distance)
        {
            var target = CurrentTarget;
            targetPos = _hero != null ? _hero.transform.position : Vector3.zero;
            desiredRange = BaseHeroAttackRange;
            distance = float.MaxValue;

            if (!IsValidTarget(target) || _hero == null)
            {
                return false;
            }

            if (target is BuildingFacade building)
            {
                targetPos = building.DoorWorldPosition;
                desiredRange = BuildingAttackRange;
            }
            else if (target is MonsterFacade monster)
            {
                targetPos = monster.transform.position;
                var monsterRange = monster.Definition != null ? monster.Definition.AttackRange : 1.5f;
                desiredRange = Mathf.Max(BaseHeroAttackRange, monsterRange + AttackRangeBuffer);
            }
            else if (target is HeroFacade heroTarget)
            {
                targetPos = heroTarget.transform.position;
                desiredRange = BaseHeroAttackRange;
            }
            else if (target is MonoBehaviour behaviour)
            {
                targetPos = behaviour.transform.position;
            }
            else
            {
                return false;
            }

            distance = Vector3.Distance(_hero.transform.position, targetPos);
            return true;
        }

        private void FaceTarget(Vector3 targetPos)
        {
            if (_hero == null)
            {
                return;
            }

            var to = targetPos - _hero.transform.position;
            to.y = 0f;
            if (to.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            var desiredRot = Quaternion.LookRotation(to.normalized, Vector3.up);
            _hero.transform.rotation = Quaternion.RotateTowards(_hero.transform.rotation, desiredRot, FaceTurnSpeed * Time.deltaTime);
        }

        private void StopMoving()
        {
            if (_navAgent == null || !_navAgent.enabled)
            {
                return;
            }

            _navAgent.ResetPath();
            _navAgent.stoppingDistance = 0f;
        }

        private bool IsValidTarget(IDamageable target)
        {
            return target != null && target.IsAlive;
        }

        private Vector3 PickFleePoint(Vector3 origin)
        {
            var terrains = Terrain.activeTerrains;
            if (terrains != null && terrains.Length > 0)
            {
                var pick = PickRandomTerrain(terrains);
                if (pick != null && pick.terrainData != null)
                {
                    var tp = pick.GetPosition();
                    var size = pick.terrainData.size;
                    var p = new Vector3(tp.x + Random.value * size.x, 0f, tp.z + Random.value * size.z);
                    p.y = pick.SampleHeight(p) + tp.y;
                    if (NavMesh.SamplePosition(p, out var hit, 200f, NavMesh.AllAreas))
                    {
                        return hit.position;
                    }
                }
            }

            var dir = Random.insideUnitSphere * 25f;
            dir.y = 0f;
            var candidate = origin + dir;
            if (NavMesh.SamplePosition(candidate, out var navHit, 25f, NavMesh.AllAreas))
            {
                return navHit.position;
            }

            return origin;
        }

        private static Terrain PickRandomTerrain(Terrain[] terrains)
        {
            if (terrains == null || terrains.Length == 0)
            {
                return null;
            }

            var total = 0f;
            for (var i = 0; i < terrains.Length; i++)
            {
                var terrain = terrains[i];
                if (terrain == null || terrain.terrainData == null)
                {
                    continue;
                }

                var size = terrain.terrainData.size;
                total += Mathf.Max(1f, size.x * size.z);
            }

            if (total <= 0f)
            {
                return terrains[Random.Range(0, terrains.Length)];
            }

            var roll = Random.value * total;
            var current = 0f;
            for (var i = 0; i < terrains.Length; i++)
            {
                var terrain = terrains[i];
                if (terrain == null || terrain.terrainData == null)
                {
                    continue;
                }

                var size = terrain.terrainData.size;
                current += Mathf.Max(1f, size.x * size.z);
                if (roll <= current)
                {
                    return terrain;
                }
            }

            return terrains[terrains.Length - 1];
        }
    }
}
