using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Common;
using UnityEngine;

namespace Heroes.Content.Definitions.Entities
{
    [CreateAssetMenu(menuName = "Heroes/Entities/Hero Definition")]
    public class HeroDefinition : EntityDefinition, IHeroDefinition
    {
        [SerializeField] private int maxWeaponSlots = 1;
        [SerializeField] private int maxArmorSlots = 1;
        [SerializeField] private int maxArtifactSlots = 3;
        [SerializeField] private List<ItemType> allowedItemTypes = new()
        {
            ItemType.Weapon,
            ItemType.Armor,
            ItemType.Artifact,
            ItemType.Consumable,
        };

        public int MaxWeaponSlots => maxWeaponSlots;
        public int MaxArmorSlots => maxArmorSlots;
        public int MaxArtifactSlots => maxArtifactSlots;
        public IReadOnlyList<ItemType> AllowedItemTypes => allowedItemTypes;
    }
}
