using System.Collections.Generic;
using Registry;
using UnityEngine;

namespace Heroes.Game.Buildings
{
    
    public sealed class BuildingIncomeService : MonoBehaviour
    {
        private KingdomService kingdom;
        private readonly Dictionary<string, float> _nextTickAt = new();

        private void Awake()
        {
            
            
        }

        public void Initialize(KingdomService kingdomService)
        {
            kingdom = kingdomService;
        }

        private void Update()
        {
            var now = Time.time;

            foreach (var b in Registry<BuildingFacade>.All())
            {
                if (b == null || !b.IsAlive || b.Model == null || b.Definition == null)
                {
                    continue;
                }

                if (!b.Model.IsCompleted)
                {
                    continue;
                }

                var perTick = b.Definition.GoldIncomePerTick;
                var interval = b.Definition.GoldIncomeIntervalSeconds;
                if (perTick <= 0 || interval <= 0f)
                {
                    continue;
                }

                var multiplier = b.Model.GoldIncomeMultiplier;
                if (multiplier <= 0f || float.IsNaN(multiplier) || float.IsInfinity(multiplier))
                {
                    multiplier = 1f;
                }

                var perTickScaled = Mathf.RoundToInt(perTick * multiplier);
                if (perTickScaled <= 0)
                {
                    continue;
                }

                _nextTickAt.TryGetValue(b.Id, out var next);
                if (next <= 0f)
                {
                    next = now + interval;
                    _nextTickAt[b.Id] = next;
                    continue;
                }

                if (now < next)
                {
                    continue;
                }

                
                if (kingdom != null)
                {
                    kingdom.AddGold(perTickScaled);
                }
                _nextTickAt[b.Id] = now + interval;
            }
        }
    }
}


