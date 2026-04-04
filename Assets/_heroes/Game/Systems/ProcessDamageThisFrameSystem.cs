using System;
using Heroes.Game.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Heroes.Game.Systems
{
    public partial struct ProcessDamageThisFrameSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            new ProcessDamageThisFrameJob().ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct ProcessDamageThisFrameJob: IJobEntity
    {
        [BurstCompile]
        public void Execute(ref Health health, ref DynamicBuffer<DamageBuffer> buffer, in MaxHealth maxHealth)
        {
            if (buffer.IsEmpty)
            {
                return;
            }

            foreach (var damage in buffer)
            {
                health.Value = math.clamp(health.Value - damage.Value, 0, maxHealth.Value);
            }   
            
            buffer.Clear();
        }
    }
}