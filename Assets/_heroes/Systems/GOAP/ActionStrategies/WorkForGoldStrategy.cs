using UnityEngine;

namespace GOAP
{
    public class WorkForGoldStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly int _goldEarned;
        private readonly float _staminaCost;

        private float _t;
        private bool _paid;

        public WorkForGoldStrategy(GoapAgent agent, float duration, int goldEarned, float staminaCost)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _goldEarned = Mathf.Max(0, goldEarned);
            _staminaCost = Mathf.Max(0f, staminaCost);
        }

        public void Start()
        {
            _t = 0f;
            _paid = false;
        }

        public void Stop()
        {
        }

        public void Update(float deltaTime)
        {
            _t += Mathf.Max(0f, deltaTime);

            var drain = (_staminaCost / _duration) * Mathf.Max(0f, deltaTime);
            _agent.Stamina = Mathf.Clamp(_agent.Stamina - drain, 0f, 100f);

            if (!_paid && _t >= _duration)
            {
                _agent.Gold += _goldEarned;
                _paid = true;
            }
        }
    }
}