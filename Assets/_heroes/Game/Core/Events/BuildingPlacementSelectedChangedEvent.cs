using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct BuildingPlacementSelectedChangedEvent : IValueChangedEvent<string>
    {
        public string Value { get; set; }
    }
}


