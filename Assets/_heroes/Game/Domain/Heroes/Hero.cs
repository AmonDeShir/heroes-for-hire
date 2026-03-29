using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions;
using Heroes.Game.Abstractions.Common;
using Heroes.Game.Abstractions.Heroes;
using Heroes.Game.Abstractions.Items;
using Heroes.Game.Core;
using Heroes.Game.Domain.Common;
using Heroes.Game.Domain.Items;

namespace Heroes.Game.Domain.Heroes
{
    public class Hero : IHasHealth, IHero
    {
        public EntityId Id { get; }
        public IHealthComponent Health { get; }
        public IEntityDefinition Definition { get; }
        
        public HeroState State { get; private set; }
        public float NormalizedSpeed { get; private set; }
        public IReadOnlyList<IItemInstance> Inventory => inventory;
        public IReadOnlyList<IItemInstance> EquippedWeapons => equippedWeapons;
        public IReadOnlyList<IItemInstance> EquippedArmor => equippedArmor;
        public IReadOnlyList<IItemInstance> EquippedArtifacts => equippedArtifacts;

        private readonly List<ItemInstance> inventory = new();
        private readonly List<ItemInstance> equippedWeapons = new();
        private readonly List<ItemInstance> equippedArmor = new();
        private readonly List<ItemInstance> equippedArtifacts = new();
        
        public Hero(EntityId id, IEntityDefinition definition)
        {
            Id = id;
            Definition = definition;
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
