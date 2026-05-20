using UnityEngine;

namespace Heroes.Content.Heroes
{
    [System.Serializable]
    public struct ItemEffectEntry
    {
        public ItemEffects.ItemEffectTrigger Trigger;
        public ItemEffects.ItemEffect Effect;
    }

    public enum EquipmentSlot
    {
        Item,
        Weapon,
        Armor,
    }

    public class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID]
        public string Id;
        public string DisplayName;
        
        [Multiline]
        public string Description;
        
        [ResourceIcon("Items")]
        public string IconPath;

        [Header("Stats")]
        public int GoldCost;

        [Tooltip("Progression tier for equipment upgrades (0..3). Used by GOAP to decide 'better' items.\n0 = none/unspecified.")]
        public int Tier;
        public float Attack;
        public float Defense;
        public float Speed;
        public float HpRegeneration;

        [Header("Limits")]
        public HeroDefinition Users;
        public EquipmentSlot Slot;
        public bool IsSingleUse;

        [Header("Effects")]
        public ItemEffectEntry[] Effects;

        [Header("Visuals")]
        
        
        public GameObject WeaponPrefab;
    }
}


