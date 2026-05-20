using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Set Castle Level")]
    public sealed class SetCastleLevelEffect : BuildingUpgradeEffect
    {
        public int CastleLevel = 1;

        public override void ApplyEffect(in BuildingUpgradeContext ctx)
        {
            if (ctx.Kingdom == null)
            {
                return;
            }

            ctx.Kingdom.TrySetCastleLevel(CastleLevel);
        }
    }
}


