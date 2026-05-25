using EventBus;
using UnityEngine;

namespace Heroes.Game.Core.Events
{
    public struct BuildingAttackedEvent : IEvent
    {
        public string InstanceId;
        public string DefinitionId;
        public Vector3 Position;
        public float Damage;
    }
}
