using System;
using System.Collections.Generic;
using System.Linq;
using Heroes.Goap.Runtime.Core;
using Heroes.Goap.Runtime.Values;
using Heroes.Goap.Runtime.World;

namespace Heroes.Goap.Runtime.Planner
{
    public class GoapPlanner
    {
        public int MaxIterations = 2000;
        public int MaxStateCount = 1000;

        public GoapPlan Plan(GoapArchetypeAsset archetype, GoapWorldState world, GoapMemoryState memory)
        {
            return Plan(archetype, world, memory, out _, out _);
        }

        public GoapPlan Plan(GoapArchetypeAsset archetype, GoapWorldState world, GoapMemoryState memory, out GoapGoalDefinition bestGoal, out List<GoapGoalDebugInfo> debug)
        {
            bestGoal = null;
            debug = new List<GoapGoalDebugInfo>();

            if (archetype == null || archetype.Graph == null)
                return null;

            var actions = ResolveActions(archetype);
            var goals = ResolveGoals(archetype);

            if (actions.Count == 0 || goals.Count == 0)
                return null;

            GoapPlan bestPlan = null;
            float bestPriority = float.MinValue;
            float bestCost = float.MaxValue;

            foreach (var goal in goals)
            {
                var plan = PlanForGoal(goal, actions, world, memory);
                var info = new GoapGoalDebugInfo(goal, plan);
                debug.Add(info);

                if (plan == null)
                    continue;

                if (goal.Priority > bestPriority || (Math.Abs(goal.Priority - bestPriority) < 0.0001f && plan.TotalCost < bestCost))
                {
                    bestPlan = plan;
                    bestPriority = goal.Priority;
                    bestCost = plan.TotalCost;
                    bestGoal = goal;
                }
            }

            return bestPlan;
        }

        GoapPlan PlanForGoal(GoapGoalDefinition goal, List<GoapActionDefinition> actions, GoapWorldState world, GoapMemoryState memory)
        {
            var start = BuildInitialState(world, memory);
            if (GoalSatisfied(goal, start))
                return new GoapPlan();

            var open = new GoapPriorityQueue<SearchNode>();
            var visited = new HashSet<GoapState>();
            var startNode = new SearchNode(null, null, start, 0f, Heuristic(goal, start));
            open.Enqueue(startNode, startNode.FScore);

            int iterations = 0;

            while (open.Count > 0 && iterations < MaxIterations)
            {
                iterations++;

                var current = open.Dequeue();
                if (visited.Contains(current.State))
                    continue;

                visited.Add(current.State);

                if (GoalSatisfied(goal, current.State))
                    return BuildPlan(current);

                if (visited.Count >= MaxStateCount)
                    break;

                foreach (var action in actions)
                {
                    if (!ActionApplicable(action, current.State))
                        continue;

                    var nextState = ApplyEffects(action, current.State);
                    if (visited.Contains(nextState))
                        continue;

                    var gScore = current.GScore + action.BaseCost;
                    var hScore = Heuristic(goal, nextState);
                    var node = new SearchNode(current, action, nextState, gScore, hScore);
                    open.Enqueue(node, node.FScore);
                }
            }

            return null;
        }

        static GoapState BuildInitialState(GoapWorldState world, GoapMemoryState memory)
        {
            var state = new GoapState();

            if (world != null)
            {
                foreach (var pair in world.Values)
                    state.Set(pair.Key, pair.Value);
            }

            if (memory != null)
            {
                foreach (var pair in memory.Values)
                    state.Set(pair.Key, pair.Value);
            }

            return state;
        }

        static bool GoalSatisfied(GoapGoalDefinition goal, GoapState state)
        {
            foreach (var condition in goal.Desired)
            {
                if (condition == null || !condition.Evaluate(state))
                    return false;
            }

            return true;
        }

        static bool ActionApplicable(GoapActionDefinition action, GoapState state)
        {
            foreach (var condition in action.Preconditions)
            {
                if (condition == null || !condition.Evaluate(state))
                    return false;
            }

            return true;
        }

        static GoapState ApplyEffects(GoapActionDefinition action, GoapState state)
        {
            var next = state.Clone();
            foreach (var effect in action.Effects)
            {
                switch (effect.Operator)
                {
                    case GoapEffectOp.Set:
                        next.Set(effect.VariableName, effect.Value);
                        break;
                    case GoapEffectOp.Add:
                        if (next.TryGet(effect.VariableName, out var current) && current.Type == GoapValueType.Float && effect.Value.Type == GoapValueType.Float)
                        {
                            var result = GoapValue.FromFloat(current.FloatValue + effect.Value.FloatValue);
                            next.Set(effect.VariableName, result);
                        }
                        break;
                }
            }
            return next;
        }

        static float Heuristic(GoapGoalDefinition goal, GoapState state)
        {
            float score = 0f;
            foreach (var condition in goal.Desired)
            {
                if (condition == null || !condition.Evaluate(state))
                    score += 1f;
            }

            return score;
        }

        static GoapPlan BuildPlan(SearchNode node)
        {
            var plan = new GoapPlan();
            var cursor = node;
            while (cursor != null && cursor.Action != null)
            {
                plan.Actions.Add(cursor.Action);
                cursor = cursor.Parent;
            }

            plan.Actions.Reverse();
            plan.TotalCost = node.GScore;
            return plan;
        }

        static List<GoapActionDefinition> ResolveActions(GoapArchetypeAsset archetype)
        {
            var resolved = new Dictionary<string, GoapActionDefinition>();
            var stack = new Stack<GoapArchetypeAsset>();
            for (var current = archetype; current != null; current = current.Parent)
                stack.Push(current);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Graph == null)
                    continue;

                foreach (var action in current.Graph.Actions)
                    resolved[action.Id] = action;
            }

            return resolved.Values.ToList();
        }

        static List<GoapGoalDefinition> ResolveGoals(GoapArchetypeAsset archetype)
        {
            var resolved = new Dictionary<string, GoapGoalDefinition>();
            var stack = new Stack<GoapArchetypeAsset>();
            for (var current = archetype; current != null; current = current.Parent)
                stack.Push(current);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Graph == null)
                    continue;

                foreach (var goal in current.Graph.Goals)
                    resolved[goal.Id] = goal;
            }

            return resolved.Values.ToList();
        }

        class SearchNode
        {
            public readonly SearchNode Parent;
            public readonly GoapActionDefinition Action;
            public readonly GoapState State;
            public readonly float GScore;
            public readonly float HScore;
            public float FScore => GScore + HScore;

            public SearchNode(SearchNode parent, GoapActionDefinition action, GoapState state, float gScore, float hScore)
            {
                Parent = parent;
                Action = action;
                State = state;
                GScore = gScore;
                HScore = hScore;
            }
        }
    }

    public readonly struct GoapGoalDebugInfo
    {
        public readonly GoapGoalDefinition Goal;
        public readonly GoapPlan Plan;

        public GoapGoalDebugInfo(GoapGoalDefinition goal, GoapPlan plan)
        {
            Goal = goal;
            Plan = plan;
        }
    }
}
