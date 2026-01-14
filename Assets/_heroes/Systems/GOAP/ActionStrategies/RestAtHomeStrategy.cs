using UnityEngine;

namespace GOAP
{
    public class RestAtHomeStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly float _staminaGain;
        private float _t;
        private float _startStamina;

        public RestAtHomeStrategy(GoapAgent agent, float duration, float staminaGain)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _staminaGain = staminaGain;
        }

        public void Start()
        {
            _t = 0f;
            _startStamina = _agent.Stamina;
        }

        public void Stop()
        {
        }

        public void Update(float deltaTime)
        {
            _t += Mathf.Max(0f, deltaTime);
            var k = Mathf.Clamp01(_t / _duration);
            _agent.Stamina = Mathf.Clamp(_startStamina + _staminaGain * k, 0f, 100f);
        }
    }
}