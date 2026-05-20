using System.Collections.Generic;

namespace EventBus
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> _bindings = new();
        
        public static void Register(IEventBinding<T> binding)
        {
            _bindings.Add(binding);
        }

        public static void Unregister(IEventBinding<T> binding)
        {
            _bindings.Remove(binding);
        }
        
        public static void Invoke(T @event)
        {
            foreach (var binding in _bindings)
            {
                binding.OnEvent.Invoke(@event);
                binding.OnEventNoArgs.Invoke();
            }
        }

        private static void Clear()
        {
            _bindings.Clear();
        }
    }
}

