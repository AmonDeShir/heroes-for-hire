using UnityEngine;

namespace WebLess
{
    public abstract class CharacterAnimationController : WebLessAnimator
    {
        public int LocomotionClip { get; protected set; } = Animator.StringToHash("Locomotion");
        public int SpeedHash { get; protected set; } = Animator.StringToHash("Speed");
        public int AttackClip { get; protected set; } = Animator.StringToHash("Attack");
        public int DeathClip { get; protected set; } = Animator.StringToHash("Death");

        public void SetSpeed(float speed) => _animator.SetFloat(SpeedHash, speed);
        public void PlayAttack() => PlayAnimationUsingTimer(AttackClip);
        public void PlayDeath() => PlayAnimation(DeathClip);

        protected void PlayAnimationUsingTimer(int clipHash)
        {
            PlayAnimationUsingTimer(clipHash, LocomotionClip);
        }
    }
}