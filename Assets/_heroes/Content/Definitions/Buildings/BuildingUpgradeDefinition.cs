using System;
using System.Collections.Generic;
using Heroes.Content.Abstractions;
using Heroes.Content.Definitions.Common;
using Heroes.Content.Definitions.Entities;
using Heroes.Content.Definitions.Items;
using Heroes.Content.Definitions.Skills;
using UnityEngine;

namespace Heroes.Content.Definitions.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Definition")]
    public class BuildingUpgradeDefinition : DefinitionBase, IBuildingUpgradeDefinition
    {
        [SerializeField] private int targetLevel;
        [SerializeField] private int goldCost;
        [SerializeField] private float durationSeconds = 10f;
        [SerializeField] private List<BuildingStatChange> statChanges = new();
        [SerializeField] private BuildingGoldGenerationChange goldGeneration;
        [SerializeField] private List<BuildingAutoSpawnChange> autoSpawnChanges = new();
        [SerializeField] private BuildingAttackChange attackChange;
        [SerializeField] private BuildingRecruitmentChange recruitmentChange;
        [SerializeField] private BuildingShopChange shopChange;
        [SerializeField] private BuildingUnlocksChange unlocksChange;
        [SerializeField] private BuildingQueueChange queueChange;
        [SerializeField] private List<ItemDefinition> unlockItems = new();
        [SerializeField] private List<SkillDefinition> unlockSkills = new();

        public int TargetLevel => targetLevel;
        public int GoldCost => goldCost;
        public float DurationSeconds => durationSeconds;
        public IReadOnlyList<IBuildingStatChange> StatChanges =>
            DefinitionListUtility.ToInterfaceList<BuildingStatChange, IBuildingStatChange>(statChanges);
        public IBuildingGoldGenerationChange GoldGeneration => goldGeneration;
        public IReadOnlyList<IBuildingAutoSpawnChange> AutoSpawnChanges =>
            DefinitionListUtility.ToInterfaceList<BuildingAutoSpawnChange, IBuildingAutoSpawnChange>(autoSpawnChanges);
        public IBuildingAttackChange AttackChange => attackChange;
        public IBuildingRecruitmentChange RecruitmentChange => recruitmentChange;
        public IBuildingShopChange ShopChange => shopChange;
        public IBuildingUnlocksChange UnlocksChange => unlocksChange;
        public IBuildingQueueChange QueueChange => queueChange;
        public IReadOnlyList<IItemDefinition> UnlockItems => unlockItems;
        public IReadOnlyList<ISkillDefinition> UnlockSkills => unlockSkills;
    }

    [Serializable]
    public struct BuildingStatChange : IBuildingStatChange
    {
        [SerializeField] private StatType stat;
        [SerializeField] private float additive;
        [SerializeField] private float multiplier;

        public StatType Stat => stat;
        public float Additive => additive;
        public float Multiplier => multiplier;
    }

    [Serializable]
    public struct BuildingGoldGenerationChange : IBuildingGoldGenerationChange
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private int goldPerTick;
        [SerializeField] private float tickIntervalSeconds;

        public bool HasValue => hasValue;
        public int GoldPerTick => goldPerTick;
        public float TickIntervalSeconds => tickIntervalSeconds;
    }

    [Serializable]
    public struct BuildingAutoSpawnChange : IBuildingAutoSpawnChange
    {
        [SerializeField] private EntityDefinition entity;
        [SerializeField] private float respawnDelaySeconds;
        [SerializeField] private bool onlyIfPreviousDead;
        [SerializeField] private int maxAlive;
        [SerializeField] private bool remove;

        public IEntityDefinition Entity => entity;
        public float RespawnDelaySeconds => respawnDelaySeconds;
        public bool OnlyIfPreviousDead => onlyIfPreviousDead;
        public int MaxAlive => maxAlive;
        public bool Remove => remove;
    }

    [Serializable]
    public struct BuildingAttackChange : IBuildingAttackChange
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private SkillDefinition attackSkill;
        [SerializeField] private float fallbackDamage;
        [SerializeField] private float attackIntervalSeconds;
        [SerializeField] private float range;
        [SerializeField] private bool isArea;
        [SerializeField] private float areaRadius;

        public bool HasValue => hasValue;
        public ISkillDefinition AttackSkill => attackSkill;
        public float FallbackDamage => fallbackDamage;
        public float AttackIntervalSeconds => attackIntervalSeconds;
        public float Range => range;
        public bool IsArea => isArea;
        public float AreaRadius => areaRadius;
    }

    [Serializable]
    public struct BuildingRecruitmentChange : IBuildingRecruitmentChange
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private List<HeroDefinition> recruitableHeroes;
        [SerializeField] private int maxHeroes;

        public bool HasValue => hasValue;
        public IReadOnlyList<IHeroDefinition> RecruitableHeroes => recruitableHeroes;
        public int MaxHeroes => maxHeroes;
    }

    [Serializable]
    public struct BuildingShopChange : IBuildingShopChange
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private List<ItemDefinition> purchasableItems;

        public bool HasValue => hasValue;
        public IReadOnlyList<IItemDefinition> PurchasableItems => purchasableItems;
    }

    [Serializable]
    public struct BuildingUnlocksChange : IBuildingUnlocksChange
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private List<ItemDefinition> unlockableItems;
        [SerializeField] private List<SkillDefinition> unlockableSkills;

        public bool HasValue => hasValue;
        public IReadOnlyList<IItemDefinition> UnlockableItems => unlockableItems;
        public IReadOnlyList<ISkillDefinition> UnlockableSkills => unlockableSkills;
    }

    [Serializable]
    public struct BuildingQueueChange : IBuildingQueueChange
    {
        [SerializeField] private bool hasValue;
        [SerializeField] private int capacity;

        public bool HasValue => hasValue;
        public int Capacity => capacity;
    }
}
