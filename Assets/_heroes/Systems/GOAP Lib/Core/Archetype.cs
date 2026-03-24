using System.Collections.Generic;

namespace Heroes.GOAP.Core
{
    public class Archetype<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public List<Action<TAgent, TSnapshot>> Actions { get; private set; }
        public List<Goal<TSnapshot>> Goals { get; private set; }
        public AgentState BaseState { get; private set; }
        public List<IdleAction<TAgent, TSnapshot>> IdleActions { get; private set; }

        public Archetype(List<Action<TAgent, TSnapshot>> actions, List<Goal<TSnapshot>> goals, AgentState baseState)
        {
            Actions = actions;
            Goals = goals;
            BaseState = baseState;
            IdleActions = new List<IdleAction<TAgent, TSnapshot>>();
        }

        public AgentState CreateState()
        {
            return BaseState.Clone();
        }

        public Goal<TSnapshot>.Builder CreateGoal()
        {
            return new Goal<TSnapshot>.Builder();
        }
        
        public Action<TAgent, TSnapshot>.Builder CreateAction()
        {
            return new Action<TAgent, TSnapshot>.Builder();
        }
    }
}
