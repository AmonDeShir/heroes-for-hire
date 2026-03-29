namespace Heroes.Content.Abstractions
{
    public interface ISkillData
    {
        bool ManaCost { get; }
        bool StaminaCost { get; }
        float Cooldown { get; }
    }
}
