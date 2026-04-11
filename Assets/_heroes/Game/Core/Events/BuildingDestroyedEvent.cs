using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct BuildingDestroyedEvent : IEvent
    {
        public string InstanceId;
        public string DefinitionId;
    }
}
