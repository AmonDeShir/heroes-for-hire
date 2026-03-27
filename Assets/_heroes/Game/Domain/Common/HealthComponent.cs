using Heroes.Game.Abstractions.Common;
using UnityEngine;

namespace Heroes.Game.Domain.Common
{
    public class HealthComponent : IHealthComponent
    {
        public float Current { get; private set; }
        public float Max { get; private set; }
        public float BaseRegeneration { get; private set; }

        public bool IsDead => Current <= 0f;
        public float Normalized => Max <= 0f ? 0f : Current / Max;

        public HealthComponent(float max, float spawnHealth, float baseRegeneration)
        {
            Max = max;
            BaseRegeneration = baseRegeneration;
            Current = Mathf.Clamp(spawnHealth, 0f, max);
        }

        public bool TryTakeDamage(float damage, out float appliedDamage)
        {
            appliedDamage = 0f;

            if (IsDead || damage <= 0f)
            {
                return false;
            }

            var before = Current;
            Current = Mathf.Clamp(Current - damage, 0f, Max);
            appliedDamage = before - Current;

            return appliedDamage > 0f;
        }

        public bool TryHeal(float amount, out float appliedHeal)
        {
            appliedHeal = 0f;

            if (IsDead || amount <= 0f)
            {
                return false;
            }

            var before = Current;
            Current = Mathf.Clamp(Current + amount, 0f, Max);
            appliedHeal = Current - before;

            return appliedHeal > 0f;
        }

        public bool TryRegenerate(float amountPerSecond, float deltaTime, out float appliedHeal)
        {
            return TryHeal(amountPerSecond * deltaTime, out appliedHeal);
        }

        public void SetCurrent(float value)
        {
            Current = Mathf.Clamp(value, 0f, Max);
        }
    }
}
