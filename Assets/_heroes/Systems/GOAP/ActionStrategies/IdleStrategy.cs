using UnityEngine;

namespace GOAP
{
    public class IdleStrategy : IActionStrategy
    {
        public bool CanPreform => true;

        public bool Complete { get; private set; }

        private readonly Timer _timer;

        public IdleStrategy(float duration)
        {
            _timer = new Timer(duration, true);
            _timer.OnStart += () =>
            {
                Complete = false;
                Debug.Log("Idle Strategy Started");
            };
            
            _timer.OnTimeOut += () =>
            {
                Complete = true;
                Debug.Log("Idle strategy Complete");
            };
        }

        public void Start()
        {
            Debug.Log("Start Relax");
            _timer.Start();
        }

        public void Update(float deltaTime)
        {
            _timer.Tick(deltaTime);
            Debug.Log("Update Relax: " + _timer.TimeLeft);
        }
    }
}