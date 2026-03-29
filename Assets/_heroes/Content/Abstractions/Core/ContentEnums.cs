using System;

namespace Heroes.Content.Abstractions
{
    [Flags]
    public enum SkillTargetFlags
    {
        None = 0,
        Self = 1 << 0,
        Allies = 1 << 1,
        Enemies = 1 << 2,
        Buildings = 1 << 3,
    }

    public enum EffectType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        Cleanse,
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        Artifact,
        Consumable,
    }

    public enum StatType
    {
        Strength,
        Agility,
        Intelligence,
        Endurance,
        Luck,
        Wisdom,
    }
}
