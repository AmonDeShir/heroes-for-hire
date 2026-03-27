using UnityEngine;

namespace WebLess
{
    public abstract class WebLessAnimator : MonoBehaviour
    {
        protected const float _crossfadeDuration = 0.1f;

        protected Animator _animator;
        protected Timer _timer;

        protected float _animationLength;

        protected void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
        }
        
        protected void Update()
        {
            _timer?.Tick(Time.deltaTime);
        }
        
        protected void PlayAnimationUsingTimer(int clipHash, int exitHash)
        {
            var length = GetAnimationLength(clipHash);
            if (length <= 0f)
            {
                length = 0.1f;
            }

            _timer = new Timer(length, oneShoot: true);
            _timer.OnStart += () => _animator.CrossFade(clipHash, _crossfadeDuration);
            _timer.OnTimeOut += () => _animator.CrossFade(exitHash, _crossfadeDuration);
            _timer.Start();
        }
        
        protected void PlayAnimation(int clipHash)
        {
            _animator.CrossFade(clipHash, _crossfadeDuration);
        }

        public float GetAnimationLength(int hash)
        {
            if (_animationLength > 0)
            {
                return _animationLength;
            }

            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (Animator.StringToHash(clip.name) == hash)
                {
                    _animationLength = clip.length;
                    return clip.length;
                }
            }

            Debug.LogWarning($"Animation clip not found for hash {hash} on {name}.");

            return -1f;
        }
    }
}