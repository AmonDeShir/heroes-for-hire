using UnityEngine;

namespace Heroes.Animations
{
    public abstract class CharacterAnimationController : AnimationController, IAttackable, IMoveable
    {
        [HideInInspector] 
        public int LocomotionClip = Animator.StringToHash("Locomotion");
    
        [HideInInspector] 
        public int SpeedHash = Animator.StringToHash("Speed");
    
        [HideInInspector] 
        public int AttackClip = Animator.StringToHash("Attack");
    
        protected override void LoadAnimationHashes() 
        {
            SetLocomotionClip();
            SetAttackClip();
            SetSpeedHash();
        }

        public void SetSpeed(float speed) => _animator.SetFloat(SpeedHash, speed);
        public void Attack() => PlayAnimationUsingTimer(AttackClip, LocomotionClip);
        
        protected abstract void SetLocomotionClip();
        protected abstract void SetAttackClip();
        protected abstract void SetSpeedHash();
    }

}