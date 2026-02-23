using System;

namespace Heroes.GOAP.Core
{
    public class Action<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        
        public Func<AgentContext<TSnapshot>, AgentState> Effect { get; private set; }
        public Func<AgentContext<TSnapshot>, bool> PreConditions { get; private set; }
        public Func<AgentContext<TSnapshot>, float> Time { get; private set; }
        public Func<TAgent, AgentContext<TSnapshot>, IActionStrategy> Implementation { get; private set; }

        private Action()
        {
            Name = string.Empty;
            Description = string.Empty;
            Effect = (_) => new AgentState();
            PreConditions = (_) => true;
            Time = (_) => 1f;
            Implementation = (_, __) => new EmptyImplementation();
        }
        
        private class EmptyImplementation : IActionStrategy
        {
            public bool CanPerform { get; } = true;
            public bool Complete { get; } = true;
        }

        public class Builder
        {
            private Action<TAgent, TSnapshot> action;
            
            public Builder()
            {
                action = new Action<TAgent, TSnapshot>();
            }

            public Action<TAgent, TSnapshot> Build()
            {
                return action;
            }

            public Builder WithName(string name)
            {
                action.Name = name;
                return this;
            }

            public Builder WithDescription(string description)
            {
                action.Description = description;
                return this;
            }

            public Builder WithEffect(Func<AgentContext<TSnapshot>, AgentState> effect)
            {
                action.Effect = effect;
                return this;
            }

            public Builder WithPreCondition(Func<AgentContext<TSnapshot>, bool> preCondition)
            {
                action.PreConditions = preCondition;
                return this;
            }

            public Builder WithTime(Func<AgentContext<TSnapshot>, float> time)
            {
                action.Time = time;
                return this;
            }

            public Builder WithImplementation(Func<TAgent, AgentContext<TSnapshot>, IActionStrategy> implementation)
            {
                action.Implementation = implementation;
                return this;
            }
        }
    }
}
