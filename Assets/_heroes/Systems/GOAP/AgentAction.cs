using System.Collections.Generic;

namespace GOAP
{
    public class AgentAction
    {
        public string Name { get; }
        public float Cost { get; private set; }
        
        public bool Complete => _strategy.Complete;
        
        public HashSet<AgentBelief> Preconditions { get; }
        public HashSet<AgentBelief> Effects { get; }

        private IActionStrategy _strategy;
        
        public AgentAction(string name, float cost = 1)
        {
            Name = name;
            Cost = cost;
            Preconditions = new HashSet<AgentBelief>();
            Effects = new HashSet<AgentBelief>();
        }

        public void Start()
        {
            _strategy.Start();
        }
        
        public void Stop()
        {
            _strategy.Stop();
        }
        
        public void Update(float deltaTime)
        {
            if (_strategy.CanPreform)
            {
                _strategy.Update(deltaTime);
            }

            if (!_strategy.Complete)
            {
                return;
            }

            foreach (var effect in Effects)
            {
                effect.Evaluate();
            }
        }

        public class Builder
        {
            private readonly AgentAction _action;

            public Builder(string name)
            {
                _action = new AgentAction(name);
            }

            public Builder WithCost(float cost)
            {
                _action.Cost = cost;
                
                return this;
            }
            
            public Builder WithStrategy(IActionStrategy strategy)
            {
                _action._strategy = strategy;
                
                return this;
            }
            
            public Builder WithPrecondition(AgentBelief precondition)
            {
                _action.Preconditions.Add(precondition);
                
                return this;
            }
            
            public Builder WithEffect(AgentBelief effect)
            {
                _action.Effects.Add(effect);
                
                return this;
            }
            
            public AgentAction Build()
            {
                return _action;
            }
        }
    }
}