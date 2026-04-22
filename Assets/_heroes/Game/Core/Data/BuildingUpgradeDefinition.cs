using Heroes.Content.Buildings.UpgradeEffects;
using UnityEngine;

namespace Heroes.Content.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade")]
    public class BuildingUpgradeDefinition : ScriptableObject
    {
        public string Id;
        public string Name;
        public string Description;
        public string IconPath;
        public float GoldCost;
        public float Duration;
        public BuildingUpgradeDefinition[] UpgradeRequirements;
        public BuildingUpgradeDefinition[] UnlocksUpgrades;
        public int UsageLimit = 1;
        public BuildingUpgradeEffect[] Effects;
    }
}