namespace Heroes.Content.Abstractions
{
    public interface IStatBlock
    {
        float Strength { get; }
        float Agility { get; }
        float Intelligence { get; }
        float Endurance { get; }
        float Luck { get; }
        float Wisdom { get; }
    }
}
