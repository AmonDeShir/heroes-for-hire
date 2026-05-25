using System.Linq;
using Heroes.Content.Heroes;
using Heroes.Content.Heroes.ItemEffects;
using Heroes.Game.Buildings;
using Heroes.Game.Heroes;

namespace Heroes.Game.Buildings
{
    public sealed class ShopService
    {
        public bool TryBuyItem(HeroFacade hero, BuildingFacade building, ItemDefinition item)
        {
            if (hero?.Model == null || building?.Model == null || building.Definition == null || item == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(item.Id))
            {
                return false;
            }

            var sells = building.Definition.SellItems;
            if (sells == null || sells.Length == 0 || !sells.Any(x => x != null && x.Id == item.Id))
            {
                return false;
            }

            if (!building.Model.IsSellItemUnlocked(item.Id))
            {
                return false;
            }

            var cost = item.GoldCost;
            if (cost <= 0 || hero.Model.Gold < cost)
            {
                return false;
            }

            if (!hero.Model.TryAddAndAutoEquip(item))
            {
                return false;
            }

            hero.ApplyItemEffects(item, ItemEffectTrigger.Equip);

            hero.Model.SetGold(hero.Model.Gold - cost);
            
            
            
            return true;
        }
    }
}


