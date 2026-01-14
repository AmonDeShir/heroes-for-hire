using UnityEngine;

namespace GOAP
{
    public sealed class BuySwordStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly int _cost;

        private float _t;
        private bool _done;

        public BuySwordStrategy(GoapAgent agent, float duration, int cost)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _cost = Mathf.Max(0, cost);
        }

        public void Start()
        {
            _t = 0f;
            _done = false;
        }

        public void Stop()
        {
        }

        public void Update(float deltaTime)
        {
            _t += Mathf.Max(0f, deltaTime);

            if (!_done && _t >= _duration)
            {
                if (_agent.Gold >= _cost)
                {
                    _agent.Gold -= _cost;
                    _agent.HasSword = true;
                }

                _done = true;
                _t = _duration;
            }
        }
    }

    public sealed class BuyPickaxeStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly int _cost;

        private float _t;
        private bool _done;

        public BuyPickaxeStrategy(GoapAgent agent, float duration, int cost)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _cost = Mathf.Max(0, cost);
        }

        public void Start()
        {
            _t = 0f;
            _done = false;
        }

        public void Stop()
        {
        }

        public void Update(float deltaTime)
        {
            _t += Mathf.Max(0f, deltaTime);

            if (!_done && _t >= _duration)
            {
                if (_agent.Gold >= _cost)
                {
                    _agent.Gold -= _cost;
                    _agent.HasPickaxe = true;
                }

                _done = true;
                _t = _duration;
            }
        }
    }

    public sealed class BuyCoffeeStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly int _cost;

        private float _t;
        private bool _done;

        public BuyCoffeeStrategy(GoapAgent agent, float duration, int cost)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _cost = Mathf.Max(0, cost);
        }

        public void Start()
        {
            _t = 0f;
            _done = false;
        }

        public void Stop()
        {
        }

        public void Update(float deltaTime)
        {
            _t += Mathf.Max(0f, deltaTime);

            if (!_done && _t >= _duration)
            {
                if (_agent.Gold >= _cost)
                {
                    _agent.Gold -= _cost;
                    _agent.Coffee += 1;
                }

                _done = true;
                _t = _duration;
            }
        }
    }

    public sealed class DrinkCoffeeStrategy : IActionStrategy
    {
        public bool CanPreform => !Complete;
        public bool Complete => _t >= _duration;

        private readonly GoapAgent _agent;
        private readonly float _duration;
        private readonly float _gain;
        private readonly float _cap;

        private float _t;
        private bool _done;

        public DrinkCoffeeStrategy(GoapAgent agent, float duration, float staminaGain, float staminaCap)
        {
            _agent = agent;
            _duration = Mathf.Max(0.01f, duration);
            _gain = staminaGain;
            _cap = staminaCap;
        }

        public void Start()
        {
            _t = 0f;
            _done = false;
        }

        public void Stop()
        {
        }

        public void Update(float deltaTime)
        {
            _t += Mathf.Max(0f, deltaTime);

            if (!_done && _t >= _duration)
            {
                if (_agent.Coffee > 0)
                {
                    _agent.Coffee -= 1;
                    _agent.Stamina = Mathf.Clamp(_agent.Stamina + _gain, 0f, Mathf.Min(100f, _cap));
                }

                _done = true;
                _t = _duration;
            }
        }
    }
}