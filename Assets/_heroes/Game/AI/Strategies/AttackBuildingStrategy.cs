using Heroes.Game.Buildings;
using Heroes.Game.Heroes;
using Heroes.GOAP;
using Heroes.GOAP.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.AI.Strategies
{
    public sealed class AttackBuildingStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly AgentContext<GameWorldSnapshot> _ctx;
        private readonly BuildingFacade _building;
        private Vector3 _destination;
        private float _nextAttackAt;

        public bool CanPerform => _building != null && _building.IsAlive;
        public bool Complete { get; private set; }

        public AttackBuildingStrategy(Agent<GameWorldSnapshot, HeroAnimationController> agent, AgentContext<GameWorldSnapshot> ctx, BuildingFacade building)
        {
            _agent = agent;
            _ctx = ctx;
            _building = building;
            _destination = building != null ? building.DoorWorldPosition : agent.transform.position;
        }

        public void Start()
        {
            Complete = false;
            _nextAttackAt = 0f;

            if (_agent?.NavAgent != null)
            {
                if (NavMesh.SamplePosition(_destination, out var hit, 4f, NavMesh.AllAreas))
                {
                    _destination = hit.position;
                }
                _agent.NavAgent.SetDestination(_destination);
            }
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

            if (_building == null || !_building.IsAlive)
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

            if (Vector3.Distance(_agent.transform.position, _destination) > 1.5f)
            {
                _agent.NavAgent.SetDestination(_destination);
                return;
            }

            if (Time.unscaledTime < _nextAttackAt)
            {
                return;
            }

            var atk = hero.Definition != null ? hero.Definition.Attack : 1f;
            var dmg = Mathf.Max(0.1f, atk + hero.Model.EquipmentAttack + hero.Model.TimedAttack);

            _agent.Animator?.PlayAttack();
            _building.ApplyDamage(dmg);
            _nextAttackAt = Time.unscaledTime + 1.0f;
        }

        public void Stop()
        {
            if (_agent?.NavAgent != null)
            {
                _agent.NavAgent.ResetPath();
            }

            _ctx?.MutateState((ref AgentState s) => s.SetLocation(_agent.transform.position));
        }
    }
}
