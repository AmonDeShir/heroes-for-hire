using System.Collections.Generic;

namespace Heroes.GOAP.Core
{
    public interface IPlanner<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public Plan<TAgent, TSnapshot> Plan(List<Action<TAgent, TSnapshot>> actions, List<Goal<TSnapshot>> goals, AgentContext<TSnapshot> ctx, int maxDepth);
    }
}


