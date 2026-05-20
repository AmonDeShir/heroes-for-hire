using Heroes.Game.Runtime;
using UnityEngine;

namespace Heroes.Content.Heroes.ItemEffects
{
    [CreateAssetMenu(menuName = "Heroes/Items/Item Effects/Timed Stat Buff")]
    public sealed class TimedStatBuffItemEffect : ItemEffect
    {
        public float AddAttack;
        public float AddDefence;
        public float AddSpeed;
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

            runner.AddTimedStatBuff(AddAttack, AddDefence, AddSpeed, DurationSeconds);
        }
    }
}


