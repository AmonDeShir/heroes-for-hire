using System;

namespace Heroes.Game.Core
{
    public sealed class KingdomModel
    {
        public int Gold { get; private set; }
        public int Population { get; private set; }

        public KingdomModel(int startGold)
        {
            Gold = startGold < 0 ? 0 : startGold;
            Population = 1;
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
    }
}
