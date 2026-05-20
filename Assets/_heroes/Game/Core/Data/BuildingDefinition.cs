using Heroes.Game.Buildings;
using Heroes.Content.Heroes;
using UnityEngine;

namespace Heroes.Content.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID]
        public string Id;
        public string DisplayName;
        public BuildingCategory Category;
        
        [Multiline]
        public string Description;
        
        [ResourceIcon("Buildings")]
        public string IconPath;

        [Header("Upgrades")]
        public BuildingUpgradeDefinition[] AvailableUpgrades;

        [Header("Shop")]
        public ItemDefinition[] SellItems;

        [Header("Cost")]
        public int GoldCost = 100;

        [Tooltip("How much population this building consumes when placed.")]
        public int PopulationCost = 1;

        [Tooltip("How much population this building provides while it exists (e.g. House).")]
        public int PopulationProvided;

        [Tooltip("If false, building is not placeable by the player.")]
        public bool IsPlayerBuildable = true;

        [Tooltip("Minimum castle level required to build this building. 1 means available from the start.")]
        public int RequiredCastleLevel = 1;

        [Header("Construction")]
        public float MaxHp = 100f;
        public float BuildHpPerSecond = 10f;
        public float StartHp = 5f;

        [Header("Economy")]
        [Tooltip("If > 0, building generates gold directly into the treasury periodically.")]
        public int GoldIncomePerTick;

        [Tooltip("Seconds between gold income ticks.")]
        public float GoldIncomeIntervalSeconds;

        [Header("Prefab")]
        public BuildingFacade Prefab;
    }
}


