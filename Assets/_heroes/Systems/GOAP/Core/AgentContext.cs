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
    }
}
