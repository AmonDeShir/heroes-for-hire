using Heroes.Game.Heroes;
using Heroes.GOAP.Core;

namespace Heroes.Game.AI.Strategies
{
    public sealed class ActiveCombatStrategy : IActionStrategy
    {
        private readonly HeroFacade _hero;

        public ActiveCombatStrategy(HeroFacade hero)
        {
            _hero = hero;
        }

        public bool CanPerform => _hero != null;
        public bool Complete { get; private set; }

        public void Start()
        {
            Complete = _hero == null;
        }

        public void Update(float deltaTime)
        {
            Complete = _hero == null || _hero.CombatController == null || !_hero.CombatController.IsActive;
        }

        public void Stop()
        {
        }
    }
}
