using System;
using Heroes.Game.Abstractions.Heroes;

namespace Heroes.Presentation.World.Hero
{
    public class HeroPresenter
    {
        private readonly IHero _hero;
        private readonly IHeroAnimationDriver _animationDriver;

        private HeroState _lastState;

        public HeroPresenter(IHero hero, IHeroAnimationDriver animationDriver)
        {
            _hero = hero;
            _animationDriver = animationDriver;
            _lastState = hero.State;
        }

        public void Tick()
        {
            _animationDriver.SetSpeed(_hero.NormalizedSpeed);

            if (_hero.State == _lastState)
            {
                return;
            }

            switch (_hero.State)
            {
                case HeroState.Attacking:
                    _animationDriver.PlayAttack();
                    break;
                
                case HeroState.Casting:
                    _animationDriver.PlayCast();
                    break;
                
                case HeroState.Dead:
                    _animationDriver.PlayDeath();
                    break;
                
                case HeroState.Idle:
                case HeroState.Moving: 
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            _lastState = _hero.State;
        }
    }
}
