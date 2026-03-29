using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface ISkillDefinition : IDefinition
    {
        float BaseDamage { get; }
        float CooldownSeconds { get; }
        float Range { get; }
        bool IsArea { get; }
        float AreaRadius { get; }
        SkillTargetFlags TargetFlags { get; }
        IReadOnlyList<IEffectDefinition> Effects { get; }
    }
}
