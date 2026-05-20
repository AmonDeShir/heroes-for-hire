using EventBus;
using Heroes.Game.Core.Events;
using Heroes.Game.Heroes;
using UnityEngine;

namespace Heroes.Content.Heroes.ItemEffects
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Effects/Modify Max HP")]
    public sealed class ModifyMaxHpItemEffect : ItemEffect
    {
        public float AddMaxHp = 0f;
        public bool HealToFull;

        public override void Apply(in ItemEffectContext ctx)
        {
            if (ctx.User?.Model?.Health == null)
            {
                return;
            }

            if (Mathf.Approximately(AddMaxHp, 0f))
            {
                return;
            }

            var nextMax = ctx.User.Model.Health.Max + AddMaxHp;
            ctx.User.Model.Health.SetMax(nextMax);
            
            if (HealToFull)
            {
                ctx.User.Model.Health.SetCurrent(ctx.User.Model.Health.Max);
            }

            EventBus<MaxHealthChangedEvent>.Invoke(new MaxHealthChangedEvent
            {
                Id = ctx.User.Model.InstanceId,
                Value = ctx.User.Model.Health.Max,
            });
            
            EventBus<HealthChangedEvent>.Invoke(new HealthChangedEvent
            {
                Id = ctx.User.Model.InstanceId,
                Value = ctx.User.Model.Health.Current,
            });
        }
    }
}


