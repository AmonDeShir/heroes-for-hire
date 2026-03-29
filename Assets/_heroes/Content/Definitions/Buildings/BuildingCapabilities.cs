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
    [Serializable]
    public struct BuildingGoldGenerationDefinition : IBuildingGoldGenerationDefinition
    {
        [SerializeField] private bool enabled;
        [SerializeField] private int goldPerTick;
        [SerializeField] private float tickIntervalSeconds;

        public bool Enabled => enabled;
        public int GoldPerTick => goldPerTick;
        public float TickIntervalSeconds => tickIntervalSeconds;
    }

    [Serializable]
    public struct BuildingAutoSpawnDefinition : IBuildingAutoSpawnDefinition
    {
        [SerializeField] private EntityDefinition entity;
        [SerializeField] private float respawnDelaySeconds;
        [SerializeField] private bool onlyIfPreviousDead;
        [SerializeField] private int maxAlive;

        public IEntityDefinition Entity => entity;
        public float RespawnDelaySeconds => respawnDelaySeconds;
        public bool OnlyIfPreviousDead => onlyIfPreviousDead;
        public int MaxAlive => maxAlive;
    }

    [Serializable]
    public struct BuildingAttackDefinition : IBuildingAttackDefinition
    {
        [SerializeField] private bool enabled;
        [SerializeField] private SkillDefinition attackSkill;

        public bool Enabled => enabled;
        public ISkillDefinition AttackSkill => attackSkill;
    }

    [Serializable]
    public struct BuildingHeroRecruitmentDefinition : IBuildingHeroRecruitmentDefinition
    {
        [SerializeField] private bool enabled;
        [SerializeField] private List<HeroDefinition> recruitableHeroes;
        [SerializeField] private int maxHeroes;

        public bool Enabled => enabled;
        public IReadOnlyList<IHeroDefinition> RecruitableHeroes => recruitableHeroes;
        public int MaxHeroes => maxHeroes;
    }

    [Serializable]
    public struct BuildingShopDefinition : IBuildingShopDefinition
    {
        [SerializeField] private bool enabled;
        [SerializeField] private List<ItemDefinition> purchasableItems;

        public bool Enabled => enabled;
        public IReadOnlyList<IItemDefinition> PurchasableItems => purchasableItems;
    }

    [Serializable]
    public struct BuildingUnlocksDefinition : IBuildingUnlocksDefinition
    {
        [SerializeField] private bool enabled;
        [SerializeField] private List<ItemDefinition> unlockableItems;
        [SerializeField] private List<SkillDefinition> unlockableSkills;

        public bool Enabled => enabled;
        public IReadOnlyList<IItemDefinition> UnlockableItems => unlockableItems;
        public IReadOnlyList<ISkillDefinition> UnlockableSkills => unlockableSkills;
    }

    [Serializable]
    public struct BuildingQueueDefinition : IBuildingQueueDefinition
    {
        [SerializeField] private bool enabled;
        [SerializeField] private int capacity;

        public bool Enabled => enabled;
        public int Capacity => capacity;
    }
}
