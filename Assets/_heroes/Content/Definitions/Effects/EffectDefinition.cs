using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Common;
using UnityEngine;

namespace Heroes.Content.Definitions.Effects
{
    [CreateAssetMenu(menuName = "Heroes/Effects/Effect Definition")]
    public class EffectDefinition : DefinitionBase, IEffectDefinition
    {
        [SerializeField] private EffectType effectType;
        [SerializeField] private float value;
        [SerializeField] private float durationSeconds;
        [SerializeField] private StatType affectedStat;
        [SerializeField] private bool hasAffectedStat;
        [SerializeField] private float healthDelta;
        [SerializeField] private float manaDelta;
        [SerializeField] private float staminaDelta;
        [SerializeField] private bool removeNegativeEffects;
        [SerializeField] private List<StatModifier> modifiers = new();

        public EffectType Type => effectType;
        public float Value => value;
        public float DurationSeconds => durationSeconds;
        public StatType? AffectedStat => hasAffectedStat ? affectedStat : null;
        public float HealthDelta => healthDelta;
        public float ManaDelta => manaDelta;
        public float StaminaDelta => staminaDelta;
        public bool RemoveNegativeEffects => removeNegativeEffects;
        public IReadOnlyList<IStatModifier> Modifiers =>
            DefinitionListUtility.ToInterfaceList<StatModifier, IStatModifier>(modifiers);
    }
}
