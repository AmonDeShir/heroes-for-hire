using System;
using UnityEngine;

namespace GOAP
{
    public sealed class EarnGoldStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly Func<int> _goldEarned;
        private readonly Func<float> _staminaCost;

        private float _t;
        private bool _paid;

        public EarnGoldStrategy(GoapAgent agent, float duration, Func<int> goldEarned, Func<float> staminaCost)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _goldEarned = goldEarned ?? (() => 0);
            _staminaCost = staminaCost ?? (() => 0f);
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

            var cost = Mathf.Max(0f, _staminaCost());
            var drain = (cost / _duration) * Mathf.Max(0f, deltaTime);
            _agent.Stamina = Mathf.Clamp(_agent.Stamina - drain, 0f, 100f);

            if (!_paid && _t >= _duration)
            {
                _agent.Gold += Mathf.Max(0, _goldEarned());
                _paid = true;
            }
        }
    }
}