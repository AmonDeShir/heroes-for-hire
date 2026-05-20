using Heroes.Content.Heroes;
using Heroes.Game.Buildings;
using UnityEngine;

namespace Heroes.Content.Buildings.UpgradeEffects
{
    [CreateAssetMenu(menuName = "Heroes/Buildings/Building Upgrade Effects/Unlock Shop Items")]
    public class UnlockShopItemsEffect : BuildingUpgradeEffect
    {
        public ItemDefinition[] Items;

        public override void ApplyEffect(in BuildingUpgradeContext ctx)
        {
            if (ctx.Model == null || Items == null || Items.Length == 0)
            {
                return;
            }

            foreach (var item in Items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Id))
                {
                    continue;
                }

                ctx.Model.UnlockSellItem(item.Id);
            }
        }
    }
}


