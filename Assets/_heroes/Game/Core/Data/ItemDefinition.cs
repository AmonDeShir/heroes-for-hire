using UnityEngine;

namespace Heroes.Content.Heroes
{
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
        public float Attack;
        public float Defense;
        public float Speed;

        [Header("Limits")]
        public HeroDefinition Users;
        public EquipmentSlot Slot;
        public bool IsSingleUse;
    }
}