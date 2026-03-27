using System;
using System.Collections.Generic;
using Heroes.Game.Core.Events.Bus;

namespace Heroes.Game.Core.Events
{
    public class GameEventBus : IGameEventBus
    {
        private readonly Dictionary<Type, Delegate> _subscriptions = new();

        public void Publish<T>(T gameEvent)
        {
            if (_subscriptions.TryGetValue(typeof(T), out var del))
            {
                (del as Action<T>)?.Invoke(gameEvent);
            }
        }

        public void Subscribe<T>(Action<T> callback)
        {
            if (_subscriptions.TryGetValue(typeof(T), out var del))
            {
                _subscriptions[typeof(T)] = (Action<T>)del + callback;
            }
            else
            {
                _subscriptions[typeof(T)] = callback;
            }
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            if (!_subscriptions.TryGetValue(typeof(T), out var del))
            {
                return;
            }

            var newDel = (Action<T>)del - callback;
                
            if (newDel == null)
            {
                _subscriptions.Remove(typeof(T));
            }
            else
            {
                _subscriptions[typeof(T)] = newDel;
            }
        }
    }
}