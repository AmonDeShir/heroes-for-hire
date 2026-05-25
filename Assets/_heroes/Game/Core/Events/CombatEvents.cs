using EventBus;
using UnityEngine;

namespace Heroes.Game.Core.Events
{
    public struct CombatStartedEvent : IEvent
    {
        public string SourceId;
        public Vector3 Position;
        public float Radius;
    }

    public struct CombatEndedEvent : IEvent
    {
        public string SourceId;
        public Vector3 Position;
        public float Radius;
    }
}
