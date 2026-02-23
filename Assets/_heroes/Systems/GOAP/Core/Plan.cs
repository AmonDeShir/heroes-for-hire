using System.Collections.Generic;

namespace Heroes.GOAP.Core
{
    public class Plan<T>
    {
        public Goal Goal { get; private set; }
        public Action<T> Step { get; private set; }
        
        protected IActionStrategy strategy;
        private readonly Stack<Action<T>> steps;

        public Plan(Goal goal, Stack<Action<T>> steps)
        {
            this.Goal = goal;
            this.steps = steps;
        }

        public int RemainingSteps => steps.Count;
        public bool IsEmpty => steps.Count == 0 && Step == null;

        public bool StartNextStep(AgentContext ctx, T agent)
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

            strategy = Step.Implementation(agent);
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