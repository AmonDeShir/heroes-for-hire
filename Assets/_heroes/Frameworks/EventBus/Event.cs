using System;

namespace EventBus
{
    public interface IEvent { }

    public interface IValueChangedEvent<T> : IEvent
    {
        T Value { get; set; }
    }
    
    public interface IValueChangedWithIdEvent<T> : IEvent
    {
        string Id { get; set; }
        T Value { get; set; }
    }
    
    public struct Event : IEvent
    {
        
    }
}

