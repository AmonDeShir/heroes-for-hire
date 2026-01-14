using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public interface IGoapPlanner
    {
        public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null);
    }

    public class GoapPlanner : IGoapPlanner
    {
        public ActionPlan Plan(GoapAgent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
        {
            var orderedGoals = goals
                .Where(goal => goal.DesiredEffects.Any(belief => !belief.Evaluate()))
                .OrderByDescending(goal => goal == mostRecentGoal ? goal.Priority - 0.01 : goal.Priority)
                .ToList();

            foreach (var goal in  orderedGoals)
            {
                var goalNode = new Node(null, null, goal.DesiredEffects, 0);

                if (FindPath(goalNode, agent.actions))
                {
                    if (goalNode.IsLeafDead)
                    {
                        continue;
                    }
                    
                    var actionStack = new Stack<AgentAction>();

                    // Find cheapest path to goal
                    while (goalNode.Leaves.Count > 0)
                    {
                        var cheapest = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();

                        goalNode = cheapest;
                        actionStack.Push(cheapest.Action);
                    }

                    return new ActionPlan(goal, actionStack, goalNode.Cost);
                }
            }

            Debug.LogWarning("Plan not found!");
            return null;
        }

        public bool FindPath(Node parent, HashSet<AgentAction> actions)
        {
            var orderedActions = actions.OrderBy(a => a.Cost);
            
            foreach (var action in orderedActions)
            {
                var requiredEffects = parent.RequiredEffects;
                
                requiredEffects.RemoveWhere(belief => belief.Evaluate());

                if (requiredEffects.Count == 0)
                {
                    return true;
                }

                if (action.Effects.Any(belief => requiredEffects.Contains(belief)))
                {
                    var newRequiredEffects = new HashSet<AgentBelief>(requiredEffects);
                    
                    newRequiredEffects.ExceptWith(action.Effects);
                    newRequiredEffects.UnionWith(action.Preconditions);
                    
                    var newNode = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);

                    if (FindPath(newNode, actions))
                    {
                        parent.Leaves.Add(newNode);
                        newRequiredEffects.ExceptWith(action.Preconditions);
                    }

                    if (newRequiredEffects.Count == 0)
                    {
                        return true;
                    }
                }
            }
            
            return parent.Leaves.Count > 0;
        }
    }

    public class Node
    {
        public Node Parent { get; }
        public AgentAction Action { get; }
        public HashSet<AgentBelief> RequiredEffects { get; }
        public List<Node> Leaves { get; }
        public float Cost { get;  }
        
        public bool IsLeafDead => Leaves.Count == 0 && Action == null;

        public Node(Node parent, AgentAction action, HashSet<AgentBelief> effects, float cost)
        {
            Parent = parent;
            Action = action;
            RequiredEffects = new HashSet<AgentBelief>(effects);
            Leaves = new List<Node>();
            Cost = cost;
        }
    }
}