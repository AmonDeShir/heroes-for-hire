using System.Collections.Generic;
using Heroes.Game.Abstractions.Entities;
using Heroes.Game.Abstractions.Items;

namespace Heroes.Game.Abstractions.Heroes
{
    public interface IHero : IEntity
    {
        HeroState State { get; }
        float NormalizedSpeed { get; }
        IReadOnlyList<IItemInstance> Inventory { get; }
        IReadOnlyList<IItemInstance> EquippedWeapons { get; }
        IReadOnlyList<IItemInstance> EquippedArmor { get; }
        IReadOnlyList<IItemInstance> EquippedArtifacts { get; }
    }
}
