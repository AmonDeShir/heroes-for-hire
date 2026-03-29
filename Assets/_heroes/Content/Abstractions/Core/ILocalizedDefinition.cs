namespace Heroes.Content.Abstractions
{
    public interface ILocalizedDefinition
    {
        string DisplayNameKey { get; }
        string DescriptionKey { get; }
    }
}
