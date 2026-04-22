using Heroes.Game.Buildings;
using UnityEngine;

namespace Heroes.Content.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Definition")]
    public class BuildingDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string Id;
        public string DisplayName;
        public string Description;
        public BuildingCategory Category;
        
        [Header("Upgrades")]
        public BuildingUpgradeDefinition[] AvailableUpgrades;

        [Header("Cost")]
        public int GoldCost = 100;

        [Header("Construction")]
        public float MaxHp = 100f;
        public float BuildHpPerSecond = 10f;
        public float StartHp = 5f;
        
        [Header("Visuals")]
        public string IconPath;

        [Header("Prefab")]
        public BuildingFacade Prefab;
    }
}
