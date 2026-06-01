using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    public class PlanExecutor<TAgent, TSnapshot> : IPlanExecutor where TSnapshot : IReadOnlyWorldSnapshot
    {
        private const float IdlePlanCooldownSeconds = 20f;
        private const float ReplanThrottleSeconds = 15f;
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

        private Task<Plan<TAgent, TSnapshot>> _planningTask;

        public bool IsPlanning => _planningTask != null;

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

            if (_planningTask != null)
            {
                if (_planningTask.IsCompleted)
                {
                    if (_planningTask.Status == TaskStatus.RanToCompletion)
                    {
                        var res = _planningTask.Result;
                        if (res is { IsEmpty: false })
                        {
                            plan = res;
                        }
                    }

                    _planningTask = null;
                    _replanRequestedImmediate = false;
                    _replanRequestedDeferred = false;
                    _nextReplanAt = now + ReplanThrottleSeconds;
                }
                else
                {
                    if (plan == null)
                    {
                        UpdateIdle(deltaTime);
                    }
                    else
                    {
                        plan.Update(deltaTime);
                    }

                    return;
                }
            }

            if (_replanRequestedImmediate)
            {
                StartPlanningAsync(onlyBetterThanCurrentGoal: false);
                return;
            }

            if (plan == null)
            {
                var worldChangedSinceFail = snapshot.Version != _lastFailedWorldVersion;
                if (worldChangedSinceFail || now >= _nextIdlePlanAttemptAt)
                {
                    StartPlanningAsync(onlyBetterThanCurrentGoal: false);
                    if (_planningTask == null)
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
                    StartPlanningAsync(onlyBetterThanCurrentGoal: true);
                    return;
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
                                                                                                           
                    _lastFailedWorldVersion = snapshot.Version;
                    _nextIdlePlanAttemptAt = now + IdlePlanCooldownSeconds;
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
            StartPlanningAsync(onlyBetterThanCurrentGoal: false);
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

        private void StartPlanningAsync(bool onlyBetterThanCurrentGoal)
        {
            if (archetype == null || planner == null)
            {
                return;
            }

            if (_planningTask != null)
            {
                return;
            }

            var current = context;
            var currentPlan = plan;
            var currentLevel = onlyBetterThanCurrentGoal ? (currentPlan?.Goal?.Importance(current) ?? 0f) : 0f;
            var actions = archetype.Actions;
            var goals = archetype.Goals;

            var goalsToCheck = goals;
            if (onlyBetterThanCurrentGoal && currentPlan?.Goal != null)
            {
                goalsToCheck = new List<Goal<TSnapshot>>(goals.Where(g => g.Importance(current) > currentLevel));
            }

            var ctxCopy = new AgentContext<TSnapshot>(current);
            var goalsCopy = goalsToCheck;
            _planningTask = Task.Run(() => planner.Plan(actions, goalsCopy, ctxCopy, 50));
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


