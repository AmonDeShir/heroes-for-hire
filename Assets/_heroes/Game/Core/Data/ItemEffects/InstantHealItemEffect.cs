using EventBus;
using Heroes.Game.Core.Events;
using UnityEngine;

namespace Heroes.Content.Heroes.ItemEffects
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Effects/Instant Heal")]
    public sealed class InstantHealItemEffect : ItemEffect
    {
        public float HealAmount = 10f;

        public override void Apply(in ItemEffectContext ctx)
        {
            if (ctx.User?.Model?.Health == null)
            {
                return;
            }

            if (HealAmount <= 0f)
            {
                return;
            }

            var before = ctx.User.Model.Health.Current;
            ctx.User.Model.Health.SetCurrent(before + HealAmount);

            if (!Mathf.Approximately(before, ctx.User.Model.Health.Current))
            {
                EventBus<HealthChangedEvent>.Invoke(new HealthChangedEvent
                {
                    Id = ctx.User.Model.InstanceId,
                    Value = ctx.User.Model.Health.Current,
                });
            }
        }
    }
}


