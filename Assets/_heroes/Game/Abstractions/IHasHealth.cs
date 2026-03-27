using Heroes.Game.Abstractions.Common;

namespace Heroes.Game.Abstractions
{
    public interface IHasHealth
    {
        IHealthComponent Health { get; }
    }
}
