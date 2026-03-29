using Heroes.Content.Abstractions;

namespace Heroes.Game.Abstractions.Effects
{
    public interface IEffectInstance
    {
        IEffectDefinition Definition { get; }
        float RemainingDuration { get; }
        bool IsExpired { get; }
    }
}
