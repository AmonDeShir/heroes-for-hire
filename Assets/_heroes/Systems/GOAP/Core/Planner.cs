using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroes.GOAP.Core
{
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
                var startWorld = new AgentContext<TSnapshot>(ctx);
                var cutoff = goal.Heuristic(ctx);
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
                        return new Plan<TAgent, TSnapshot>(goal, plan);
                    }

                    if (Mathf.Approximately(newCutoff, float.MaxValue))
                    {
                        break;
                    }

                    cutoff = newCutoff;
                }
            }
            
            return null;
        }

        private Stack<Action<TAgent, TSnapshot>> DoDepthFirst(AgentContext<TSnapshot> world, Goal<TSnapshot> goal, List<Action<TAgent, TSnapshot>> actions, TranspositionTable transposition, int maxDepth, float cutoff, out float smallestCutoff)
        {
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
                
                if (!nextAction.PreConditions(ctx))
                {
                    continue;
                }
                
                var nextCost = costs[currentDepth] + nextAction.Time(ctx);
                var nextState = nextAction.Effect(ctx);

                if (transposition.HasBetterOrEqual(nextState, nextCost))
                {
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
    }
}
