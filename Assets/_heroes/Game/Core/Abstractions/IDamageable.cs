namespace Heroes.Game.Abstractions
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void ApplyDamage(float amount);
    }
}
