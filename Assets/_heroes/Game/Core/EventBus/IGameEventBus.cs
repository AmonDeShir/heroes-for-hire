using System;

namespace Heroes.Game.Core.Events.Bus
{
    public interface IGameEventBus
    {
        void Publish<T>(T gameEvent);
        void Subscribe<T>(Action<T> callback);
        void Unsubscribe<T>(Action<T> callback);
    }
}