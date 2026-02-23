using System.Collections.Generic;

namespace Heroes.GOAP.Core
{
    public class Plan<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public Goal<TSnapshot> Goal { get; private set; }
        public Action<TAgent, TSnapshot> Step { get; private set; }
        
        protected IActionStrategy strategy;
        private readonly Stack<Action<TAgent, TSnapshot>> steps;

        public Plan(Goal<TSnapshot> goal, Stack<Action<TAgent, TSnapshot>> steps)
        {
            this.Goal = goal;
            this.steps = steps;
        }

        public int RemainingSteps => steps.Count;
        public bool IsEmpty => steps.Count == 0 && Step == null;

        public bool StartNextStep(AgentContext<TSnapshot> ctx, TAgent agent)
        {
            if (steps.Count == 0)
            {
                Step = null;
                strategy = null;
                
                return false;
            }

            Step = steps.Pop();

            if (!Step.PreConditions(ctx))
            {
                Step = null;
                Goal = null;
                strategy = null;

                return false;
            }

            strategy = Step.Implementation(agent, ctx);
            strategy.Start();
            
            return true;
        }

        public void Update(float deltaTime)
        {
            if (strategy == null)
            {
                return;
            }

            strategy.Update(deltaTime);

            if (strategy.Complete)
            {
                strategy.Stop();
                strategy = null;
                Step = null;
            }
        }
    }

}
