namespace Heroes.GOAP.Core
{
    public class AgentContext<TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public AgentState state { get; private set; }
        public TSnapshot world { get; private set; }

        public AgentContext(AgentContext<TSnapshot> ctx)
        {
            state = ctx.state.Clone();
            world = ctx.world;
        }

        public AgentContext(AgentState state, TSnapshot world)
        {
            this.state = state.Clone();
            this.world = world;
        }

        public void SetState(AgentState newState)
        {
            state = newState;
        }

        public void MutateState(RefAction<AgentState> mutator)
        {
            var current = state;
            mutator(ref current);
            state = current;
        }
    }
}
