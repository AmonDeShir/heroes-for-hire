using UnityEngine;
using System.Collections.Generic;

namespace WebLess
{
    public abstract class WebLessAnimator : MonoBehaviour
    {
        protected const float _crossfadeDuration = 0.1f;

        protected Animator _animator;
        protected Timer _timer;

        private readonly Dictionary<int, float> _animationLengths = new();
        private int _activeTimedClipHash;
        private int _activeExitClipHash;
        private bool _freezeOnTimeout;

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
            if (_timer != null && !_timer.Stopped && _activeTimedClipHash == clipHash && _activeExitClipHash == exitHash)
            {
                return;
            }

            StopTimedAnimation();
            _animator.speed = 1f;

            var length = GetAnimationLength(clipHash);
            if (length <= 0f)
            {
                length = 0.1f;
            }

            _activeTimedClipHash = clipHash;
            _activeExitClipHash = exitHash;
            _freezeOnTimeout = false;
            _timer = new Timer(length, oneShoot: true);
            _timer.OnStart += () => _animator.CrossFade(clipHash, _crossfadeDuration);
            _timer.OnTimeOut += () =>
            {
                if (_freezeOnTimeout)
                {
                    _animator.speed = 0f;
                }
                else
                {
                    _animator.CrossFade(exitHash, _crossfadeDuration);
                }
            };
            _timer.Start();
        }
        
        protected void PlayAnimation(int clipHash)
        {
            StopTimedAnimation();
            _animator.speed = 1f;
            _animator.CrossFade(clipHash, _crossfadeDuration);
        }

        protected void PlayAnimationAndFreeze(int clipHash)
        {
            if (_timer != null && !_timer.Stopped && _activeTimedClipHash == clipHash && _freezeOnTimeout)
            {
                return;
            }

            StopTimedAnimation();
            _animator.speed = 1f;

            var length = GetAnimationLength(clipHash);
            if (length <= 0f)
            {
                length = 0.1f;
            }

            _activeTimedClipHash = clipHash;
            _activeExitClipHash = 0;
            _freezeOnTimeout = true;
            _timer = new Timer(length, oneShoot: true);
            _timer.OnStart += () => _animator.CrossFade(clipHash, _crossfadeDuration);
            _timer.OnTimeOut += () => _animator.speed = 0f;
            _timer.Start();
        }

        protected void StopTimedAnimation()
        {
            _timer = null;
            _activeTimedClipHash = 0;
            _activeExitClipHash = 0;
            _freezeOnTimeout = false;
        }

        public void ResumeAnimator()
        {
            StopTimedAnimation();

            if (_animator != null)
            {
                _animator.speed = 1f;
            }
        }

        public float GetAnimationLength(int hash)
        {
            if (_animationLengths.TryGetValue(hash, out var cached) && cached > 0f)
            {
                return cached;
            }

            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (Animator.StringToHash(clip.name) == hash)
                {
                    _animationLengths[hash] = clip.length;
                    return clip.length;
                }
            }

            Debug.LogWarning($"Animation clip not found for hash {hash} on {name}.");

            return -1f;
        }
    }
}
