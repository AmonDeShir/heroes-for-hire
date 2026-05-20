using Heroes.Game.Runtime;
using UnityEngine;

namespace Heroes.Content.Heroes.ItemEffects
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Effects/Timed HP Regeneration")]
    public sealed class TimedHpRegenerationItemEffect : ItemEffect
    {
        public float AddHpPerSecond = 1f;
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

            runner.AddTimedHpRegeneration(AddHpPerSecond, DurationSeconds);
        }
    }
}


