using Heroes.Content.Heroes;
using Heroes.Content.Heroes.ItemEffects;
using Heroes.Game.Heroes;

namespace Heroes.Game.Combat
{
    public sealed class CombatService
    {
        private readonly ItemCatalog _items;

        public CombatService(ItemCatalog items)
        {
            _items = items;
        }

        public bool TryUseHealingConsumable(HeroFacade hero)
        {
            if (hero?.Model == null || _items == null)
            {
                return false;
            }

            var list = hero.Model.EquippedConsumables;
            if (list == null || list.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < list.Count; i++)
            {
                var id = list[i];
                var def = _items.GetById(id);
                if (def == null || !def.IsSingleUse)
                {
                    continue;
                }

                if (!IsHealing(def))
                {
                    continue;
                }

                hero.ApplyItemEffects(def, ItemEffectTrigger.Use);
                hero.Model.RemoveConsumable(id);
                return true;
            }

            return false;
        }

        public bool TryUseSpeedConsumable(HeroFacade hero)
        {
            if (hero?.Model == null || _items == null)
            {
                return false;
            }

            var list = hero.Model.EquippedConsumables;
            if (list == null || list.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < list.Count; i++)
            {
                var id = list[i];
                var def = _items.GetById(id);
                if (def == null || !def.IsSingleUse)
                {
                    continue;
                }

                if (!IsSpeed(def))
                {
                    continue;
                }

                hero.ApplyItemEffects(def, ItemEffectTrigger.Use);
                hero.Model.RemoveConsumable(id);
                return true;
            }

            return false;
        }

        private static bool IsHealing(ItemDefinition def)
        {
            if (def == null)
            {
                return false;
            }

            var effects = def.Effects;
            if (effects == null)
            {
                return false;
            }

            for (var i = 0; i < effects.Length; i++)
            {
                var e = effects[i];
                if (e.Effect is InstantHealItemEffect)
                {
                    return true;
                }

                if (e.Effect is TimedHpRegenerationItemEffect)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSpeed(ItemDefinition def)
        {
            if (def == null)
            {
                return false;
            }

            if (def.Speed > 0f)
            {
                return true;
            }

            var effects = def.Effects;
            if (effects == null)
            {
                return false;
            }

            for (var i = 0; i < effects.Length; i++)
            {
                var e = effects[i];
                if (e.Effect is TimedStatBuffItemEffect buff && buff.AddSpeed > 0f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
