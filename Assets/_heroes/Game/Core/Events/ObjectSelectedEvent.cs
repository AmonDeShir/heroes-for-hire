using EventBus;
using Heroes.Game.Abstractions;

namespace Heroes.Game.Core.Events
{
    public struct ObjectSelectedEvent : IValueChangedEvent<ISelectable>
    {
        public ISelectable Value { get; set; }
    }
}


