using System;
using UnityEngine;

namespace Heroes.GOAP.Core
{
    public sealed class Goal
    {
        public int Priority { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Func<AgentContext, float> Importance { get; private set; }
        public Func<AgentContext, float> Heuristic { get; private set; }
        public Func<AgentContext, bool> Achieved { get; private set; }

        private Goal()
        {
            Priority = 1;
            Name = string.Empty;
            Description = string.Empty;
            Importance = (_) => 0f;
            Achieved = (_) => false;
            Heuristic = (ctx) => IsAchieved(ctx) ? 0f : 1f;
        }
        
        public float Execute(AgentContext context)
        {
            return Importance(context) * Priority;
        }

        public bool IsAchieved(AgentContext context)
        {
            return Achieved(context);
        }

        public class Builder
        {
            private Goal goal;

            public Builder()
            {
                goal = new Goal();
            }

            public Goal Build()
            {
                return goal;
            }

            public Builder WithName(string name)
            {
                goal.Name = name;
                return this;
            }

            public Builder WithDescription(string description)
            {
                goal.Description = description;
                return this;
            }

            public Builder WithImportance(Func<AgentContext, float> importance)
            {
                goal.Importance = importance;
                return this;
            }

            public Builder WithPriority(int priority)
            {
                goal.Priority = priority;
                return this;
            }
            
            public Builder WithHeuristic(Func<AgentContext, float> heuristic)
            {
                goal.Heuristic = heuristic;
                return this;
            }
            
            public Builder WithAchieved(Func<AgentContext, bool> achieved)
            {
                goal.Achieved = achieved;
                return this;
            }
        }
    }
}
