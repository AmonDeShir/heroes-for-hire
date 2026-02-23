namespace Heroes.GOAP.Core
{
    public class AgentContext
    {
        public AgentState state { get; private set; }
        
        public AgentContext(AgentContext ctx)
        {
            this.state = ctx.state.Clone();
        }
        
        public AgentContext(AgentState state)
        {
            this.state = state.Clone();
        }
    }
}