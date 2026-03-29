using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Effects;
using UnityEngine;

namespace Heroes.Content.Definitions.Skills
{
    [CreateAssetMenu(menuName = "Heroes/Skills/Skill Definition")]
    public class SkillDefinition : DefinitionBase, ISkillDefinition
    {
        [SerializeField] private float baseDamage;
        [SerializeField] private float cooldownSeconds;
        [SerializeField] private float range = 1f;
        [SerializeField] private bool isArea;
        [SerializeField] private float areaRadius;
        [SerializeField] private SkillTargetFlags targetFlags = SkillTargetFlags.Enemies;
        [SerializeField] private List<EffectDefinition> effects = new();

        public float BaseDamage => baseDamage;
        public float CooldownSeconds => cooldownSeconds;
        public float Range => range;
        public bool IsArea => isArea;
        public float AreaRadius => areaRadius;
        public SkillTargetFlags TargetFlags => targetFlags;
        public IReadOnlyList<IEffectDefinition> Effects => effects;
    }
}
