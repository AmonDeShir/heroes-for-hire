using System.Collections.Generic;
using Heroes.Content.Definitions.Buildings;
using UnityEngine;

namespace Heroes.Content.Abstractions
{
    public interface IBuildingDefinition :
        IDefinition,
        ILocalizedDefinition,
        IIconDefinition,
        IHealthDefinition
    {
        public BuildingCategory Category { get; }
        public int GoldCost { get; }
        
        public GameObject Prefab { get; }

        public IBuildingGoldGenerationDefinition GoldGeneration { get; }
        public IReadOnlyList<IBuildingAutoSpawnDefinition> AutoSpawns { get; }
        public IBuildingAttackDefinition Attack { get; }
        public IBuildingHeroRecruitmentDefinition HeroRecruitment { get; }
        public IBuildingShopDefinition Shop { get; }
        public IBuildingUnlocksDefinition Unlocks { get; }
        public IReadOnlyList<IBuildingUpgradeDefinition> Upgrades { get; }
        public IBuildingQueueDefinition Queue { get; }
    }
}
