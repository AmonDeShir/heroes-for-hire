using Heroes.Game.Buildings;
using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using Heroes.GOAP;
using Heroes.GOAP.Core;

namespace Heroes.Game.AI.Strategies
{
    public sealed class AttackBuildingStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly AgentContext<GameWorldSnapshot> _ctx;
        private readonly BuildingFacade _building;

        public bool CanPerform => _building != null && _building.IsAlive;
        public bool Complete { get; private set; }

        public AttackBuildingStrategy(Agent<GameWorldSnapshot, HeroAnimationController> agent, AgentContext<GameWorldSnapshot> ctx, BuildingFacade building)
        {
            _agent = agent;
            _ctx = ctx;
            _building = building;
        }

        public void Start()
        {
            Complete = false;

            var hero = _agent != null ? _agent.GetComponent<HeroFacade>() : null;
            hero?.CombatController?.StartCombat(_building, HeroCombatIntent.AttackBuilding);
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

            var controller = hero.CombatController;
            Complete = controller == null || (!controller.HasPrimaryTarget(_building) && !controller.IsActive);
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
