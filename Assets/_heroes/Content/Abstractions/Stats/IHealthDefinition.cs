namespace Heroes.Content.Abstractions
{
    public interface IHealthDefinition
    {
        float MaxHealth { get; }
        float SpawnHealth { get; }
        float BaseRegeneration { get; }
    }
}
