using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions.Common;
using Heroes.Game.Core;

namespace Heroes.Game.Abstractions.Entities
{
    public interface IEntity
    {
        EntityId Id { get; }
        IEntityDefinition Definition { get; }
        IHealthComponent Health { get; }
    }
}
