using EventBus;
using Heroes.Content.Heroes;
using Heroes.Game.Core.Events;
using UnityEngine;

namespace Heroes.Game.Heroes
{
    public sealed class HeroModel
    {
        public string InstanceId { get; }
        public string DefinitionId { get; }
        public string HomeBuildingInstanceId { get; }
        public Core.Health.HealthModel Health { get; }

        public int Gold { get; private set; }
        public float GearLevel { get; private set; }
        public float DangerLevel { get; private set; }
        public bool IsAlive => Health.Current > 0f;
        public bool IsInHome { get; private set; }
        public float HomeRadius { get; }
        public float DangerSenseRadius { get; }

        public HeroModel(string instanceId, HeroDefinition definition, string homeBuildingInstanceId)
        {
            InstanceId = instanceId;
            DefinitionId = definition != null ? definition.Id : string.Empty;
            HomeBuildingInstanceId = homeBuildingInstanceId;
            HomeRadius = definition != null ? Mathf.Max(0.1f, definition.HomeRadius) : 2f;
            DangerSenseRadius = definition != null ? Mathf.Max(1f, definition.DangerSenseRadius) : 12f;
            Health = new Core.Health.HealthModel(instanceId, definition != null ? definition.MaxHp : 0f, definition != null ? definition.StartHp : 0f);
            Gold = definition != null ? definition.StartGold : 0;
            GearLevel = definition != null ? definition.BaseGearLevel : 0f;
            DangerLevel = 0f;
        }

        public void SetGold(int value)
        {
            Gold = value < 0 ? 0 : value;
            EventBus<HeroGoldChangedEvent>.Invoke(new HeroGoldChangedEvent { Id = InstanceId, Value = Gold });
        }

        public void SetGearLevel(float value)
        {
            GearLevel = value < 0f ? 0f : value;
        }

        public void SetDangerLevel(float value)
        {
            DangerLevel = Mathf.Clamp01(value);
            EventBus<HeroDangerChangedEvent>.Invoke(new HeroDangerChangedEvent { Id = InstanceId, Value = DangerLevel });
        }

        public void SetInHome(bool value)
        {
            IsInHome = value;
            if (value)
            {
                SetDangerLevel(0f);
            }
        }
    }
}
