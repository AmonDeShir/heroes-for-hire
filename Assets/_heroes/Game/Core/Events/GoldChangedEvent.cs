using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct GoldChangedEvent : IEvent
    {
        public int OldValue;
        public int NewValue;
    }
}
