namespace Heroes.Presentation.World.Hero
{
    public interface IHeroAnimationDriver
    {
        void SetSpeed(float normalizedSpeed);
        void PlayAttack();
        void PlayCast();
        void PlayDeath();
    }
}