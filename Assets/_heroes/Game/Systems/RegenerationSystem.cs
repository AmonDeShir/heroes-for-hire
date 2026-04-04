using Heroes.Game.Components;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Heroes.Game.Systems
{
    public partial struct RegenerationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            new RegenerationJob
            {
                deltaTime = SystemAPI.Time.DeltaTime
            }.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct RegenerationJob: IJobEntity
    {
        public float deltaTime;
        
        [BurstCompile]
        public void Execute(ref DynamicBuffer<DamageBuffer> buffer, in Health health, in HealthRegeneration regeneration, in MaxHealth maxHealth)
        {
            if (Mathf.Approximately(health.Value, maxHealth.Value))
            {
                return;
            }
            
            var regenerated = regeneration.Value * deltaTime;
            
            buffer.Add(new DamageBuffer { Value = -regenerated });
        }
    }
}