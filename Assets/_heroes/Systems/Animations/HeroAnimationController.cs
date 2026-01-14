using UnityEngine;

namespace _heroes.Systems.Animations
{
    public class HeroAnimationController : AnimationController
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