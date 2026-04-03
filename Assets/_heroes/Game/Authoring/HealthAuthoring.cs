using Unity.Entities;
using UnityEngine;

namespace Heroes.Game.Authoring
{
    public class HealthAuthoring : MonoBehaviour
    {
        public float max;
        public float start;
        public float regen;

        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                AddComponent(entity, new Components.Health
                {
                    max = authoring.max,
                    value = authoring.start,
                    regen = authoring.regen
                });
            }
        }
    }
}