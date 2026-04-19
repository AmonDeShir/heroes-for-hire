namespace Heroes.Game.Abstractions
{
    public interface IDamageable
    {
        float Health { get; }
        float MaxHealth { get; }
        
        bool IsAlive { get; }
        void ApplyDamage(float amount);
    }
}
