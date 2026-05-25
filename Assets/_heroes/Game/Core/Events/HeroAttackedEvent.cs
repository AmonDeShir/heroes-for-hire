using EventBus;
using UnityEngine;

namespace Heroes.Game.Core.Events
{
    public struct HeroAttackedEvent : IEvent
    {
        public string HeroInstanceId;
        public Vector3 Position;
        public float Damage;
    }
}
