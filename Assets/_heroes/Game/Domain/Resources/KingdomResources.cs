using Heroes.Game.Abstractions;

namespace Heroes.Game.Domain.Resources
{
    public class KingdomResources : IKingdomResources
    {
        public int Gold { get; private set; }

        public KingdomResources(int startingGold)
        {
            Gold = startingGold;
        }

        public bool HasEnoughGold(int amount) => Gold >= amount;

        public bool TrySpendGold(int amount)
        {
            if (Gold < amount)
            {
                return false;
            }

            Gold -= amount;
            
            return true;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
        }
    }
}
