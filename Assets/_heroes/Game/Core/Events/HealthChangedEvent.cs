using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct HealthChangedEvent : IEvent
    {
        public string Id;
        public float OldValue;
        public float NewValue;
    }
}
