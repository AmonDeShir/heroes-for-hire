public interface IDamageable
{
    void TakeDamage(float amount);
    float CurrentHp { get; }
    bool IsDead { get; }
}