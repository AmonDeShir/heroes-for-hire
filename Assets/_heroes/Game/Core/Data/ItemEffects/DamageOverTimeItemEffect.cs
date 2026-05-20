using Heroes.Game.Runtime;
using UnityEngine;

namespace Heroes.Content.Heroes.ItemEffects
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Effects/Damage Over Time")]
    public sealed class DamageOverTimeItemEffect : ItemEffect
    {
        public float DamagePerSecond = 1f;
        public float DurationSeconds = 10f;

        public override void Apply(in ItemEffectContext ctx)
        {
            var target = ctx.Target != null ? ctx.Target : ctx.User;
            if (target == null)
            {
                return;
            }

            if (!target.TryGetComponent<TimedEffectRunner>(out var runner) || runner == null)
            {
                return;
            }

            runner.AddDamageOverTime(DamagePerSecond, DurationSeconds);
        }
    }
}


