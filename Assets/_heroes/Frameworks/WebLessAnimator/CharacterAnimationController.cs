using UnityEngine;

namespace WebLess
{
    public abstract class CharacterAnimationController : WebLessAnimator
    {
        public int LocomotionClip { get; protected set; } = Animator.StringToHash("Locomotion");
        public int SpeedHash { get; protected set; } = Animator.StringToHash("Speed");
        public int AttackClip { get; protected set; } = Animator.StringToHash("Attack");
        public int BuyClip { get; protected set; } = Animator.StringToHash("Buy");
        public int DeathClip { get; protected set; } = Animator.StringToHash("Death");
        
        public void SetSpeed(float speed) => _animator.SetFloat(SpeedHash, speed);

        public void PlayAttack() => PlayAnimationUsingTimer(AttackClip);
        public void PlayBuy() => PlayAnimationUsingTimer(BuyClip);
        public void PlayDeath() => PlayAnimationAndFreeze(DeathClip);
        public float GetAttackDuration() => GetAnimationLength(AttackClip);
        public float GetDeathDuration() => GetAnimationLength(DeathClip);
        public void ResetToLocomotion()
        {
            ResumeAnimator();
            _animator.CrossFade(LocomotionClip, _crossfadeDuration);
        }

        protected void PlayAnimationUsingTimer(int clipHash)
        {
            PlayAnimationUsingTimer(clipHash, LocomotionClip);
        }
    }
}

