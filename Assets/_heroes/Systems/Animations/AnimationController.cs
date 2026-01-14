using UnityEngine;

public abstract class AnimationController : MonoBehaviour 
{
    private const float _crossfadeDuration = 0.1f;
    
    private Animator _animator;
    private Timer _timer;
    
    private float _animationLength;
    
    [HideInInspector] 
    public int LocomotionClip = Animator.StringToHash("Locomotion");
    
    [HideInInspector] 
    public int SpeedHash = Animator.StringToHash("Speed");
    
    [HideInInspector] 
    public int AttackClip = Animator.StringToHash("Attack");
    
    void Awake() 
    {
        _animator = GetComponentInChildren<Animator>();
        
        SetLocomotionClip();
        SetAttackClip();
        SetSpeedHash();
    }

    public void SetSpeed(float speed) => _animator.SetFloat(SpeedHash, speed);
    public void Attack() => PlayAnimationUsingTimer(AttackClip);

    private void Update()
    {
        _timer?.Tick(Time.deltaTime);   
    }

    private void PlayAnimationUsingTimer(int clipHash) 
    {
        _timer = new Timer(GetAnimationLength(clipHash));
        _timer.OnStart += () => _animator.CrossFade(clipHash, _crossfadeDuration);
        _timer.OnTimeOut += () => _animator.CrossFade(LocomotionClip, _crossfadeDuration);
        _timer.Start();
    }

    public float GetAnimationLength(int hash) {
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

    protected abstract void SetLocomotionClip();
    protected abstract void SetAttackClip();
    protected abstract void SetSpeedHash();
}