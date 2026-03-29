using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IBuildingUpgradeDefinition : IDefinition
    {
        int TargetLevel { get; }
        int GoldCost { get; }
        float DurationSeconds { get; }

        IReadOnlyList<IBuildingStatChange> StatChanges { get; }
        IBuildingGoldGenerationChange GoldGeneration { get; }
        IReadOnlyList<IBuildingAutoSpawnChange> AutoSpawnChanges { get; }
        IBuildingAttackChange AttackChange { get; }
        IBuildingRecruitmentChange RecruitmentChange { get; }
        IBuildingShopChange ShopChange { get; }
        IBuildingUnlocksChange UnlocksChange { get; }
        IBuildingQueueChange QueueChange { get; }

        IReadOnlyList<IItemDefinition> UnlockItems { get; }
        IReadOnlyList<ISkillDefinition> UnlockSkills { get; }
    }

    public interface IBuildingStatChange
    {
        StatType Stat { get; }
        float Additive { get; }
        float Multiplier { get; }
    }

    public interface IBuildingGoldGenerationChange
    {
        bool HasValue { get; }
        int GoldPerTick { get; }
        float TickIntervalSeconds { get; }
    }

    public interface IBuildingAutoSpawnChange
    {
        IEntityDefinition Entity { get; }
        float RespawnDelaySeconds { get; }
        bool OnlyIfPreviousDead { get; }
        int MaxAlive { get; }
        bool Remove { get; }
    }

    public interface IBuildingAttackChange
    {
        bool HasValue { get; }
        ISkillDefinition AttackSkill { get; }
        float FallbackDamage { get; }
        float AttackIntervalSeconds { get; }
        float Range { get; }
        bool IsArea { get; }
        float AreaRadius { get; }
    }

    public interface IBuildingRecruitmentChange
    {
        bool HasValue { get; }
        IReadOnlyList<IHeroDefinition> RecruitableHeroes { get; }
        int MaxHeroes { get; }
    }

    public interface IBuildingShopChange
    {
        bool HasValue { get; }
        IReadOnlyList<IItemDefinition> PurchasableItems { get; }
    }

    public interface IBuildingUnlocksChange
    {
        bool HasValue { get; }
        IReadOnlyList<IItemDefinition> UnlockableItems { get; }
        IReadOnlyList<ISkillDefinition> UnlockableSkills { get; }
    }

    public interface IBuildingQueueChange
    {
        bool HasValue { get; }
        int Capacity { get; }
    }
}
