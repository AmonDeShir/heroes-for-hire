using EventBus;
using Heroes.Game.Abstractions;

namespace Heroes.Game.Core.Events
{
    public struct ObjectSelectedEvent : IEvent
    {
        public ISelectable value;
    }
}
