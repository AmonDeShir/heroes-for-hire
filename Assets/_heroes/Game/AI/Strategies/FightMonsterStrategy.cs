using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using Heroes.Game.Monsters;
using Heroes.GOAP;
using Heroes.GOAP.Core;

namespace Heroes.Game.AI.Strategies
{
    public sealed class FightMonsterStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private readonly AgentContext<GameWorldSnapshot> _ctx;
        private readonly MonsterFacade _monster;

        public FightMonsterStrategy(
            Agent<GameWorldSnapshot, HeroAnimationController> agent,
            AgentContext<GameWorldSnapshot> ctx,
            MonsterFacade monster,
            CombatService combat)
        {
            _agent = agent;
            _ctx = ctx;
            _monster = monster;
        }

        public bool CanPerform => _monster != null && _monster.IsAlive;
        public bool Complete { get; private set; }

        public void Start()
        {
            Complete = false;

            var hero = _agent != null ? _agent.GetComponent<HeroFacade>() : null;
            hero?.CombatController?.StartCombat(_monster, HeroCombatIntent.Hunt);
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
            Complete = controller == null || (!controller.HasPrimaryTarget(_monster) && !controller.IsActive);
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
