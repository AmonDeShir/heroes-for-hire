using Heroes.Game.Buildings;
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

        [Header("Cost")]
        public int GoldCost = 100;

        [Header("Construction")]
        public float MaxHp = 100f;
        public float BuildHpPerSecond = 10f;
        public float StartHp = 5f;

        [Header("Prefab")]
        public BuildingFacade Prefab;
    }
}
