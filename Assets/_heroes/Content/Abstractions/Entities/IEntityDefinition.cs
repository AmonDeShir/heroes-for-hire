using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IEntityDefinition :
        IDefinition,
        ILocalizedDefinition,
        IIconDefinition,
        IHealthDefinition
    {
        IStatBlock BaseStats { get; }
        IReadOnlyList<ISkillDefinition> Skills { get; }
        ISkillDefinition DefaultAttackSkill { get; }
    }
}
