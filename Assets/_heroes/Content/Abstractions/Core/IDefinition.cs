namespace Heroes.Content.Abstractions
{
    public interface IDefinition
    {
        string Id { get; }
        string DisplayName { get; }
        string Description { get; }
    }
}
