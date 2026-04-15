using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct PopulationChangedEvent : IEvent
    {
        public int OldValue;
        public int NewValue;
    }
}
