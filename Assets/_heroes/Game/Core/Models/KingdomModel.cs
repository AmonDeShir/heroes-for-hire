namespace Heroes.Game.Core
{
    public sealed class KingdomModel
    {
        public int Gold { get; private set; }

        public KingdomModel(int startGold)
        {
            Gold = startGold < 0 ? 0 : startGold;
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
    }
}
