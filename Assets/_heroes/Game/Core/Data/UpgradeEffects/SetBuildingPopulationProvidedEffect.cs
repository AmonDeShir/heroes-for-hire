using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Set Building Population")]
    public sealed class SetBuildingPopulationProvidedEffect : BuildingUpgradeEffect
    {
        public int PopulationProvided;

        public override void ApplyEffect(in BuildingUpgradeContext ctx)
        {
            if (ctx.Model == null || ctx.Kingdom == null || ctx.Building == null)
            {
                return;
            }

            var value = PopulationProvided < 0 ? 0 : PopulationProvided;
            ctx.Model.SetPopulationProvided(value);
            ctx.Kingdom.SetPopulationContribution(ctx.Building.Id, value);
        }
    }
}


