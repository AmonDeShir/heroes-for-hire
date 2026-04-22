using Heroes.Game.Buildings;
using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Modify Building Stats")]
    public class ModifyBuildingStatsEffect : BuildingUpgradeEffect
    {
        public float HealthModifier;

        public override void ApplyEffect(BuildingModel model)
        {
            model.Health.SetMax(model.Health.Max * HealthModifier);
        }
    }
}
