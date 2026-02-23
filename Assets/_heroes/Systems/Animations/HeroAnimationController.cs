using UnityEngine;

namespace Heroes.Animations
{
    public class HeroAnimationController : CharacterAnimationController
    {
        protected override void SetLocomotionClip() {
            LocomotionClip = Animator.StringToHash("Run");
        }
    
        protected override void SetAttackClip() {
            AttackClip = Animator.StringToHash("Punch");
        }
    
        protected override void SetSpeedHash() {
            SpeedHash = Animator.StringToHash("Speed");
        }
    }
}