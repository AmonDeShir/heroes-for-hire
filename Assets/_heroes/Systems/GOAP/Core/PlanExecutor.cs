using System.Collections.Generic;
using System.Linq;

namespace Heroes.GOAP.Core
{
    public class PlanExecutor<TAgent, TSnapshot> : IPlanExecutor where TSnapshot : IReadOnlyWorldSnapshot
    {
        protected TAgent agent;
        protected Archetype<TAgent, TSnapshot> archetype;
        protected IWorldState<TSnapshot> worldState;
        protected AgentContext<TSnapshot> context;
        protected IPlanner<TAgent, TSnapshot> planner;
        
        protected Plan<TAgent, TSnapshot> plan;
        public Goal<TSnapshot> Goal => plan.Goal;
        
        public event System.Action OnNextStepLoaded;

        public PlanExecutor(TAgent agent, Archetype<TAgent, TSnapshot> archetype, IWorldState<TSnapshot> worldState)
        {
            this.agent = agent;
            this.archetype = archetype;
            this.worldState = worldState;

            var snapshot = worldState.CreateSnapshot();
            context = new AgentContext<TSnapshot>(archetype.CreateState(), snapshot);
            planner = new Planner<TAgent, TSnapshot>();
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
                goalsToCheck = new List<Goal<TSnapshot>>(archetype.Goals.Where(g => g.Importance(context) > currentLevel));
            }

            var snapshot = worldState.CreateSnapshot();
            context = new AgentContext<TSnapshot>(context.state, snapshot);
            var newPlan = planner.Plan(archetype.Actions, goalsToCheck, context, 50);

            if (newPlan is { IsEmpty: false })
            {
                plan = newPlan;
            }
        }
    }
}
