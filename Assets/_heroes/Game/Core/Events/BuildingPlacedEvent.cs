using EventBus;
using UnityEngine;

namespace Heroes.Game.Core.Events
{
    public struct BuildingPlacedEvent : IEvent
    {
        public string InstanceId;
        public string DefinitionId;
        public Vector3 Position;
    }
}


