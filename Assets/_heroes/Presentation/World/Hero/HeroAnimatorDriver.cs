using WebLess;

namespace Heroes.Presentation.World.Hero
{
    using UnityEngine;

    public class HeroAnimatorDriver : CharacterAnimationController, IHeroAnimationDriver
    {
        private static readonly int CastClip = Animator.StringToHash("Cast");
        
        public void PlayCast() => PlayAnimationUsingTimer(CastClip);
    }
}