using System;

namespace EventBus
{
    public interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = delegate { };
        private Action _onEventNoArgs = delegate { };
        
        Action<T> IEventBinding<T>.OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        public EventBinding(Action<T> onEvent)
        {
            _onEvent = onEvent;
        }

        public EventBinding(Action onEvent)
        {
            _onEventNoArgs = onEvent;
        }

        public void Subscribe(Action<T> onEvent)
        {
            _onEvent += onEvent;
        }

        public void Subscribe(Action onEvent)
        {
            _onEventNoArgs += onEvent;
        }

        public void Unsubscribe(Action<T> onEvent)
        {
            _onEvent -= onEvent;
        }

        public void Unsubscribe(Action onEvent)
        {
            _onEventNoArgs -= onEvent;
        }
    }
}

