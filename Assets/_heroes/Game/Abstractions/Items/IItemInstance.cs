using Heroes.Content.Abstractions;

namespace Heroes.Game.Abstractions.Items
{
    public interface IItemInstance
    {
        IItemDefinition Definition { get; }
        bool IsEquipped { get; }
    }
}
