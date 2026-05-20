using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Content.Heroes.ItemEffects
{
    public enum ItemEffectTrigger
    {
        Equip = 0,
        Use = 1,
        Hit = 2,
    }

    public struct ItemEffectContext
    {
        public HeroFacade User;
        public HeroFacade Target;
        public ItemDefinition Item;
        public ItemEffectTrigger Trigger;
    }

    public abstract class ItemEffect : ScriptableObject
    {
        public abstract void Apply(in ItemEffectContext ctx);
    }
}


