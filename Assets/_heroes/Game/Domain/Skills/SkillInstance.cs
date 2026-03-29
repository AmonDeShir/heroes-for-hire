using Heroes.Content.Abstractions;
using Heroes.Game.Abstractions.Skills;

namespace Heroes.Game.Domain.Skills
{
    public class SkillInstance : ISkillInstance
    {
        public ISkillDefinition Definition { get; }
        public float CooldownRemaining { get; private set; }
        public bool IsReady => CooldownRemaining <= 0f;

        public SkillInstance(ISkillDefinition definition)
        {
            Definition = definition;
        }

        public void TriggerCooldown()
        {
            CooldownRemaining = Definition != null ? Definition.CooldownSeconds : 0f;
        }

        public void Tick(float deltaTime)
        {
            if (CooldownRemaining <= 0f)
            {
                return;
            }

            CooldownRemaining -= deltaTime;
            if (CooldownRemaining < 0f)
            {
                CooldownRemaining = 0f;
            }
        }
    }
}
