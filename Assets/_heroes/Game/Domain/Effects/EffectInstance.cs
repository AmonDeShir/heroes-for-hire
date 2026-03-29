using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions.Effects;

namespace Heroes.Game.Domain.Effects
{
    public class EffectInstance : IEffectInstance
    {
        public IEffectDefinition Definition { get; }
        public float RemainingDuration { get; private set; }
        public bool IsExpired => RemainingDuration <= 0f;

        public EffectInstance(IEffectDefinition definition)
        {
            Definition = definition;
            RemainingDuration = definition != null ? definition.DurationSeconds : 0f;
        }

        public void Tick(float deltaTime)
        {
            if (RemainingDuration <= 0f)
            {
                return;
            }

            RemainingDuration -= deltaTime;
            if (RemainingDuration < 0f)
            {
                RemainingDuration = 0f;
            }
        }
    }
}
