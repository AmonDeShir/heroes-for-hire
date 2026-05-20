using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Modify Building Stats")]
    public class ModifyBuildingStatsEffect : BuildingUpgradeEffect
    {
        public float HealthModifier;

        public override void ApplyEffect(in BuildingUpgradeContext ctx)
        {
            if (ctx.Model?.Health == null)
            {
                return;
            }

            ctx.Model.Health.SetMax(ctx.Model.Health.Max * HealthModifier);
        }
    }
}


