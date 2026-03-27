using Heroes.Game.Abstractions;
using Heroes.Game.Abstractions.Common;
using Heroes.Game.Abstractions.Heroes;
using Heroes.Game.Core;
using Heroes.Game.Domain.Common;

namespace Heroes.Game.Domain.Heroes
{
    public class Hero : IHasHealth, IHero
    {
        public EntityId Id { get; }
        public IHealthComponent Health { get; }
        
        public HeroState State { get; private set; }
        public float NormalizedSpeed { get; private set; }
        
        public Hero(EntityId id, IHealthDefinition definition)
        {
            Id = id;
            State = HeroState.Idle;
            NormalizedSpeed = 0.0f;
            Health = new HealthComponent(
                definition.MaxHealth,
                definition.SpawnHealth,
                definition.BaseRegeneration);
        }
        
        public void SetMovement(float normalizedSpeed)
        {
            NormalizedSpeed = normalizedSpeed;
            State = normalizedSpeed > 0.01f ? HeroState.Moving : HeroState.Idle;
        }

        public void BeginAttack() => State = HeroState.Attacking;
        public void BeginCast() => State = HeroState.Casting;
        public void Die() => State = HeroState.Dead;
        
        
    }
}
