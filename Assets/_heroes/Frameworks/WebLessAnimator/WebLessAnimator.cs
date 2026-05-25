using UnityEngine;
using System.Collections.Generic;

namespace WebLess
{
    public abstract class WebLessAnimator : MonoBehaviour
    {
        protected const float _crossfadeDuration = 0.1f;

        protected Animator _animator;
        protected Timer _timer;

        private readonly Dictionary<int, float> _lengthByHash = new();
        private int _activeTimedClipHash;
        private int _activeTimedExitHash;

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
            if (_timer != null && !_timer.Stopped && _activeTimedClipHash == clipHash && _activeTimedExitHash == exitHash)
            {
                return;
            }

            if (_animator == null)
            {
                return;
            }

            if (!_animator.HasState(0, clipHash))
            {
                return;
            }

            var length = GetAnimationLength(clipHash);
            if (length <= 0f)
            {
                length = 0.1f;
            }

            _activeTimedClipHash = clipHash;
            _activeTimedExitHash = exitHash;
            _timer = new Timer(length, oneShoot: true);
            _timer.OnStart += () => _animator.CrossFade(clipHash, _crossfadeDuration);
            _timer.OnTimeOut += () =>
            {
                var resolvedExit = exitHash;
                if (!_animator.HasState(0, resolvedExit))
                {
                    var current = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
                    if (_animator.HasState(0, current) && current != clipHash)
                    {
                        resolvedExit = current;
                    }
                }

                if (_animator.HasState(0, resolvedExit))
                {
                    _animator.CrossFade(resolvedExit, _crossfadeDuration);
                }
            };
            _timer.Start();
        }
        
        protected void PlayAnimation(int clipHash)
        {
            if (_animator == null || !_animator.HasState(0, clipHash))
            {
                return;
            }
            _animator.CrossFade(clipHash, _crossfadeDuration);
        }

        public float GetAnimationLength(int hash)
        {
            if (_lengthByHash.TryGetValue(hash, out var cached) && cached > 0f)
            {
                return cached;
            }

            foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
            {
                if (Animator.StringToHash(clip.name) == hash)
                {
                    _lengthByHash[hash] = clip.length;
                    return _lengthByHash[hash];
                }
            }

            Debug.LogWarning($"Animation clip not found for hash {hash} on {name}.");

            return -1f;
        }
    }
}

