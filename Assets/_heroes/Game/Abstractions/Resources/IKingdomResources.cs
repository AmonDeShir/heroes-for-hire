namespace Heroes.Game.Abstractions
{
    public interface IKingdomResources
    {
        int Gold { get; }
        bool HasEnoughGold(int amount);
        bool TrySpendGold(int amount);
        void AddGold(int amount);
    }
}
