using Heroes.Content.Heroes;
using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Spawn Hero")]
    public class SpawnHeroEffect : BuildingUpgradeEffect
    {
        public HeroDefinition Hero;
        public int Count = 1;

        public override void ApplyEffect(in BuildingUpgradeContext ctx)
        {
            if (ctx.HeroSpawn == null || ctx.Building == null || Hero == null)
            {
                return;
            }

            var count = Count <= 0 ? 1 : Count;
            for (var i = 0; i < count; i++)
            {
                ctx.HeroSpawn.Spawn(Hero, ctx.Building);
            }
        }
    }
}


