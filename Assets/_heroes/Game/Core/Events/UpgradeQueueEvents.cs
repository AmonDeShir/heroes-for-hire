using System.Collections.Generic;
using EventBus;

namespace Heroes.Game.Core.Events
{
    public struct UpgradeQueueProgressChangedEvent : IValueChangedWithIdEvent<float>
    {
        public string Id { get; set; }
        public float Value { get; set; }
    }
    
    public struct UpgradeQueueChangedEvent : IValueChangedWithIdEvent<IReadOnlyList<string>>
    {
        public string Id { get; set; }
        public IReadOnlyList<string> Value { get; set; }
    }
    
    public struct UpgradeQueueActiveChangedEvent : IValueChangedWithIdEvent<string>
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
    
    public struct UpgradeQueueAvailableListChangedEvent : IValueChangedWithIdEvent<IReadOnlyList<string>>
    {
        public string Id { get; set; }
        public IReadOnlyList<string> Value { get; set; }
    }
    
    public struct UpgradeQueueUpgradeCompletedEvent : IValueChangedWithIdEvent<string>
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}

