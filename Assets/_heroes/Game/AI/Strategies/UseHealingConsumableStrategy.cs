using Heroes.Game.Combat;
using Heroes.Game.Heroes;
using Heroes.GOAP;
using Heroes.GOAP.Core;

namespace Heroes.Game.AI.Strategies
{
    public sealed class UseHealingConsumableStrategy : IActionStrategy
    {
        private readonly Agent<GameWorldSnapshot, HeroAnimationController> _agent;
        private bool _used;

        public bool CanPerform => !_used;
        public bool Complete { get; private set; }

        public UseHealingConsumableStrategy(Agent<GameWorldSnapshot, HeroAnimationController> agent)
        {
            _agent = agent;
        }

        public void Start()
        {
            Complete = false;
            _used = false;
        }

        public void Update(float deltaTime)
        {
            if (Complete || _used || _agent == null)
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

            CombatRuntimeConfig.Service?.TryUseHealingConsumable(hero);
            _used = true;
            Complete = true;
        }

        public void Stop()
        {
        }
    }
}
