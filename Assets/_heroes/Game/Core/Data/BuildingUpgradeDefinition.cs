using Heroes.Content.Buildings.UpgradeEffects;
using UnityEngine;

namespace Heroes.Content.Buildings
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade")]
    public class BuildingUpgradeDefinition : ScriptableObject
    {
        [Header("Identity")]
        [GUID]
        public string Id;
        public string Name;
        
        [Multiline]
        public string Description;
        
        [ResourceIcon("Icons")]
        public string IconPath;
        
        [Header("Stats")] 
        public float GoldCost;
        public float Duration;
        public int UsageLimit = 1;
    
        [Header("Effects")]
        public BuildingUpgradeEffect[] Effects;
        
        [Header("Requirements")]
        public BuildingUpgradeDefinition[] UpgradeRequirements;
        public BuildingUpgradeDefinition[] UnlocksUpgrades;
    }
}