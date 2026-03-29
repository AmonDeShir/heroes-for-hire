using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IItemDefinition : IDefinition, IIconDefinition
    {
        ItemType ItemType { get; }
        int GoldCost { get; }
        bool IsConsumable { get; }
        IReadOnlyList<IEffectDefinition> Effects { get; }
        IReadOnlyList<ISkillDefinition> GrantedSkills { get; }
    }
}
