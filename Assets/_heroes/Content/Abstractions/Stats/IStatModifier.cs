namespace Heroes.Content.Abstractions
{
    public interface IStatModifier
    {
        StatType Stat { get; }
        float Value { get; }
        float DurationSeconds { get; }
    }
}
