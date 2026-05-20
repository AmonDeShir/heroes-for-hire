using System;

public class IntervalTimer : Timer
{
    private readonly float _totalTime;
    private readonly float _interval;
    private float _nextInterval;
    
    public Action OnInterval = delegate { };

    public IntervalTimer(float totalTime, float intervalSeconds, bool oneShoot = false) : base(totalTime, oneShoot)
    {
        _totalTime = totalTime;
        _interval = intervalSeconds;
        _nextInterval = _totalTime - _interval;
    }

    public override void Start()
    {
        base.Start();
        _nextInterval = _totalTime - _interval;
    }

    public override void Tick(float deltaTime)
    {
        if (_paused)
        {
            return;
        }
        
        _timeLeft -= deltaTime;
        
        if (_timeLeft <= 0)
        {
            OnTimeOut();

            if (_oneShoot)
            {
                Pause();
                return;
            }

            Start();
        }
        else
        {
            while (_timeLeft <= _nextInterval && _nextInterval >= 0)
            {
                OnInterval();
                _nextInterval -= _interval;
            }
        }
    }
}


