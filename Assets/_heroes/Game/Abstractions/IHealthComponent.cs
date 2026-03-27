namespace Heroes.Game.Abstractions.Common
{
    public interface IHealthComponent
    {
        float Current { get; }
        float Max { get; }
        float BaseRegeneration { get; }
        bool IsDead { get; }
        float Normalized { get; }

        bool TryTakeDamage(float damage, out float appliedDamage);
        bool TryHeal(float amount, out float appliedHeal);
        bool TryRegenerate(float amountPerSecond, float deltaTime, out float appliedHeal);
        void SetCurrent(float value);
    }
}
