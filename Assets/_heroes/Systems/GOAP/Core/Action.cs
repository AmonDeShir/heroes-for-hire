using System;

namespace Heroes.GOAP.Core
{
    public class Action<T>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        
        public Func<AgentContext, AgentState> Effect { get; private set; }
        public Func<AgentContext, bool> PreConditions { get; private set; }
        public Func<AgentContext, float> Time { get; private set; }
        public Func<T, IActionStrategy> Implementation { get; private set; }

        private Action()
        {
            Name = string.Empty;
            Description = string.Empty;
            Effect = (_) => new AgentState();
            PreConditions = (_) => true;
            Time = (_) => 1f;
            Implementation = (_) => new EmptyImplementation();
        }
        
        private class EmptyImplementation : IActionStrategy
        {
            public bool CanPreform { get; } = true;
            public bool Complete { get; } = true;
        }

        public class Builder
        {
            private Action<T> action;
            
            public Builder()
            {
                action = new Action<T>();
            }

            public Action<T> Build()
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

            public Builder WithEffect(Func<AgentContext, AgentState> effect)
            {
                action.Effect = effect;
                return this;
            }

            public Builder WithPreCondition(Func<AgentContext, bool> preCondition)
            {
                action.PreConditions = preCondition;
                return this;
            }

            public Builder WithTime(Func<AgentContext, float> time)
            {
                action.Time = time;
                return this;
            }

            public Builder WithImplementation(Func<T, IActionStrategy> implementation)
            {
                action.Implementation = implementation;
                return this;
            }
        }
    }
}