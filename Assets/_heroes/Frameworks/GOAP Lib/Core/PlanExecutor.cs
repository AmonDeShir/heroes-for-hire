using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    public class PlanExecutor<TAgent, TSnapshot> : IPlanExecutor where TSnapshot : IReadOnlyWorldSnapshot
    {
        private const float IdlePlanCooldownSeconds = 10f;
        private const float ReplanThrottleSeconds = 5f;
        private const float ImportanceHysteresis = 0.25f;

        protected TAgent agent;
        protected Archetype<TAgent, TSnapshot> archetype;
        protected IWorldState<TSnapshot> worldState;
        protected AgentContext<TSnapshot> context;
        protected IPlanner<TAgent, TSnapshot> planner;
        
        protected Plan<TAgent, TSnapshot> plan;
        private IdleAction<TAgent, TSnapshot> idleAction;
        private IActionStrategy idleStrategy;

        private float _nextIdlePlanAttemptAt;
        private float _nextReplanAt;
        private float _nextImportanceCheckAt;
        private int _lastFailedWorldVersion = -1;
        private bool _replanRequestedDeferred;
        private bool _replanRequestedImmediate;

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
            var now = Time.unscaledTime;
            var snapshot = worldState.CreateSnapshot();
            context = new AgentContext<TSnapshot>(context.state, snapshot);

            if (_replanRequestedImmediate)
            {
                AbortPlan();
                CalculatePlanInternal(snapshot.Version);
                _replanRequestedImmediate = false;
                _replanRequestedDeferred = false;
                _nextReplanAt = now + ReplanThrottleSeconds;
            }

            if (plan == null)
            {
                var worldChangedSinceFail = snapshot.Version != _lastFailedWorldVersion;
                if (worldChangedSinceFail || now >= _nextIdlePlanAttemptAt)
                {
                    var planned = CalculatePlanInternal(snapshot.Version);
                    if (!planned)
                    {
                        _lastFailedWorldVersion = snapshot.Version;
                        _nextIdlePlanAttemptAt = now + IdlePlanCooldownSeconds;
                    }
                }
            }
            else
            {
                if (_replanRequestedDeferred && now >= _nextReplanAt)
                {
                    CalculatePlanInternal(snapshot.Version);
                    _replanRequestedDeferred = false;
                    _nextReplanAt = now + ReplanThrottleSeconds;
                }

                if (now >= _nextImportanceCheckAt)
                {
                    _nextImportanceCheckAt = now + ReplanThrottleSeconds;

                    var current = plan.Goal?.Importance(context) ?? 0f;
                    var best = 0f;
                    if (archetype?.Goals != null)
                    {
                        for (var i = 0; i < archetype.Goals.Count; i++)
                        {
                            var g = archetype.Goals[i];
                            if (g == null)
                            {
                                continue;
                            }

                            var v = g.Importance(context);
                            if (v > best)
                            {
                                best = v;
                            }
                        }
                    }

                    if (best > current + ImportanceHysteresis)
                    {
                        _replanRequestedDeferred = true;
                    }
                }
            }

            if (plan != null && plan.Step == null && plan.RemainingSteps == 0)
            {
                if (plan.Goal == null || plan.Goal.IsAchieved(context))
                {
                    AbortPlan();
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
                    AbortPlan();
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
            CalculatePlanInternal(worldState.CreateSnapshot().Version);
        }

        public void RequestReplan(bool immediate)
        {
            if (immediate)
            {
                _replanRequestedImmediate = true;
            }
            else
            {
                _replanRequestedDeferred = true;
            }
        }

        public void AbortPlan()
        {
            plan?.Abort();
            plan = null;
            StopIdle();
        }

        private bool CalculatePlanInternal(int worldVersion)
        {
            if (archetype == null || planner == null)
            {
                return false;
            }

            var currentLevel = plan?.Goal?.Importance(context) ?? 0f;
            var goalsToCheck = archetype.Goals;

            if (plan?.Goal != null)
            {
                goalsToCheck = new List<Goal<TSnapshot>>(archetype.Goals.Where(g => g.Importance(context) > currentLevel));
            }

            var newPlan = planner.Plan(archetype.Actions, goalsToCheck, context, 50);
            if (newPlan is { IsEmpty: false })
            {
                plan = newPlan;
                return true;
            }

            return false;
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


