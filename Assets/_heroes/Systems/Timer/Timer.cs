using System;

public class Timer
{
    private float _waitTime;
    private bool _oneShoot;
    private bool _paused;
    private float _timeLeft;
    
    public bool Stopped => _paused;
    
    public Action OnTimeOut = delegate { };
    public Action OnStart = delegate { };

    public float TimeLeft => _timeLeft;
    
    public Timer(float timeSec, bool oneShoot = false)
    {
        _waitTime = timeSec;
        _oneShoot = oneShoot;
        _paused = false;
    }

    public void Start()
    {
        _timeLeft = _waitTime;
        _paused = false;
        OnStart();
    }

    public void Pause()
    {
        _paused = true;
    }

    public void Resume()
    {
        _paused = false;
    }

    public void Tick(float deltaTime)
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
    }
}