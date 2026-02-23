using System.Collections.Generic;
using System.Linq;

namespace Heroes.GOAP.Core
{
    public class PlanExecutor<T>
    {
        protected T agent;
        protected Archetype<T> archetype;
        protected AgentContext context;
        protected Planner<T> planner;
        
        protected Plan<T> plan;
        public Goal Goal => plan.Goal;
        
        public event System.Action OnNextStepLoaded;

        public PlanExecutor(T agent, Archetype<T> archetype)
        {
            this.agent = agent;
            this.archetype = archetype;

            context = new AgentContext(archetype.CreateState());
            planner = new Planner<T>();
        }

        public void Update(float deltaTime)
        {
            if (plan == null)
            {
                CalculatePlan();
            }

            if (plan is { RemainingSteps: > 0 })
            {
                OnNextStepLoaded?.Invoke();
                
                if (!plan.StartNextStep(context, agent))
                {
                    plan = null;    
                }
            }

            plan?.Update(deltaTime);
        }
        
        public void CalculatePlan()
        {
            var currentLevel = plan?.Goal?.Importance(context) ?? 0f;
            var goalsToCheck = archetype.Goals;

            if (plan?.Goal != null)
            {
                goalsToCheck = new List<Goal>(archetype.Goals.Where(g => g.Importance(context) > currentLevel));
            }

            var newPlan = planner.Plan(archetype.Actions, goalsToCheck, context, 50);

            if (newPlan is { IsEmpty: false })
            {
                plan = newPlan;
            }
        }
    }
}