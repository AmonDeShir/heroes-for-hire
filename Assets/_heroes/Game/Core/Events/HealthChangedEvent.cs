using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct HealthChangedEvent : IValueChangedEvent<float>
    {
        public string Id { get; set; }
        public float Value { get; set; }
    }

    public struct MaxHealthChangedEvent : IValueChangedEvent<float>
    {
        public string Id { get; set; }
        public float Value { get; set; }
    }
}
