using Heroes.Game.Buildings;
using Heroes.Game.Heroes;
using Heroes.GOAP;
using Heroes.GOAP.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Heroes.Game.AI.Strategies
{
    public sealed class DefendBuildingStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly AgentContext<GameWorldSnapshot> _ctx;
        private readonly BuildingFacade _building;

        private Vector3 _destination;

        public bool CanPerform => _building != null && _building.IsAlive;
        public bool Complete { get; private set; }

        public DefendBuildingStrategy(
            Agent<GameWorldSnapshot, HeroAnimationController> agent,
            AgentContext<GameWorldSnapshot> ctx,
            BuildingFacade building)
        {
            _agent = agent;
            _ctx = ctx;
            _building = building;
            _destination = building != null ? building.DoorWorldPosition : agent.transform.position;
        }

        public void Start()
        {
            Complete = false;
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

            if (Vector3.Distance(_agent.transform.position, _destination) > 1.25f)
            {
                return;
            }

            var hero = _agent.GetComponent<HeroFacade>();
            hero?.Model?.SetDefendBuilding(string.Empty, 0f);
            Complete = true;
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
