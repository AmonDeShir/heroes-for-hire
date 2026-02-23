using System.Collections.Generic;

namespace Heroes.GOAP.Core
{
    public class Archetype<T>
    {
        public List<Action<T>> Actions { get; private set; }
        public List<Goal> Goals { get; private set; }

        public AgentState BaseState { get; private set; }

        public Archetype(List<Action<T>> actions, List<Goal> goals, AgentState baseState)
        {
            Actions = actions;
            Goals = goals;
            BaseState = baseState;
        }

        public AgentState CreateState()
        {
            return BaseState.Clone();
        }
    }
}
