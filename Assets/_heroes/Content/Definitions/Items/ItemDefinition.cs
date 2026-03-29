using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Effects;
using Heroes.Content.Definitions.Skills;
using UnityEngine;

namespace Heroes.Content.Definitions.Items
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Definition")]
    public class ItemDefinition : DefinitionBase, IItemDefinition
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private int goldCost;
        [SerializeField] private bool isConsumable;
        [SerializeField] private List<EffectDefinition> effects = new();
        [SerializeField] private List<SkillDefinition> grantedSkills = new();

        public ItemType ItemType => itemType;
        public int GoldCost => goldCost;
        public bool IsConsumable => isConsumable;
        public IReadOnlyList<IEffectDefinition> Effects => effects;
        public IReadOnlyList<ISkillDefinition> GrantedSkills => grantedSkills;
    }
}
