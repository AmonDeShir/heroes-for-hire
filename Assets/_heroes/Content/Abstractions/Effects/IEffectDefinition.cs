using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IEffectDefinition : IDefinition
    {
        EffectType Type { get; }
        float Value { get; }
        float DurationSeconds { get; }
        StatType? AffectedStat { get; }
        float HealthDelta { get; }
        float ManaDelta { get; }
        float StaminaDelta { get; }
        bool RemoveNegativeEffects { get; }
        IReadOnlyList<IStatModifier> Modifiers { get; }
    }
}
