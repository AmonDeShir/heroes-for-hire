using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Multiply Gold Income")]
    public class MultiplyBuildingGoldIncomeEffect : BuildingUpgradeEffect
    {
        [Min(0.01f)]
        public float Multiplier = 2f;

        public override void ApplyEffect(in BuildingUpgradeContext ctx)
        {
            if (ctx.Model == null)
            {
                return;
            }

            ctx.Model.MultiplyGoldIncome(Multiplier);
        }
    }
}


