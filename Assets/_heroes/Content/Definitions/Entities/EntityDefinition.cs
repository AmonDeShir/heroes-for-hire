using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Skills;
using UnityEngine;

namespace Heroes.Content.Definitions.Entities
{
    [CreateAssetMenu(menuName = "Heroes/Entities/Entity Definition")]
    public class EntityDefinition : DefinitionBase, IEntityDefinition, IHealthDefinition
    {
        [SerializeField] private StatBlock baseStats;
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float spawnHealth = 10f;
        [SerializeField] private float baseRegeneration = 1f;
        [SerializeField] private List<SkillDefinition> skills = new();
        [SerializeField] private SkillDefinition defaultAttackSkill;

        public IStatBlock BaseStats => baseStats;
        public float MaxHealth => maxHealth;
        public float SpawnHealth => spawnHealth;
        public float BaseRegeneration => baseRegeneration;
        public IReadOnlyList<ISkillDefinition> Skills => skills;
        public ISkillDefinition DefaultAttackSkill => defaultAttackSkill;
    }
}
