using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IHeroDefinition : IEntityDefinition
    {
        int MaxWeaponSlots { get; }
        int MaxArmorSlots { get; }
        int MaxArtifactSlots { get; }
        IReadOnlyList<ItemType> AllowedItemTypes { get; }
    }
}
