using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    
    
    public static class PlanningDebugSettings
    {
        public static bool Enabled;

        
        public static bool LogToFile = true;
        public static int MaxPlansToLog = 10;
        public static float FlushIntervalSeconds = 0.5f;
        public static int MaxBufferedLines = 2000;

        public static float MaxPlannerTimeMs = 2.5f;
    }

    public class Planner<TAgent, TSnapshot> : IPlanner<TAgent, TSnapshot> where TSnapshot : IReadOnlyWorldSnapshot
    {
        public Plan<TAgent, TSnapshot> Plan(List<Action<TAgent, TSnapshot>> actions, List<Goal<TSnapshot>> goals, AgentContext<TSnapshot> ctx, int maxDepth)
        {
            var orderedGoals = goals
                .Where(goal => !goal.IsAchieved(ctx))
                .OrderByDescending(goal => goal.Execute(ctx))
                .ToList();
            
            foreach (var goal in orderedGoals)
            {
                var planLogToken = GoapPlanFileLogger.BeginPlanIfNeeded(goal?.Name, PlanningDebugSettings.MaxPlansToLog);

                var startWorld = new AgentContext<TSnapshot>(ctx);
                
                
                
                var h0 = goal.Heuristic(ctx);
                var cutoff = h0 + 1f;

                
                
                
                
                
                
                var transpositionTable = new TranspositionTable(size: 2048);
                
                while (cutoff >= 0f)
                {
                    transpositionTable.Clear();

                    var plan = DoDepthFirst(startWorld,
                        goal,
                        actions,
                        transpositionTable,
                        maxDepth,
                        cutoff,
                        out var newCutoff);

                    if (plan.Count > 0)
                    {
                        DebugPlanEvent(planLogToken, $"FOUND steps={plan.Count}");
                        if (PlanningDebugSettings.Enabled && PlanningDebugSettings.LogToFile)
                        {
                            GoapPlanFileLogger.FlushIfDue(Time.unscaledTime, PlanningDebugSettings.FlushIntervalSeconds, PlanningDebugSettings.MaxBufferedLines);
                        }
                        return new Plan<TAgent, TSnapshot>(goal, plan);
                    }

                    if (Mathf.Approximately(newCutoff, float.MaxValue))
                    {
                        DebugPlanEvent(planLogToken, "NO_PLAN newCutoff=MaxValue");
                        if (PlanningDebugSettings.Enabled && PlanningDebugSettings.LogToFile)
                        {
                            GoapPlanFileLogger.FlushIfDue(Time.unscaledTime, PlanningDebugSettings.FlushIntervalSeconds, PlanningDebugSettings.MaxBufferedLines);
                        }
                        break;
                    }

                    DebugPlanEvent(planLogToken, $"DEEPEN cutoff {cutoff:0.###} -> {newCutoff:0.###}");
                    if (PlanningDebugSettings.Enabled && PlanningDebugSettings.LogToFile)
                    {
                        GoapPlanFileLogger.FlushIfDue(Time.unscaledTime, PlanningDebugSettings.FlushIntervalSeconds, PlanningDebugSettings.MaxBufferedLines);
                    }

                    cutoff = newCutoff;
                }

                DebugPlanEvent(planLogToken, "END");
                if (PlanningDebugSettings.Enabled && PlanningDebugSettings.LogToFile)
                {
                    GoapPlanFileLogger.FlushIfDue(Time.unscaledTime, PlanningDebugSettings.FlushIntervalSeconds, PlanningDebugSettings.MaxBufferedLines);
                }
            }
            
            return null;
        }

        private Stack<Action<TAgent, TSnapshot>> DoDepthFirst(AgentContext<TSnapshot> world, Goal<TSnapshot> goal, List<Action<TAgent, TSnapshot>> actions, TranspositionTable transposition, int maxDepth, float cutoff, out float smallestCutoff)
        {
            var sw = Stopwatch.StartNew();

            var models = new AgentState[maxDepth+1];
            var plan = new Action<TAgent, TSnapshot>[maxDepth];
            var costs = new float[maxDepth + 1];
            
            var actionIndex = new int[maxDepth + 1];
            for (var i = 0; i < actionIndex.Length; i++)
            {
                actionIndex[i] = -1;
            }

            models[0] = world.state.Clone();
            costs[0] = 0f;

            smallestCutoff = float.MaxValue;
            var currentDepth = 0;
            
            while (currentDepth >= 0)
            {
                if (PlanningDebugSettings.MaxPlannerTimeMs > 0f && sw.Elapsed.TotalMilliseconds >= PlanningDebugSettings.MaxPlannerTimeMs)
                {
                    smallestCutoff = float.MaxValue;
                    return new Stack<Action<TAgent, TSnapshot>>();
                }

                var ctx = new AgentContext<TSnapshot>(models[currentDepth], world.world);
                
                if (goal.IsAchieved(ctx))
                {
                    return PackStack(plan, currentDepth);
                }

                if (currentDepth >= maxDepth)
                {
                    currentDepth -= 1;
                    continue;
                }
                
                var estimate = goal.Heuristic(ctx);
                var totalCost = costs[currentDepth] + estimate;
                
                if (totalCost > cutoff)
                {
                    DebugStep(goal, cutoff, currentDepth, costs[currentDepth], estimate, null, "pruned by cutoff");
                    if (totalCost < smallestCutoff)
                    {
                        smallestCutoff = totalCost;
                    }
                    
                    currentDepth -= 1;
                    
                    continue;
                }

                actionIndex[currentDepth] += 1;
                
                if (actionIndex[currentDepth] >= actions.Count)
                {
                    actionIndex[currentDepth] = -1;
                    currentDepth -= 1;
                    
                    continue;
                }

                var nextAction = actions[actionIndex[currentDepth]];
                
                var preOk = nextAction.PreConditions(ctx);
                if (!preOk)
                {
                    DebugStep(goal, cutoff, currentDepth, costs[currentDepth], estimate, nextAction, "preconditions failed");
                    continue;
                }

                DebugStep(goal, cutoff, currentDepth, costs[currentDepth], estimate, nextAction, "preconditions ok");
                 
                var nextCost = costs[currentDepth] + nextAction.Time(ctx);
                var nextState = nextAction.Effect(ctx);

                if (transposition.HasBetterOrEqual(nextState, nextCost))
                {
                    DebugStep(goal, cutoff, currentDepth, nextCost, estimate, nextAction, "pruned by transposition");
                    continue;
                }
                
                var nextDepth = currentDepth + 1;
                transposition.AddOrImprove(nextState, nextCost);

                models[nextDepth] = nextState;
                costs[nextDepth] = nextCost;
                plan[currentDepth] = nextAction;
                
                currentDepth = nextDepth;
            }
            
            return new Stack<Action<TAgent, TSnapshot>>();
        }
        
        private Stack<Action<TAgent, TSnapshot>> PackStack(Action<TAgent, TSnapshot>[] actions, int depth)
        {
            var stack = new Stack<Action<TAgent, TSnapshot>>();

            for (var i = depth - 1; i >= 0; i--)
            {
                stack.Push(actions[i]);
            }
            
            return stack;
        }

        private static void DebugStep(Goal<TSnapshot> goal, float cutoff, int depth, float costSoFar, float estimate, Action<TAgent, TSnapshot> action, string reason)
        {
            if (!PlanningDebugSettings.Enabled)
            {
                return;
            }

            var actionName = action != null ? action.Name : "<null action>";
            var preDesc = action != null ? action.PreconditionsDescription : string.Empty;

            var msg = $"[GOAP-PLAN] goal='{goal?.Name}' depth={depth} cutoff={cutoff:0.###} cost={costSoFar:0.###} est={estimate:0.###} action='{actionName}' reason={reason} pre='{preDesc}'";
            if (PlanningDebugSettings.LogToFile)
            {
                GoapPlanFileLogger.AppendLine(msg);
            }
            else
            {
                UnityEngine.Debug.Log(msg);
            }
        }

        private static void DebugPlanEvent(int token, string msg)
        {
            if (!PlanningDebugSettings.Enabled)
            {
                return;
            }

            var line = $"[GOAP-PLAN] token={token} {msg}";
            if (PlanningDebugSettings.LogToFile)
            {
                GoapPlanFileLogger.AppendLine(line);
            }
            else
            {
                UnityEngine.Debug.Log(line);
            }
        }
    }
}


