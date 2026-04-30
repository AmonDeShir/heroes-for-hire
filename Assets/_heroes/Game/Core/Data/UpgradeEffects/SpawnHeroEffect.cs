using Heroes.Content.Heroes;
using Heroes.Game.Buildings;
using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Spawn Hero")]
    public class SpawnHeroEffect : BuildingUpgradeEffect
    {
        public HeroDefinition Hero;
        public int Count = 1;

        public override void ApplyEffect(BuildingModel model)
        {
        }
    }
}
