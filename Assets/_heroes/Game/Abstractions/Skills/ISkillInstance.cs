using Heroes.Content.Abstractions;

namespace Heroes.Game.Abstractions.Skills
{
    public interface ISkillInstance
    {
        ISkillDefinition Definition { get; }
        float CooldownRemaining { get; }
        bool IsReady { get; }
    }
}
