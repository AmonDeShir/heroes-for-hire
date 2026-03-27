namespace Heroes.Game.Abstractions.Heroes
{
    public interface IHero
    {
        HeroState State { get; }
        float NormalizedSpeed { get; }
    }
}
