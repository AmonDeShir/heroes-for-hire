using System;
using System.Collections.Generic;
using Heroes.Content.Monsters;
using Heroes.Game.Abstractions;
using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using Heroes.Content.Heroes.ItemEffects;
using Heroes.Game.Core.Events;
using EventBus;
using Heroes.Game.Buildings;
using Registry;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.Monsters
{
    public sealed class MonsterFacade : MonoBehaviour, IDamageable, ISelectable
    {
        private enum State
        {
            Wander = 0,
            Chase = 1,
            Attack = 2,
            Flee = 3,
            Dead = 4,
        }

        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private Faction faction;
        [SerializeField] private global::Heroes.Game.AI.HeroAnimationController animator;
        [SerializeField] private LayerMask overlapMask = ~0;
        [Header("Sensors")]
        [SerializeField] private MonsterHeroSensor heroSensor;

        public MonsterDefinition Definition { get; private set; }

        public string InstanceId { get; private set; }

        public string Id => InstanceId;
        public string Name => Definition != null ? Definition.DisplayName : string.Empty;
        public string Description => Definition != null ? Definition.Description : string.Empty;
        public string Icon => Definition != null ? Definition.IconPath : string.Empty;

        public float Health => _hp;
        public float MaxHealth => _maxHp;
        public bool IsAlive => _state != State.Dead;

        private State _state;
        private Vector3 _spawnPoint;
        private float _hp;
        private float _maxHp;

        private float _nextAttackAt;
        private Transform _target;
        private bool _attackHitPending;
        private float _attackHitAt;
        private float _pendingAttackDamage;

        public Transform CurrentTarget => _target;

        private float _nextAttackLogAt;
        private float _nextAttackStateLogAt;

        private float _nextAcquireTargetAt;
        private float _nextHeroSenseAt;

        private float _nextWanderRerollAt;
        private float _lastWanderProgressAt;
        private float _lastWanderRemaining = float.MaxValue;

        private bool _combatAnnounced;
        public bool RaidMode { get; private set; }

        private readonly Dictionary<string, float> _damageByHero = new();
        private readonly Dictionary<string, float> _lastHitAtByHero = new();

        public void Initialize(MonsterDefinition definition, Vector3 spawnPoint)
        {
            Definition = definition;
            InstanceId = Guid.NewGuid().ToString();
            _spawnPoint = spawnPoint;

            _maxHp = definition != null ? Mathf.Max(1f, definition.MaxHp) : 1f;
            _hp = _maxHp;

            if (navMeshAgent != null && definition != null)
            {
                navMeshAgent.speed = Mathf.Max(0.1f, definition.MoveSpeed);

                if (NavMesh.SamplePosition(transform.position, out var hit, 6f, NavMesh.AllAreas))
                {
                    navMeshAgent.Warp(hit.position);
                }
            }

            if (faction != null)
            {
                faction.Team = TeamType.Enemies;
                faction.Level = Mathf.Clamp(Mathf.RoundToInt(_maxHp / 50f), 1, 10);
            }

            _state = State.Wander;
            _nextAcquireTargetAt = 0f;
            _nextHeroSenseAt = 0f;
            _nextWanderRerollAt = 0f;
            _lastWanderProgressAt = 0f;
            _lastWanderRemaining = float.MaxValue;
            _attackHitPending = false;
            _attackHitAt = 0f;
            _pendingAttackDamage = 0f;
            Registry<MonsterFacade>.TryAdd(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var def = Definition;
            if (def == null)
            {
                return;
            }

            var pos = transform.position;
            Gizmos.color = new Color(1f, 0.25f, 0.25f, 0.25f);
            Gizmos.DrawWireSphere(pos, def.AggroRange);
            Gizmos.color = new Color(1f, 0.8f, 0.1f, 0.25f);
            Gizmos.DrawWireSphere(pos, def.AttackRange);
        }
#endif

        private void OnDestroy()
        {
            Registry<MonsterFacade>.Remove(this);
        }

        private void Update()
        {
            if (_state == State.Dead)
            {
                return;
            }

            if (Definition == null)
            {
                return;
            }

            PruneContributors(Time.unscaledTime);

            if (animator != null && navMeshAgent != null)
            {
                animator.SetSpeed(navMeshAgent.velocity.magnitude);
            }

            var hpPct = _maxHp > 0.001f ? _hp / _maxHp : 0f;
            if (hpPct <= Definition.FleeHpPct)
            {
                _state = State.Flee;
            }

            if (RaidMode && _state == State.Wander)
            {
                                                                                        
                if (TryAcquireHeroTarget(out var heroTarget))
                {
                    _target = heroTarget;
                    _state = State.Chase;
                }
                else
                {
                                                                                                    
                    if (AcquireBuildingTarget())
                    {
                        _state = State.Chase;
                    }
                }
            }

            switch (_state)
            {
                case State.Wander:
                    TickWander();
                    break;
                case State.Chase:
                    TickChase();
                    break;
                case State.Attack:
                    TickAttack();
                    break;
                case State.Flee:
                    TickFlee();
                    break;
            }
        }

        public void SetRaidMode(bool value)
        {
            RaidMode = value;
        }

        public void ApplyDamage(float amount)
        {
            ApplyDamageFrom(heroId: null, amount);
        }

        public void ApplyDamageFrom(string heroId, float amount)
        {
            if (_state == State.Dead || amount <= 0f)
            {
                return;
            }

            _hp = Mathf.Max(0f, _hp - amount);

            if (!string.IsNullOrWhiteSpace(heroId))
            {
                var now = Time.unscaledTime;
                _lastHitAtByHero[heroId] = now;
                _damageByHero.TryGetValue(heroId, out var prev);
                _damageByHero[heroId] = prev + amount;
            }

            if (_hp <= 0f)
            {
                Die();
                return;
            }

            if (!_combatAnnounced)
            {
                AnnounceCombatStart();
            }

            if (_target == null)
            {
                AcquireTarget();
            }
        }

        private void Die()
        {
            if (_state == State.Dead)
            {
                return;
            }

            _state = State.Dead;
            _attackHitPending = false;
            _pendingAttackDamage = 0f;

            if (animator != null)
            {
                animator.PlayDeath();
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.ResetPath();
                navMeshAgent.enabled = false;
            }

            DistributeLoot();
            EventBus<MonsterKilledEvent>.Invoke(new MonsterKilledEvent { InstanceId = InstanceId });
            AnnounceCombatEnd();
            Destroy(gameObject, 1.5f);
        }

        private void TickWander()
        {
            var now = Time.unscaledTime;
            if (now >= _nextAcquireTargetAt && AcquireTarget())
            {
                _nextAcquireTargetAt = now + 0.25f;
                _state = State.Chase;
                return;
            }

            if (now >= _nextAcquireTargetAt)
            {
                _nextAcquireTargetAt = now + 0.25f;
            }

            if (navMeshAgent == null)
            {
                return;
            }

            var reachThreshold = Mathf.Max(0.5f, navMeshAgent.stoppingDistance + 0.25f);
            var remaining0 = navMeshAgent.remainingDistance;
            if (!navMeshAgent.hasPath || float.IsInfinity(remaining0) || remaining0 <= reachThreshold)
            {
                var dest = PickWanderPoint();
                navMeshAgent.SetDestination(dest);
                _nextWanderRerollAt = now + 5f;
                _lastWanderProgressAt = now;
                _lastWanderRemaining = float.MaxValue;
                return;
            }

            if (navMeshAgent.pathPending)
            {
                return;
            }

            var remaining = navMeshAgent.remainingDistance;
            if (remaining + 0.05f < _lastWanderRemaining)
            {
                _lastWanderRemaining = remaining;
                _lastWanderProgressAt = now;
            }

            if (now >= _nextWanderRerollAt || (now - _lastWanderProgressAt) >= 5f)
            {
                var dest = PickWanderPoint();
                navMeshAgent.SetDestination(dest);
                _nextWanderRerollAt = now + 5f;
                _lastWanderProgressAt = now;
                _lastWanderRemaining = float.MaxValue;
            }
        }

        private void TickChase()
        {
            if (_target == null)
            {
                _state = State.Wander;
                return;
            }

                                                                        
            if (RaidMode && _target.TryGetComponent<BuildingFacade>(out var _))
            {
                if (TryAcquireHeroTarget(out var heroTarget))
                {
                    _target = heroTarget;
                }
            }

            var targetPos = _target.position;
            if (_target.TryGetComponent<BuildingFacade>(out var b) && b != null)
            {
                targetPos = b.DoorWorldPosition;
            }

            var dist = Vector3.Distance(transform.position, targetPos);
            if (!RaidMode && dist > Definition.AggroRange * 1.25f)
            {
                _target = null;
                _state = State.Wander;
                return;
            }

            if (dist <= GetAttackStartRange())
            {
                if (navMeshAgent != null)
                {
                    navMeshAgent.ResetPath();
                }
                _state = State.Attack;
                return;
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.stoppingDistance = Mathf.Max(0.1f, GetAttackStartRange() - 0.05f);
                navMeshAgent.SetDestination(targetPos);
            }
        }

        private bool AcquireBuildingTarget()
        {
            var castleDefId = global::Heroes.Game.AI.GoapRuntimeConfig.Buildings != null && global::Heroes.Game.AI.GoapRuntimeConfig.Buildings.Castle != null
                ? global::Heroes.Game.AI.GoapRuntimeConfig.Buildings.Castle.Id
                : string.Empty;

            BuildingFacade bestCastle = null;
            var bestCastleDist = float.MaxValue;
            BuildingFacade best = null;
            var bestDist = float.MaxValue;
            foreach (var b in Registry.Registry<BuildingFacade>.All())
            {
                if (b == null || !b.IsAlive)
                {
                    continue;
                }

                if (b.TryGetComponent<Faction>(out var bf) && bf != null && bf.Team == TeamType.Enemies)
                {
                    continue;
                }

                var d = (b.DoorWorldPosition - transform.position).sqrMagnitude;

                if (!string.IsNullOrWhiteSpace(castleDefId) && b.Definition != null && b.Definition.Id == castleDefId)
                {
                    if (d < bestCastleDist)
                    {
                        bestCastleDist = d;
                        bestCastle = b;
                    }
                }

                if (d < bestDist)
                {
                    bestDist = d;
                    best = b;
                }
            }

            var pick = bestCastle != null ? bestCastle : best;
            _target = pick != null ? pick.transform : null;
            return _target != null;
        }

        private bool TryAcquireHeroTarget(out Transform target)
        {
            target = null;
            if (Definition == null)
            {
                return false;
            }

            var now = Time.unscaledTime;
            if (now < _nextHeroSenseAt)
            {
                return false;
            }
            _nextHeroSenseAt = now + 0.25f;

            if (heroSensor != null && heroSensor.TryGetNearestEnemy(transform.position, out var sensed))
            {
                target = sensed;
                return target != null;
            }

            return false;
        }

        private void TickAttack()
        {
            if (_target == null)
            {
                _attackHitPending = false;
                _state = State.Wander;
                return;
            }

                                                                                                     
            if (RaidMode && _target.TryGetComponent<BuildingFacade>(out var _))
            {
                if (TryAcquireHeroTarget(out var heroTarget))
                {
                    _target = heroTarget;
                    _state = State.Chase;
                    return;
                }
            }

            if (_target.TryGetComponent<IDamageable>(out var targetDmg) && !targetDmg.IsAlive)
            {
                _attackHitPending = false;
                _target = null;
                _state = State.Wander;
                return;
            }

            if (Time.unscaledTime >= _nextAttackStateLogAt)
            {
                _nextAttackStateLogAt = Time.unscaledTime + 1.0f;
                Debug.Log($"[Monster] {Name} entering ATTACK vs {FormatTarget(_target)}", this);
            }

            ResolvePendingAttackHit();

            var targetPos = _target.position;
            if (_target.TryGetComponent<BuildingFacade>(out var b) && b != null)
            {
                targetPos = b.DoorWorldPosition;
            }

            var to = targetPos - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
            {
                var desired = Quaternion.LookRotation(to.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, 720f * Time.deltaTime);
            }

            var dist = Vector3.Distance(transform.position, targetPos);
            if (dist > GetAttackKeepRange())
            {
                _state = State.Chase;
                return;
            }

            if (_attackHitPending || Time.unscaledTime < _nextAttackAt)
            {
                return;
            }

            if (animator != null)
            {
                animator.PlayAttack();
            }

            var attackDuration = animator != null ? animator.GetAttackDuration() : 0f;
            var cadence = Mathf.Max(0.1f, Definition.AttackIntervalSeconds, attackDuration);
            _nextAttackAt = Time.unscaledTime + cadence;
            _attackHitPending = true;
            _attackHitAt = Time.unscaledTime + Mathf.Max(0.05f, cadence * 0.45f);
            _pendingAttackDamage = Mathf.Max(0.1f, Definition.AttackDamage);
        }

        private void ResolvePendingAttackHit()
        {
            if (!_attackHitPending || Time.unscaledTime < _attackHitAt)
            {
                return;
            }

            _attackHitPending = false;

            if (_target == null || !_target.TryGetComponent<IDamageable>(out var dmg) || !dmg.IsAlive)
            {
                return;
            }

            var targetPos = _target.position;
            if (_target.TryGetComponent<BuildingFacade>(out var building) && building != null)
            {
                targetPos = building.DoorWorldPosition;
            }

            if (Vector3.Distance(transform.position, targetPos) > GetAttackKeepRange() + 0.15f)
            {
                return;
            }

            if (Time.unscaledTime >= _nextAttackLogAt)
            {
                _nextAttackLogAt = Time.unscaledTime + 0.5f;
                Debug.Log($"[Monster] {Name} attacking {FormatTarget(_target)} dmg={_pendingAttackDamage:0.##}", this);
            }

            if (dmg is HeroFacade heroTarget)
            {
                heroTarget.ApplyDamageFrom(this, _pendingAttackDamage);
            }
            else
            {
                dmg.ApplyDamage(_pendingAttackDamage);
            }
        }

        private float GetAttackStartRange()
        {
            return Mathf.Max(1f, Definition != null ? Definition.AttackRange + 0.25f : 2.25f);
        }

        private float GetAttackKeepRange()
        {
            return Mathf.Max(GetAttackStartRange(), Definition != null ? Definition.AttackRange + 0.5f : 2.5f);
        }

        private string FormatTarget(Transform t)
        {
            if (t == null)
            {
                return "<null>";
            }

            if (t.TryGetComponent<BuildingFacade>(out var b) && b != null)
            {
                return $"Building:{b.Name}#{b.GetInstanceID()}";
            }

            if (t.TryGetComponent<HeroFacade>(out var h) && h != null)
            {
                return $"Hero:{h.Name}#{h.GetInstanceID()}";
            }

            return $"{t.name}#{t.GetInstanceID()}";
        }

        private void TickFlee()
        {
            if (navMeshAgent == null)
            {
                return;
            }

            navMeshAgent.SetDestination(_spawnPoint);
            if (Vector3.Distance(transform.position, _spawnPoint) <= 1.0f)
            {
                _target = null;
                _state = State.Wander;
            }
        }

        private bool AcquireTarget()
        {
            if (TryAcquireHeroTarget(out var heroTarget))
            {
                _target = heroTarget;
            }
            else if (RaidMode && AcquireBuildingTarget())
            {
            }
            else
            {
                _target = null;
            }

            if (_target != null && !_combatAnnounced)
            {
                AnnounceCombatStart();
            }
            return _target != null;
        }

        private void AnnounceCombatStart()
        {
            _combatAnnounced = true;
            EventBus<CombatStartedEvent>.Invoke(new CombatStartedEvent
            {
                SourceId = gameObject != null ? gameObject.GetInstanceID().ToString() : string.Empty,
                Position = transform.position,
                Radius = Mathf.Max(4f, Definition != null ? Definition.AggroRange : 10f),
            });
        }

        private void AnnounceCombatEnd()
        {
            if (!_combatAnnounced)
            {
                return;
            }

            EventBus<CombatEndedEvent>.Invoke(new CombatEndedEvent
            {
                SourceId = gameObject != null ? gameObject.GetInstanceID().ToString() : string.Empty,
                Position = transform.position,
                Radius = Mathf.Max(4f, Definition != null ? Definition.AggroRange : 10f),
            });
        }

        private Vector3 PickWanderPoint()
        {
            var terrains = Terrain.activeTerrains;
            if (terrains != null && terrains.Length > 0)
            {
                var pick = PickRandomTerrain(terrains);
                if (pick != null && pick.terrainData != null)
                {
                    var tp = pick.GetPosition();
                    var size = pick.terrainData.size;
                    var terrainP = new Vector3(
                        tp.x + UnityEngine.Random.value * size.x,
                        0f,
                        tp.z + UnityEngine.Random.value * size.z
                    );

                    terrainP.y = pick.SampleHeight(terrainP) + tp.y;

                    if (NavMesh.SamplePosition(terrainP, out var terrainHit, 200f, NavMesh.AllAreas))
                    {
                        return terrainHit.position;
                    }
                }
            }

            var r = Mathf.Max(1f, Definition.WanderRadius);
            var localP = _spawnPoint + new Vector3(UnityEngine.Random.Range(-r, r), 0f, UnityEngine.Random.Range(-r, r));
            if (NavMesh.SamplePosition(localP, out var hit, 6f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return _spawnPoint;
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
                var t = terrains[i];
                if (t == null || t.terrainData == null)
                {
                    continue;
                }

                var s = t.terrainData.size;
                total += Mathf.Max(0.001f, s.x * s.z);
            }

            if (total <= 0.001f)
            {
                return terrains[0];
            }

            var roll = UnityEngine.Random.value * total;
            for (var i = 0; i < terrains.Length; i++)
            {
                var t = terrains[i];
                if (t == null || t.terrainData == null)
                {
                    continue;
                }

                var s = t.terrainData.size;
                roll -= Mathf.Max(0.001f, s.x * s.z);
                if (roll <= 0f)
                {
                    return t;
                }
            }

            return terrains[0];
        }

        private void PruneContributors(float now)
        {
            if (_lastHitAtByHero.Count == 0)
            {
                return;
            }

            var toRemove = (List<string>)null;
            foreach (var pair in _lastHitAtByHero)
            {
                if (now - pair.Value <= 30f)
                {
                    continue;
                }

                toRemove ??= new List<string>();
                toRemove.Add(pair.Key);
            }

            if (toRemove == null)
            {
                return;
            }

            for (var i = 0; i < toRemove.Count; i++)
            {
                var id = toRemove[i];
                _lastHitAtByHero.Remove(id);
                _damageByHero.Remove(id);
            }
        }

        private void DistributeLoot()
        {
            if (Definition == null || _damageByHero.Count == 0)
            {
                return;
            }

            var contributors = new List<(HeroFacade hero, float dmg)>();
            float total = 0f;
            foreach (var pair in _damageByHero)
            {
                var hero = Registry<HeroFacade>.Get(items =>
                    {
                        foreach (var h in items)
                        {
                            if (h != null && h.Model != null && h.Model.InstanceId == pair.Key)
                            {
                                return h;
                            }
                        }
                        return null;
                    });

                if (hero == null || hero.Model == null || !hero.Model.IsAlive)
                {
                    continue;
                }

                var dmg = Mathf.Max(0f, pair.Value);
                if (dmg <= 0.01f)
                {
                    continue;
                }

                contributors.Add((hero, dmg));
                total += dmg;
            }

            if (contributors.Count == 0 || total <= 0.01f)
            {
                return;
            }

            contributors.Sort((a, b) => b.dmg.CompareTo(a.dmg));

            var gold = UnityEngine.Random.Range(Definition.GoldMin, Definition.GoldMax + 1);
            gold = Mathf.Max(0, gold);
            if (gold > 0)
            {
                var remaining = gold;
                for (var i = 0; i < contributors.Count; i++)
                {
                    var share = Mathf.FloorToInt(gold * (contributors[i].dmg / total));
                    if (i == contributors.Count - 1)
                    {
                        share = remaining;
                    }
                    remaining -= share;
                    if (share > 0)
                    {
                        contributors[i].hero.AddGold(share);
                    }
                }
            }

                                                               
        }
    }
}
