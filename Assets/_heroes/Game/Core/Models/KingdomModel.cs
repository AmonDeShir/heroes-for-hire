using System;
using System.Collections.Generic;

namespace Heroes.Game.Core
{
    public sealed class KingdomModel
    {
        public int Gold { get; private set; }
        public int Population { get; private set; }

        public int CastleLevel { get; private set; }

        private readonly Dictionary<string, int> _populationContributions = new();

        public KingdomModel(int startGold)
        {
            Gold = startGold < 0 ? 0 : startGold;
            Population = 1;
            CastleLevel = 1;
        }

        public bool TrySetCastleLevel(int level)
        {
            if (level < 1)
            {
                level = 1;
            }

            if (level == CastleLevel)
            {
                return false;
            }

            CastleLevel = level;
            return true;
        }

        public bool CanAfford(int amount)
        {
            return amount >= 0 && Gold >= amount;
        }

        public bool TrySpendGold(int amount)
        {
            if (!CanAfford(amount))
            {
                return false;
            }

            Gold -= amount;
            return true;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Gold += amount;
        }

        public void AddPeople(int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            
            Population += amount;
        }

        public void RemovePeople(int amount)
        {
            Population = Math.Max(Population - amount, 0);
        }

        public bool SetPopulationContribution(string key, int amount)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            if (amount < 0)
            {
                amount = 0;
            }

            _populationContributions.TryGetValue(key, out var prev);
            if (prev == amount)
            {
                return false;
            }

            _populationContributions[key] = amount;
            Population += (amount - prev);
            if (Population < 0)
            {
                Population = 0;
            }

            return true;
        }

        public bool RemovePopulationContribution(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || !_populationContributions.TryGetValue(key, out var prev))
            {
                return false;
            }

            _populationContributions.Remove(key);
            Population = Math.Max(Population - prev, 0);
            return true;
        }
    }
}


