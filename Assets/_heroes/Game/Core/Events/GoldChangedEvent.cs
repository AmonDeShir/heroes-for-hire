using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct GoldChangedEvent : IValueChangedEvent<int>
    {
        public int Value { get; set; }
    }
}


