using System.Collections.Generic;

namespace Heroes.Content.Abstractions
{
    public interface IBuildingGoldGenerationDefinition
    {
        bool Enabled { get; }
        int GoldPerTick { get; }
        float TickIntervalSeconds { get; }
    }

    public interface IBuildingAutoSpawnDefinition
    {
        IEntityDefinition Entity { get; }
        float RespawnDelaySeconds { get; }
        bool OnlyIfPreviousDead { get; }
        int MaxAlive { get; }
    }

    public interface IBuildingAttackDefinition
    {
        bool Enabled { get; }
        ISkillDefinition AttackSkill { get; }
    }

    public interface IBuildingHeroRecruitmentDefinition
    {
        bool Enabled { get; }
        IReadOnlyList<IHeroDefinition> RecruitableHeroes { get; }
        int MaxHeroes { get; }
    }

    public interface IBuildingShopDefinition
    {
        bool Enabled { get; }
        IReadOnlyList<IItemDefinition> PurchasableItems { get; }
    }

    public interface IBuildingUnlocksDefinition
    {
        bool Enabled { get; }
        IReadOnlyList<IItemDefinition> UnlockableItems { get; }
        IReadOnlyList<ISkillDefinition> UnlockableSkills { get; }
    }

    public interface IBuildingQueueDefinition
    {
        bool Enabled { get; }
        int Capacity { get; }
    }
}
