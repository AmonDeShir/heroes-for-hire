using Heroes.Content.Buildings;
using Heroes.Game.Buildings;
using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    public readonly struct BuildingUpgradeContext
    {
        public readonly BuildingFacade Building;
        public readonly BuildingModel Model;
        public readonly BuildingDefinition Definition;
        public readonly KingdomService Kingdom;
        public readonly HeroSpawnService HeroSpawn;

        public BuildingUpgradeContext(BuildingFacade building, KingdomService kingdom, HeroSpawnService heroSpawn)
        {
            Building = building;
            Model = building != null ? building.Model : null;
            Definition = building != null ? building.Definition : null;
            Kingdom = kingdom;
            HeroSpawn = heroSpawn;
        }
    }

    public abstract class BuildingUpgradeEffect : ScriptableObject
    {
        public abstract void ApplyEffect(in BuildingUpgradeContext ctx);
    }
}


