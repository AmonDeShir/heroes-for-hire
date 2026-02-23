using System.Collections.Generic;

namespace Heroes.GOAP.Core
{
    public interface IPlanner<T>
    {
        public Plan<T> Plan(List<Action<T>> actions, List<Goal> goals, AgentContext ctx, int maxDepth);
    }
}