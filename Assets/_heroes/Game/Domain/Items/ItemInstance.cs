using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions.Items;

namespace Heroes.Game.Domain.Items
{
    public class ItemInstance : IItemInstance
    {
        public IItemDefinition Definition { get; }
        public bool IsEquipped { get; private set; }

        public ItemInstance(IItemDefinition definition)
        {
            Definition = definition;
        }

        public void SetEquipped(bool value)
        {
            IsEquipped = value;
        }
    }
}
