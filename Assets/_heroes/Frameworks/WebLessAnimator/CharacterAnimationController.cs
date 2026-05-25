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

        private int _resolvedAttackClip;

        public void SetSpeed(float speed) => _animator.SetFloat(SpeedHash, speed);

        public void PlayAttack()
        {
            if (_animator == null)
            {
                return;
            }

            if (_resolvedAttackClip == 0)
            {
                _resolvedAttackClip = ResolveFirstStateHash(
                    "Attack",
                    "attack",
                    "MeleeAttack",
                    "Melee",
                    "Atk",
                    "Hit");
            }

            if (_resolvedAttackClip == 0)
            {
                return;
            }

            PlayAnimationUsingTimer(_resolvedAttackClip);
        }
        public void PlayBuy() => PlayAnimationUsingTimer(BuyClip);
        public void PlayDeath() => PlayAnimation(DeathClip);

        protected void PlayAnimationUsingTimer(int clipHash)
        {
            PlayAnimationUsingTimer(clipHash, LocomotionClip);
        }

        private int ResolveFirstStateHash(params string[] names)
        {
            if (_animator == null || names == null)
            {
                return 0;
            }

            for (var i = 0; i < names.Length; i++)
            {
                var n = names[i];
                if (string.IsNullOrWhiteSpace(n))
                {
                    continue;
                }

                var h = Animator.StringToHash(n);
                if (_animator.HasState(0, h))
                {
                    return h;
                }
            }

            return 0;
        }
    }
}

