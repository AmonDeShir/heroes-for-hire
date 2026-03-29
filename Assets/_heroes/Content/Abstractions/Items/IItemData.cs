using Heroes.Content.Abstractions;

namespace Heroes.Content.Abstractions
{
    public interface IItemData : IDescription
    {
        int Cost { get; }
        IEntityStats Stats { get; }
        int AttackRange { get; }
    }
}
