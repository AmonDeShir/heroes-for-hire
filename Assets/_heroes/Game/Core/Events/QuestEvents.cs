using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct QuestCreatedEvent : IValueChangedEvent<string>
    {
        public string Value { get; set; }
    }

    public struct QuestUpdatedEvent : IValueChangedEvent<string>
    {
        public string Value { get; set; }
    }

    public struct QuestCompletedEvent : IValueChangedEvent<string>
    {
        public string Value { get; set; }
    }

    public struct QuestAcceptedEvent : IEvent
    {
        public string QuestId;
        public string HeroId;
    }

    public struct MonsterKilledEvent : IEvent
    {
        public string InstanceId;
    }
}
