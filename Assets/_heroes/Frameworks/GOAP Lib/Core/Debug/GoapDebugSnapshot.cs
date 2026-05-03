using System.Collections.Generic;
using UnityEngine;

namespace Heroes.GOAP.Core.Debug
{
    public sealed class GoapDebugSnapshot
    {
        public string AgentName { get; }
        public GoapDebugPlan Plan { get; }
        public IReadOnlyList<GoapDebugGoal> Goals { get; }
        public IReadOnlyList<GoapDebugAction> Actions { get; }
        public GoapDebugMemory Memory { get; }
        public GoapDebugWorld World { get; }
        public GoapDebugIdle Idle { get; }

        public GoapDebugSnapshot(string agentName, GoapDebugPlan plan, IReadOnlyList<GoapDebugGoal> goals, IReadOnlyList<GoapDebugAction> actions, GoapDebugMemory memory, GoapDebugWorld world, GoapDebugIdle idle)
        {
            AgentName = agentName;
            Plan = plan;
            Goals = goals;
            Actions = actions;
            Memory = memory;
            World = world;
            Idle = idle;
        }
    }

    public sealed class GoapDebugIdle
    {
        public bool IsActive { get; }
        public string Name { get; }

        public GoapDebugIdle(bool isActive, string name)
        {
            IsActive = isActive;
            Name = name;
        }
    }

    public sealed class GoapDebugPlan
    {
        public string GoalName { get; }
        public int CurrentStepIndex { get; }
        public IReadOnlyList<GoapDebugPlanStep> Steps { get; }

        public GoapDebugPlan(string goalName, int currentStepIndex, IReadOnlyList<GoapDebugPlanStep> steps)
        {
            GoalName = goalName;
            CurrentStepIndex = currentStepIndex;
            Steps = steps;
        }
    }

    public sealed class GoapDebugPlanStep
    {
        public string Name { get; }
        public string Description { get; }
        public float EstimatedTime { get; }
        public bool PreconditionsMet { get; }
        public string PreconditionsLabel { get; }
        public string EffectLabel { get; }
        public Vector2 PreviewLocation { get; }
        public IReadOnlyList<GoapDebugBelief> PreviewBeliefs { get; }
        public bool GoalAchieved { get; }
        public float GoalHeuristic { get; }

        public GoapDebugPlanStep(string name, string description, float estimatedTime, bool preconditionsMet, string preconditionsLabel, string effectLabel,
            Vector2 previewLocation, IReadOnlyList<GoapDebugBelief> previewBeliefs, bool goalAchieved, float goalHeuristic)
        {
            Name = name;
            Description = description;
            EstimatedTime = estimatedTime;
            PreconditionsMet = preconditionsMet;
            PreconditionsLabel = preconditionsLabel;
            EffectLabel = effectLabel;
            PreviewLocation = previewLocation;
            PreviewBeliefs = previewBeliefs;
            GoalAchieved = goalAchieved;
            GoalHeuristic = goalHeuristic;
        }
    }

    public sealed class GoapDebugGoal
    {
        public string Name { get; }
        public int Priority { get; }
        public float Importance { get; }
        public float Heuristic { get; }
        public bool Achieved { get; }
        public string Icon { get; }
        public string Description { get; }

        public GoapDebugGoal(string name, string description, string icon, int priority, float importance, float heuristic, bool achieved)
        {
            Name = name;
            Description = description;
            Icon = icon;
            Priority = priority;
            Importance = importance;
            Heuristic = heuristic;
            Achieved = achieved;
        }
    }

    public sealed class GoapDebugAction
    {
        public string Name { get; }
        public string Description { get; }
        public float EstimatedTime { get; }
        public bool PreconditionsMet { get; }
        public string PreconditionsLabel { get; }
        public string EffectLabel { get; }

        public GoapDebugAction(string name, string description, float estimatedTime, bool preconditionsMet, string preconditionsLabel, string effectLabel)
        {
            Name = name;
            Description = description;
            EstimatedTime = estimatedTime;
            PreconditionsMet = preconditionsMet;
            PreconditionsLabel = preconditionsLabel;
            EffectLabel = effectLabel;
        }
    }

    public sealed class GoapDebugMemory
    {
        public Vector2 Location { get; }
        public IReadOnlyList<GoapDebugBelief> Beliefs { get; }

        public GoapDebugMemory(Vector2 location, IReadOnlyList<GoapDebugBelief> beliefs)
        {
            Location = location;
            Beliefs = beliefs;
        }
    }

    public readonly struct GoapDebugBelief
    {
        public int Index { get; }
        public string Name { get; }
        public float Value { get; }

        public GoapDebugBelief(int index, string name, float value)
        {
            Index = index;
            Name = name;
            Value = value;
        }
    }

    public sealed class GoapDebugWorld
    {
        public int Version { get; }
        public bool IsValid { get; }
        public object Snapshot { get; }

        public GoapDebugWorld(int version, bool isValid, object snapshot)
        {
            Version = version;
            IsValid = isValid;
            Snapshot = snapshot;
        }
    }
}
