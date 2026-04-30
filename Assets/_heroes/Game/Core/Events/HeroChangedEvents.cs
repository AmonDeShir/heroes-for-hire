using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct HeroGoldChangedEvent : IValueChangedEvent<int>
    {
        public string Id { get; set; }
        public int Value { get; set; }
    }

    public struct HeroDangerChangedEvent : IValueChangedEvent<float>
    {
        public string Id { get; set; }
        public float Value { get; set; }
    }
}
