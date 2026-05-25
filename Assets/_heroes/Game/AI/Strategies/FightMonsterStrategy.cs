using Heroes.Game.Heroes;
using Heroes.Game.Monsters;
using Heroes.Game.Combat;
using Heroes.GOAP;
using Heroes.GOAP.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.AI.Strategies
{
    public sealed class FightMonsterStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly AgentContext<GameWorldSnapshot> _ctx;
        private readonly MonsterFacade _monster;
        private readonly CombatService _combat;

        private float _nextAttackAt;

        private bool _fleeing;
        private float _fleeUntil;
        private Vector3 _fleeDestination;
        private float _nextFleeRerollAt;
        private float _lastFleeProgressAt;
        private float _lastFleeRemaining = float.MaxValue;

        private float _nextHealAt;

        public FightMonsterStrategy(
            Agent<GameWorldSnapshot, HeroAnimationController> agent,
            AgentContext<GameWorldSnapshot> ctx,
            MonsterFacade monster,
            CombatService combat)
        {
            _agent = agent;
            _ctx = ctx;
            _monster = monster;
            _combat = combat ?? CombatRuntimeConfig.Service;
        }

        public bool CanPerform => _monster != null && _monster.IsAlive;
        public bool Complete { get; private set; }

        public void Start()
        {
            Complete = false;
            _nextAttackAt = 0f;

            _fleeing = false;
            _fleeUntil = 0f;
            _nextFleeRerollAt = 0f;
            _lastFleeProgressAt = 0f;
            _lastFleeRemaining = float.MaxValue;
            _nextHealAt = 0f;

            var hero = _agent != null ? _agent.GetComponent<HeroFacade>() : null;
            hero?.SetCombatTarget(_monster);
        }

        public void Update(float deltaTime)
        {
            if (Complete || _agent == null || _ctx == null)
            {
                Complete = true;
                return;
            }

            var hero = _agent.GetComponent<HeroFacade>();
            if (hero?.Model == null || !hero.Model.IsAlive)
            {
                Complete = true;
                return;
            }

            _ctx.MutateState((ref AgentState s) => s.SetLocation(_agent.transform.position));

            if (_monster == null || !_monster.IsAlive)
            {
                Complete = true;
                return;
            }

            var dist = Vector3.Distance(_agent.transform.position, _monster.transform.position);
            var desired = _monster.Definition != null ? Mathf.Max(1.0f, _monster.Definition.AttackRange) : 1.5f;

            var hpPct = hero.Model.Health.Max > 0.001f ? hero.Model.Health.Current / hero.Model.Health.Max : 1f;

            var now2 = Time.unscaledTime;
            if (!_fleeing && hpPct <= 0.40f && now2 >= _nextHealAt)
            {
                if (_combat?.TryUseHealingConsumable(hero) == true)
                {
                    _nextHealAt = now2 + 2.0f;
                }
            }

            if (hpPct <= 0.20f && !_fleeing)
            {
                _fleeing = true;
                _combat?.TryUseSpeedConsumable(hero);
                var now = Time.unscaledTime;
                _fleeUntil = now + 8f;
                _fleeDestination = PickFleePoint(hero.transform.position);
                _agent.NavAgent?.SetDestination(_fleeDestination);
                _nextFleeRerollAt = now + 5f;
                _lastFleeProgressAt = now;
                _lastFleeRemaining = float.MaxValue;
                return;
            }

            if (hpPct <= 0.20f && !_fleeing)
            {
                _fleeing = true;
                _combat?.TryUseSpeedConsumable(hero);
                var now = Time.unscaledTime;
                _fleeUntil = now + 8f;
                _fleeDestination = PickFleePoint(hero.transform.position);
                _agent.NavAgent?.SetDestination(_fleeDestination);
                _nextFleeRerollAt = now + 5f;
                _lastFleeProgressAt = now;
                _lastFleeRemaining = float.MaxValue;
                return;
            }

            if (_fleeing)
            {
                var now = Time.unscaledTime;
                if (now >= _fleeUntil)
                {
                    Complete = true;
                    return;
                }

                if (_agent.NavAgent == null)
                {
                    Complete = true;
                    return;
                }

                if (_agent.NavAgent.pathPending)
                {
                    return;
                }

                var remaining = _agent.NavAgent.remainingDistance;
                if (remaining + 0.05f < _lastFleeRemaining)
                {
                    _lastFleeRemaining = remaining;
                    _lastFleeProgressAt = now;
                }

                if (remaining <= 2f)
                {
                    Complete = true;
                    return;
                }

                if (now >= _nextFleeRerollAt || (now - _lastFleeProgressAt) >= 5f || !_agent.NavAgent.hasPath)
                {
                    _fleeDestination = PickFleePoint(hero.transform.position);
                    _agent.NavAgent.SetDestination(_fleeDestination);
                    _nextFleeRerollAt = now + 5f;
                    _lastFleeProgressAt = now;
                    _lastFleeRemaining = float.MaxValue;
                }

                return;
            }

            if (dist > desired)
            {
                if (_agent.NavAgent != null)
                {
                    _agent.NavAgent.SetDestination(_monster.transform.position);
                }
                return;
            }

            if (Time.unscaledTime < _nextAttackAt)
            {
                return;
            }

            var atk = hero.Definition != null ? hero.Definition.Attack : 1f;
            var dmg = Mathf.Max(0.1f, atk + hero.Model.EquipmentAttack + hero.Model.TimedAttack);

            var to = _monster.transform.position - _agent.transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
            {
                var desiredRot = Quaternion.LookRotation(to.normalized, Vector3.up);
                _agent.transform.rotation = Quaternion.RotateTowards(_agent.transform.rotation, desiredRot, 720f * Time.deltaTime);
            }

            _agent.Animator?.PlayAttack();
            _monster.ApplyDamageFrom(hero.Model.InstanceId, dmg);
            _nextAttackAt = Time.unscaledTime + 1.0f;
        }

        public void Stop()
        {
            var hero = _agent != null ? _agent.GetComponent<HeroFacade>() : null;
            hero?.ClearCombatTarget();

            if (_agent?.NavAgent != null)
            {
                _agent.NavAgent.ResetPath();
            }

            _ctx?.MutateState((ref AgentState s) => s.SetLocation(_agent.transform.position));
        }

        private static Vector3 PickFleePoint(Vector3 origin)
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
            if (NavMesh.SamplePosition(candidate, out var h2, 25f, NavMesh.AllAreas))
            {
                return h2.position;
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

            var roll = Random.value * total;
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
    }
}
