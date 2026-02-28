using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        private IdleAction<TAgent, TSnapshot> idleAction;
        private IActionStrategy idleStrategy;
        public Goal<TSnapshot> Goal => plan.Goal;

        public bool IsIdleActive => idleStrategy != null;
        public string IdleActionName => idleAction?.Name;

        public TAgent Agent => agent;
        public Plan<TAgent, TSnapshot> CurrentPlan => plan;
        public AgentContext<TSnapshot> Context => context;
        public Archetype<TAgent, TSnapshot> Archetype => archetype;
        public IWorldState<TSnapshot> WorldState => worldState;
        
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

            if (plan != null && plan.Step == null && plan.RemainingSteps == 0)
            {
                if (plan.Goal == null || plan.Goal.IsAchieved(context))
                {
                    plan = null;
                }
            }

            if (plan != null)
            {
                StopIdle();
            }

            if (plan is { RemainingSteps: > 0 } && plan.Step == null)
            {
                OnNextStepLoaded?.Invoke();

                if (!plan.StartNextStep(context, agent))
                {
                    plan = null;
                }
            }

            plan?.Update(deltaTime);

            if (plan == null)
            {
                UpdateIdle(deltaTime);
            }
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

        private void UpdateIdle(float deltaTime)
        {
            if (archetype == null || archetype.IdleActions == null || archetype.IdleActions.Count == 0)
            {
                return;
            }

            if (idleStrategy == null)
            {
                idleAction = PickIdleAction();
                if (idleAction == null || idleAction.Implementation == null)
                {
                    return;
                }

                idleStrategy = idleAction.Implementation(agent, context);
                idleStrategy.Start();
            }

            idleStrategy.Update(deltaTime);

            if (idleStrategy.Complete)
            {
                idleStrategy.Stop();
                idleStrategy = null;
                idleAction = null;
            }
        }

        private void StopIdle()
        {
            if (idleStrategy == null)
            {
                return;
            }

            idleStrategy.Stop();
            idleStrategy = null;
            idleAction = null;
        }

        private IdleAction<TAgent, TSnapshot> PickIdleAction()
        {
            if (archetype.IdleActions.Count == 1)
            {
                return archetype.IdleActions[0];
            }

            var index = UnityEngine.Random.Range(0, archetype.IdleActions.Count);
            return archetype.IdleActions[index];
        }
    }
}
