using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct PopulationChangedEvent : IValueChangedEvent<int>
    {
        public int Value { get; set; }
    }
}
