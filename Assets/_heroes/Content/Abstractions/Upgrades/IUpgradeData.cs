using Heroes.Content.Abstractions;
namespace Heroes.Content.Abstractions
{
    public interface IUpgradeData : IDescription
    {
        int Cost { get; }
    }
}
