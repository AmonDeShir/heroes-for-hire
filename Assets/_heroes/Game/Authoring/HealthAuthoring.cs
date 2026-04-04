using Heroes.Game.Components;
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
                
                AddComponent(entity, new Health { Value = authoring.start });
                AddComponent(entity, new HealthRegeneration { Value = authoring.regen });
                AddComponent(entity, new MaxHealth { Value = authoring.max });
                
                AddBuffer<DamageBuffer>(entity);
            }
        }
    }
}