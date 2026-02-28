using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heroes.GOAP.Core.Debug
{
    public sealed class GoapDebugAdapter<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        private readonly PlanExecutor<TAgent, TSnapshot> executor;
        private readonly IBeliefNameProvider beliefNameProvider;

        public GoapDebugAdapter(PlanExecutor<TAgent, TSnapshot> executor)
        {
            this.executor = executor;
            if (executor != null && executor.Agent is IBeliefNameProvider provider)
            {
                beliefNameProvider = provider;
            }
        }

        public bool TryBuildSnapshot(out GoapDebugSnapshot snapshot)
        {
            if (executor == null)
            {
                snapshot = null;
                return false;
            }

            var ctx = executor.Context;
            if (ctx == null)
            {
                snapshot = null;
                return false;
            }

            var agentName = ResolveAgentName(executor.Agent);
            var planSnapshot = BuildPlan(executor.CurrentPlan, ctx);
            var goalsSnapshot = BuildGoals(executor.Archetype, ctx);
            var actionsSnapshot = BuildActions(executor.Archetype, ctx);
            var memorySnapshot = BuildMemory(ctx);
            var worldSnapshot = BuildWorld(executor.WorldState);
            var idleName = executor.IdleActionName ?? string.Empty;
            if (string.IsNullOrEmpty(idleName) && executor.Archetype?.IdleActions != null && executor.Archetype.IdleActions.Count > 0)
            {
                idleName = executor.Archetype.IdleActions[0]?.Name ?? string.Empty;
            }
            var idleSnapshot = new GoapDebugIdle(executor.IsIdleActive, idleName);

            snapshot = new GoapDebugSnapshot(agentName, planSnapshot, goalsSnapshot, actionsSnapshot, memorySnapshot, worldSnapshot, idleSnapshot);
            return true;
        }

        private static string ResolveAgentName(TAgent agent)
        {
            if (agent is Object unityObject)
            {
                if (unityObject == null)
                {
                    return "<destroyed>";
                }

                return unityObject.name;
            }

            return agent != null ? agent.ToString() : "<null>";
        }

        private GoapDebugPlan BuildPlan(Plan<TAgent, TSnapshot> plan, AgentContext<TSnapshot> ctx)
        {
            var steps = new List<GoapDebugPlanStep>();
            var currentIndex = -1;
            var goalName = string.Empty;

            if (plan == null)
            {
                return new GoapDebugPlan(goalName, currentIndex, steps);
            }

            goalName = plan.Goal?.Name ?? string.Empty;

            var actions = new List<Action<TAgent, TSnapshot>>();
            if (plan.Step != null)
            {
                currentIndex = 0;
                actions.Add(plan.Step);
            }

            var remaining = plan.GetRemainingSteps();
            if (remaining != null && remaining.Length > 0)
            {
                actions.AddRange(remaining);
            }

            if (currentIndex < 0 && actions.Count > 0)
            {
                currentIndex = 0;
            }

            var predictedState = ctx.state.Clone();
            for (var i = 0; i < actions.Count; i++)
            {
                var stepCtx = new AgentContext<TSnapshot>(predictedState, ctx.world);
                var action = actions[i];
                var step = BuildStep(action, stepCtx, plan.Goal, out var nextState);
                steps.Add(step);
                predictedState = nextState;
            }

            return new GoapDebugPlan(goalName, currentIndex, steps);
        }

        private GoapDebugPlanStep BuildStep(Action<TAgent, TSnapshot> action, AgentContext<TSnapshot> ctx, Goal<TSnapshot> goal, out AgentState nextState)
        {
            var name = action.Name;
            var description = action.Description;
            var estimatedTime = 0f;
            var preconditionsMet = false;
            var preconditionsLabel = "Preconditions: n/a";
            var effectLabel = "Effect: n/a";
            var previewLocation = Vector2.zero;
            IReadOnlyList<GoapDebugBelief> previewBeliefs = Array.Empty<GoapDebugBelief>();
            var goalAchieved = false;
            var goalHeuristic = 0f;
            nextState = ctx.state;

            if (ctx != null)
            {
                preconditionsMet = action.PreConditions(ctx);
                preconditionsLabel = BuildPreconditionsLabel(action, preconditionsMet);
                estimatedTime = action.Time(ctx);
                effectLabel = BuildEffectLabel(action, ctx);

                nextState = action.Effect(ctx);
                previewLocation = nextState.Location;
                previewBeliefs = BuildBeliefs(nextState);
                if (goal != null)
                {
                    var afterCtx = new AgentContext<TSnapshot>(nextState, ctx.world);
                    goalAchieved = goal.Achieved(afterCtx);
                    goalHeuristic = goal.Heuristic(afterCtx);
                }
            }

            return new GoapDebugPlanStep(name, description, estimatedTime, preconditionsMet, preconditionsLabel, effectLabel,
                previewLocation, previewBeliefs, goalAchieved, goalHeuristic);
        }

        private string BuildPreconditionsLabel(Action<TAgent, TSnapshot> action, bool met)
        {
            var description = action.PreconditionsDescription;
            if (!string.IsNullOrWhiteSpace(description))
            {
                return $"Preconditions: {description} ({(met ? "met" : "not met")})";
            }

            var methodName = action.PreConditions?.Method?.Name ?? "(unknown)";
            return $"Preconditions: {methodName} ({(met ? "met" : "not met")})";
        }

        private string BuildEffectLabel(Action<TAgent, TSnapshot> action, AgentContext<TSnapshot> ctx)
        {
            var before = ctx.state;
            var after = action.Effect(ctx);

            if (before.Equals(after))
            {
                return "Effect: no change";
            }

            var sb = new StringBuilder();
            var changes = 0;
            sb.Append("Effect: ");

            if (!string.IsNullOrWhiteSpace(action.EffectDescription))
            {
                sb.Append(action.EffectDescription);
                changes++;
            }

            if (!before.Location.Equals(after.Location))
            {
                if (changes > 0)
                {
                    sb.Append("; ");
                }

                sb.Append("Location ");
                sb.Append(before.Location);
                sb.Append(" -> ");
                sb.Append(after.Location);
                changes++;
            }

            var beforeCount = before.BeliefCount;
            var afterCount = after.BeliefCount;
            var count = beforeCount < afterCount ? beforeCount : afterCount;

            for (var i = 0; i < count; i++)
            {
                var a = before.GetBelieve(i);
                var b = after.GetBelieve(i);
                if (!a.Equals(b))
                {
                    if (changes > 0)
                    {
                        sb.Append("; ");
                    }

                    sb.Append(GetBeliefLabel(i));
                    sb.Append(" ");
                    sb.Append(a.ToString("0.###"));
                    sb.Append(" -> ");
                    sb.Append(b.ToString("0.###"));
                    changes++;
                }
            }

            if (changes == 0)
            {
                return "Effect: no change";
            }

            return sb.ToString();
        }

        private string GetBeliefLabel(int index)
        {
            if (beliefNameProvider != null && beliefNameProvider.TryGetBeliefName(index, out var name) && !string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return $"Belief[{index}]";
        }

        private static List<GoapDebugGoal> BuildGoals(Archetype<TAgent, TSnapshot> archetype, AgentContext<TSnapshot> ctx)
        {
            var goals = new List<GoapDebugGoal>();
            if (archetype == null || archetype.Goals == null)
            {
                return goals;
            }

            foreach (var goal in archetype.Goals)
            {
                var importance = goal.Importance(ctx);
                var heuristic = goal.Heuristic(ctx);
                var achieved = goal.Achieved(ctx);
                goals.Add(new GoapDebugGoal(goal.Name, goal.Priority, importance, heuristic, achieved));
            }

            return goals;
        }

        private List<GoapDebugAction> BuildActions(Archetype<TAgent, TSnapshot> archetype, AgentContext<TSnapshot> ctx)
        {
            var actions = new List<GoapDebugAction>();
            if (archetype == null)
            {
                return actions;
            }

            if (archetype.Actions != null)
            {
                foreach (var action in archetype.Actions)
                {
                    var preconditionsMet = action.PreConditions(ctx);
                    var preconditionsLabel = BuildPreconditionsLabel(action, preconditionsMet);
                    var effectLabel = BuildEffectLabel(action, ctx);
                    var time = action.Time(ctx);
                    actions.Add(new GoapDebugAction(action.Name, action.Description, time, preconditionsMet, preconditionsLabel, effectLabel));
                }
            }

            if (archetype.IdleActions != null && archetype.IdleActions.Count > 0)
            {
                foreach (var idle in archetype.IdleActions)
                {
                    if (idle == null)
                    {
                        continue;
                    }

                    var name = string.IsNullOrEmpty(idle.Name) ? "Idle" : $"Idle: {idle.Name}";
                    actions.Add(new GoapDebugAction(name, idle.Description, 0f, true, "Idle", "Strategy only"));
                }
            }

            return actions;
        }

        private GoapDebugMemory BuildMemory(AgentContext<TSnapshot> ctx)
        {
            var state = ctx.state;
            var beliefs = BuildBeliefs(state);
            return new GoapDebugMemory(state.Location, beliefs);
        }

        private IReadOnlyList<GoapDebugBelief> BuildBeliefs(AgentState state)
        {
            var beliefs = new List<GoapDebugBelief>();
            var count = state.BeliefCount;
            for (var i = 0; i < count; i++)
            {
                var name = string.Empty;
                if (beliefNameProvider != null)
                {
                    beliefNameProvider.TryGetBeliefName(i, out name);
                }

                beliefs.Add(new GoapDebugBelief(i, name, state.GetBelieve(i)));
            }

            return beliefs;
        }

        private static GoapDebugWorld BuildWorld(IWorldState<TSnapshot> worldState)
        {
            if (worldState == null)
            {
                return new GoapDebugWorld(0, false, null);
            }

            var snapshot = worldState.CreateSnapshot();
            return new GoapDebugWorld(snapshot.Version, snapshot.IsValid, snapshot);
        }
    }
}
