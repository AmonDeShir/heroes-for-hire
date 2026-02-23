using UnityEngine;

namespace Heroes.Animations
{
    public abstract class AnimationController : MonoBehaviour
    {
        protected const float _crossfadeDuration = 0.1f;

        protected Animator _animator;
        protected Timer _timer;

        protected float _animationLength;
        
        protected void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            LoadAnimationHashes();
        }

        protected abstract void LoadAnimationHashes(); 
        
        protected void Update()
        {
            _timer?.Tick(Time.deltaTime);
        }

        protected void PlayAnimationUsingTimer(int clipHash, int exitHash)
        {
            _timer = new Timer(GetAnimationLength(clipHash));
            _timer.OnStart += () => _animator.CrossFade(clipHash, _crossfadeDuration);
            _timer.OnTimeOut += () => _animator.CrossFade(exitHash, _crossfadeDuration);
            _timer.Start();
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

            return -1f;
        }
    }
}