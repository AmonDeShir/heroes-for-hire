namespace Heroes.Game.Core.Events.Health
{
    public struct EntityDamagedEvent
    {
        public EntityId EntityId { get; }
        public float Damage { get; }
        public float CurrentHealth { get; }

        public EntityDamagedEvent(EntityId entityId, float damage, float currentHealth)
        {
            EntityId = entityId;
            Damage = damage;
            CurrentHealth = currentHealth;
        }
    }
}